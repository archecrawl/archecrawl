using Content.Shared._ArcheCrawl.Crits.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._ArcheCrawl.Crits;

public sealed class RandomCritSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<RandomCritMeleeComponent, MeleeHitEvent>(OnRandomCritMeleeHit);
        SubscribeLocalEvent<BonusCritChanceComponent, GetCritChanceEvent>(OnGetBonusCritChance);
        SubscribeLocalEvent<BonusCritDamageComponent, GetCritDamageEvent>(OnGetBonusCritDamage);
    }

    private void OnRandomCritMeleeHit(EntityUid uid, RandomCritMeleeComponent component, MeleeHitEvent args)
    {
        if (!args.IsHit)
            return;

        foreach (var hitEnt in args.HitEntities)
        {
            if (component.CritQueued)
            {
                args.BonusDamage += args.BaseDamage * GetCritDamageMultiplier(uid, args.User, component);
                var ev = new CriticalHitEvent(uid, args.User, hitEnt);
                RaiseLocalEvent(uid, ref ev);

                if (component.CritSound != null)
                    args.HitSoundOverride = component.CritSound;
                if (_net.IsClient && _timing.IsFirstTimePredicted)
                    _popup.PopupEntity(Loc.GetString("random-crit-popup"), hitEnt, PopupType.SmallCaution);
            }
            RerollCrit(uid, args.User, component);
        }
    }

    private void OnGetBonusCritChance(EntityUid uid, BonusCritChanceComponent component, GetCritChanceEvent args)
    {
        args.CritChance += component.FlatModifier;
        args.Multipliers *= component.Multiplier;
    }

    private void OnGetBonusCritDamage(EntityUid uid, BonusCritDamageComponent component, GetCritDamageEvent args)
    {
        args.DamageMultiplier += component.FlatModifier;
        args.Multipliers *= component.Multiplier;
    }

    public void RerollCrit(EntityUid uid, EntityUid user, RandomCritMeleeComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        // I'd rather mispredict no crit on a laggy client
        // than mispredict multiple crits in a row.
        if (_net.IsClient)
        {
            component.CritQueued = false;
            return;
        }

        component.CritQueued = _random.Prob(GetCritChance(uid, user, component));
        Dirty(component);
    }

    public float GetCritChance(EntityUid uid, EntityUid user, RandomCritMeleeComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return 0;

        var ev = new GetCritChanceEvent(uid, component.BaseCritChance, user);
        RaiseLocalEvent(user, ev);
        RaiseLocalEvent(uid, ev);

        return Math.Clamp(ev.CritChance * ev.Multipliers, 0f, 1f);
    }

    public float GetCritDamageMultiplier(EntityUid uid, EntityUid user, RandomCritMeleeComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return 0;

        var ev = new GetCritDamageEvent(uid, component.DamageMultiplier, user);
        RaiseLocalEvent(user, ev);
        RaiseLocalEvent(uid, ev);

        return MathF.Max(ev.DamageMultiplier * ev.Multipliers - 1, 1);
    }
}
