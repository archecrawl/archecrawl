using Content.Shared._ArcheCrawl.CCVar;
using Content.Shared._ArcheCrawl.Crits.Components;
using Content.Shared._ArcheCrawl.StatEffects.Events;
using Content.Shared.Damage;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

/// I am so sorry.
namespace Content.Shared._ArcheCrawl.StatEffects
{
    /// <summary>
    /// The system in shared that controls most things related to AC's Status Effects.
    /// </summary>
    public sealed class SharedStatEffectsSystem : EntitySystem
    {
        [Dependency] private readonly IGameTiming _gameTiming = default!;
        [Dependency] private readonly IPrototypeManager _protoManager = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        [Dependency] private readonly SharedContainerSystem _container = default!;

        private ISawmill _sawmill = default!;

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
            _sawmill = Logger.GetSawmill("effects");
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
            comp.Length = _gameTiming.CurTime + TimeSpan.FromSeconds(comp.DefaultLength);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            var curTime = _gameTiming.CurTime;

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

            if (!_protoManager.TryIndex<EntityPrototype>(statusEffect, out var statusPrototype))
            {
                _sawmill.Error("Entity prototype of '" + statusEffect + "' could not be found.");
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

            if (comp.MaxStrength > 0)
                comp.OverallStrength = Math.Clamp(newStrength, 0, comp.MaxStrength);
            else
                comp.OverallStrength = newStrength;

            var curTime = _gameTiming.CurTime;

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
                RaiseLocalEvent(effect, relayedArgs);
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
                RaiseLocalEvent(effect, relayedArgs);
        }

        #endregion
    }
}
