using Content.Shared.Damage;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared._ArcheCrawl.Stats;

public abstract partial class SharedStatsSystem
{
    private void InitializeEffects()
    {
        SubscribeLocalEvent<StatScaledDamageComponent, MeleeHitEvent>(AttackDamageEffect);
    }

    // so small issue, melee hit events aren't relayed to the attacker if the attacker uses a weapon. I am sad :(.
    private void AttackDamageEffect(EntityUid uid, StatScaledDamageComponent comp, MeleeHitEvent args)
    {
        args.BonusDamage = args.BaseDamage * (comp.CurMultiplier - 1);
    }
}
