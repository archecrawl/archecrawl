using Content.Shared.Damage;
using Content.Shared._ArcheCrawl.StatEffects.Components;
using Content.Shared._ArcheCrawl.StatEffects.Components.Effects.Active;
using Content.Shared._ArcheCrawl.StatEffects.Components.Effects.Passive;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared._ArcheCrawl.ACMath;

namespace Content.Shared._ArcheCrawl.StatEffects
{
    /// <summary>
    /// // Mostly for effects that don't spawn stuff/delete entities
    /// </summary>

    public abstract partial class SharedStatEffectsSystem
    {
        [Dependency] private readonly ArcheCrawlMath _archeMath = default!;

        public void InitializeEffects()
        {
            SubscribeLocalEvent<DamageEntityEffectComponent, StatEffectActivateEvent>(DamageEffect);

            SubscribeLocalEvent<AttackDamageEffectComponent, StatEffectRelayEvent<MeleeHitEvent>>(AttackDamageEffect);
            SubscribeLocalEvent<DefenceEffectComponent, StatEffectRelayEvent<DamageModifyEvent>>(DefenceEffect);
            SubscribeLocalEvent<StatEffectIconComponent, StatEffectRelayEvent<GetStatusIconsEvent>>(OnGetStatusIcon);
        }

        #region Active Effects

        private void DamageEffect(EntityUid uid, DamageEntityEffectComponent comp, StatEffectActivateEvent args)
        {
            if (!TryComp<StatEffectComponent>(uid, out var effectComp) || !TryComp<DamageableComponent>(args.Victim, out var damageComp))
                return;

            var damage = comp.Damage;

            if (comp.ScaleWithStrength)
                damage *= effectComp.OverallStrength;

            if (!comp.IsFatalDamage &&
                _thresholdSystem.TryGetDeadThreshold(args.Victim, out var deadThreshold) &&
                deadThreshold < damageComp.Damage.Total + damage.Total)
                return;

            _damageableSystem.TryChangeDamage(args.Victim, damage, true, false, origin: args.Victim);
        }

        #endregion

        #region Passive Effects
        private void DefenceEffect(EntityUid uid, DefenceEffectComponent comp, StatEffectRelayEvent<DamageModifyEvent> args)
        {
            if (!TryComp<StatEffectComponent>(uid, out var effectComp))
                return;

            args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, _archeMath.MultiplyDamageModifier(comp.Modifiers, 1 + comp.ValueAdded * effectComp.OverallStrength));
        }

        private void AttackDamageEffect(EntityUid uid, AttackDamageEffectComponent comp, StatEffectRelayEvent<MeleeHitEvent> args)
        {
            if (!TryComp<StatEffectComponent>(uid, out var effectComp))
                return;

            args.Args.BonusDamage = DamageSpecifier.ApplyModifierSet(args.Args.BonusDamage, _archeMath.MultiplyDamageModifier(comp.Modifiers, 1 + comp.ValueAdded * effectComp.OverallStrength));
        }

        private void OnGetStatusIcon(EntityUid uid, StatEffectIconComponent component, StatEffectRelayEvent<GetStatusIconsEvent> args)
        {
            args.Args.StatusIcons.Add(PrototypeManager.Index<StatusIconPrototype>(component.StatusIcon));
        }

        #endregion
    }
}
