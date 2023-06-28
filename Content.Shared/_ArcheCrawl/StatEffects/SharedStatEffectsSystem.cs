using Content.Shared._ArcheCrawl.CCVar;
using Content.Shared._ArcheCrawl.Crits.Components;
using Content.Shared._ArcheCrawl.StatEffects.Components;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._ArcheCrawl.StatEffects;

/// <summary>
/// The system in shared that controls most things related to AC's Status Effects.
/// </summary>
public abstract partial class SharedStatEffectsSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly MobThresholdSystem _thresholdSystem = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StatEffectsComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<StatEffectComponent, ComponentStartup>(OnEffectStartup);

        // Event relays down here
        SubscribeLocalEvent<StatEffectsComponent, MeleeHitEvent>(RelayEvent);
        SubscribeLocalEvent<StatEffectsComponent, BeforeDamageChangedEvent>(RefRelayEvent);
        SubscribeLocalEvent<StatEffectsComponent, DamageModifyEvent>(RelayEvent);
        SubscribeLocalEvent<StatEffectsComponent, DamageChangedEvent>(RelayEvent);
        SubscribeLocalEvent<StatEffectsComponent, GetCritChanceEvent>(RelayEvent);
        SubscribeLocalEvent<StatEffectsComponent, GetCritDamageEvent>(RelayEvent);
        SubscribeLocalEvent<StatEffectsComponent, GetStatusIconsEvent>(RefRelayEvent);

        InitializeActivation();
        InitializeEffects();
    }

    #region Events
    /// <summary>
    /// The entire stat effect economy will collapse without this.
    /// </summary>
    private void OnStartup(EntityUid uid, StatEffectsComponent component, ComponentStartup args)
    {
        component.StatusContainer = _container.EnsureContainer<Container>(uid, component.StatusContainerId);
        component.StatusContainer.OccludesLight = false;
    }

    private void OnEffectStartup(EntityUid uid, StatEffectComponent comp, ComponentStartup args)
    {
        comp.Length = Timing.CurTime + TimeSpan.FromSeconds(comp.DefaultLength);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = Timing.CurTime;

        var query = EntityQueryEnumerator<StatEffectsComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            var args = new StatEffectUpdateEvent();
            if (comp.NextActivation < curTime)
            {
                comp.NextActivation = curTime + TimeSpan.FromSeconds(_cfg.GetCVar(ACCCVars.StatusEffectUpdateInterval));
                RelayEvent(uid, comp, args);
            }
        }
    }

    #endregion

    #region Funcitons

    /// <summary>
    /// Used to apply a status effect onto an entity "The Intended Way™"
    /// </summary>
    /// <param name="uid">The player that is recieving the status effect</param>
    /// <param name="statusEffect">The prototype of the status effect being applied</param>
    /// <param name="strength">How powerful is the effect?</param>
    /// <param name="newLength">How long should the effect last</param>
    /// <param name="addOn">Should this add strength to the entity or simply just override it?</param>
    /// <param name="overrideEffect">If true, and there is already an effect present, it will be overwritten</param>
    /// <param name="comp"></param>
    public EntityUid? ApplyEffect(
        EntityUid uid,
        string statusEffect,
        int strength = 1,
        TimeSpan? newLength = null,
        bool addOn = true,
        bool overrideEffect = false,
        StatEffectsComponent? comp = null)
    {
        if (!Resolve(uid, ref comp))
            return null;

        if (comp.StatusContainer == null)
            return null;

        if (!PrototypeManager.TryIndex<EntityPrototype>(statusEffect, out var statusPrototype))
        {
            Log.Error($"Entity prototype of '{statusEffect}' could not be found.");
            return null;
        }

        foreach (var storedEffect in comp.StatusContainer.ContainedEntities)
        {
            if (TryComp<StatEffectComponent>(storedEffect, out var statEffect) &&
                TryComp<MetaDataComponent>(storedEffect, out var metaData) && metaData.EntityPrototype == statusPrototype)
            {
                if (addOn)
                    ModifyEffect(storedEffect, statEffect.OverallStrength + strength, newLength, overrideEffect, statEffect);
                else
                    ModifyEffect(storedEffect, strength, newLength, overrideEffect, statEffect);

                return storedEffect;
            }
        }

        var effect = Spawn(statusEffect, Transform(uid).Coordinates);
        ModifyEffect(effect, strength, newLength, true);

        comp.StatusContainer.Insert(effect);

        return effect;
    }


    /// <summary>
    /// Should only be used on an already applied effect. For properly applying effects, see ApplyEffect()
    /// </summary>
    /// <param name="uid">The effect UID</param>
    /// <param name="newStrength">What should be the new size of the effect?</param>
    /// <param name="newLength">Should the effect have a different length?</param>
    /// <param name="overrideEffect">Should the current effect settings be overwritten despite it's strength/length</param>
    /// <param name="comp"></param>
    public void ModifyEffect(EntityUid uid, int newStrength, TimeSpan? newLength = null, bool overrideEffect = false, StatEffectComponent? comp = null)
    {
        if (!Resolve(uid, ref comp))
            return;

        if (!overrideEffect && newStrength < comp.OverallStrength)
            return;

        if (newStrength <= 0)
        {
            QueueDel(uid);
            return;
        }

        comp.OverallStrength = comp.MaxStrength > 0
            ? Math.Clamp(newStrength, 0, comp.MaxStrength)
            : newStrength;

        var curTime = Timing.CurTime;

        if (newLength == null || !overrideEffect && curTime + newLength.Value > comp.Length)
            return;

        comp.Length = curTime + newLength.Value;
    }

    #endregion

    #region Relays

    /// <summary>
    /// Used to relay an event that an entity recieved into it's effects so that the event can be modified by the effects.
    /// </summary>
    private void RelayEvent<TEvent>(EntityUid uid, StatEffectsComponent comp, TEvent args)
    {
        var relayedArgs = new StatEffectRelayEvent<TEvent>(args, uid);

        if (comp.StatusContainer == null)
            return;

        foreach (var effect in comp.StatusContainer.ContainedEntities)
        {
            RaiseLocalEvent(effect, relayedArgs);
        }
    }

    /// <summary>
    /// A ref version of RelayEvent. Does the same thing as it.
    /// </summary>
    private void RefRelayEvent<TEvent>(EntityUid uid, StatEffectsComponent comp, ref TEvent args)
    {
        var relayedArgs = new StatEffectRelayEvent<TEvent>(args, uid);

        if (comp.StatusContainer == null)
            return;

        foreach (var effect in comp.StatusContainer.ContainedEntities)
        {
            RaiseLocalEvent(effect, relayedArgs);
        }
    }

    #endregion
}
