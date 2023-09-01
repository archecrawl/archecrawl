using Content.Shared._ArcheCrawl.ACMath;
using Content.Shared._ArcheCrawl.Stats.Components;
using Content.Shared.Damage;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Random;

namespace Content.Shared._ArcheCrawl.Stats;

public abstract partial class SharedStatsSystem
{
    [Dependency] private readonly ArcheCrawlMath _archeMath = default!;

    private void InitializeEffects()
    {
        SubscribeLocalEvent<StatScaledDamageComponent, MeleeHitEvent>(IncreaseMeleeDamage);
        SubscribeLocalEvent<StatScaledDamageComponent, ProjectileHitEvent>(IncreaseProjectileDamage);
        SubscribeLocalEvent<ACDodgeComponent, BeforeDamageChangedEvent>(TryDodge);
        SubscribeLocalEvent<ACDodgeComponent, ACBeforeDodgeEvent>(OnDodgeEvent);
    }

    private void IncreaseMeleeDamage(EntityUid uid, StatScaledDamageComponent comp, MeleeHitEvent args)
    {
        if (!TryComp<StatsComponent>(args.User, out var statsComp)
        || !statsComp.Stats.ContainsKey(comp.ScalingStat))
            return;

        var statValue = GetStat(args.User, comp.ScalingStat, statsComp);

        args.ModifiersList.Add(_archeMath.MultiplyDamageModifier(comp.Modifiers, comp.BaseMultiplier + comp.ValueAdded * statValue));
    }

    private void IncreaseProjectileDamage(EntityUid uid, StatScaledDamageComponent comp, ref ProjectileHitEvent args)
    {
        if (args.Shooter == null
        || !TryComp<StatsComponent>(args.Shooter, out var statsComp)
        || !statsComp.Stats.ContainsKey(comp.ScalingStat))
            return;

        var statValue = GetStat(args.Shooter.Value, comp.ScalingStat, statsComp);

        args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, _archeMath.MultiplyDamageModifier(comp.Modifiers, comp.BaseMultiplier + comp.ValueAdded * statValue));
    }

    private void OnDodgeEvent(EntityUid uid, ACDodgeComponent comp, ACBeforeDodgeEvent args)
    {
        if (!TryComp<StatsComponent>(uid, out var statsComp) || !statsComp.Stats.ContainsKey(comp.ScalingStat))
            return;

        var statValue = GetStat(uid, comp.ScalingStat, statsComp);

        if (Random.Prob(Math.Clamp(comp.BaseChance + comp.ValueAdded * statValue, 0, 1)))
            args.Success = true;
    }

    private void TryDodge(EntityUid uid, ACDodgeComponent comp, ref BeforeDamageChangedEvent args)
    {
        if (args.Origin == null || args.Origin == uid || args.Delta.Total <= 0)
            return;
        var ev = new ACBeforeDodgeEvent(args.Delta, args.Origin);

        RaiseLocalEvent(uid, ev);

        if (ev.Cancelled || !ev.Success)
            return;

        args.Cancelled = true;

        if (!NetManager.IsClient)
            PopupSystem.PopupEntity(Loc.GetString("ac-dodged-damage", ("damage", args.Delta.Total)), uid);
    }
}

public sealed class ACBeforeDodgeEvent : EntityEventArgs
{
    public DamageSpecifier Damage;
    public EntityUid? Origin;
    public bool Success;
    public bool Cancelled;

    /// <summary>
    /// Fired right before the entity dodges.
    /// </summary>
    /// <param name="origin">The entity that caused the damage.
    /// <param name="success">For when you want the dodge to succeed.</param>
    /// <param name="cancelled">If, for some reason, you want to cancel the dodge entirely. Could be used for "undodgeable" attacks.</param>
    public ACBeforeDodgeEvent(DamageSpecifier damage, EntityUid? origin = null, bool success = false, bool cancelled = false)
    {
        Damage = damage;
        Origin = origin;
        Success = success;
        Cancelled = cancelled;
    }
}
