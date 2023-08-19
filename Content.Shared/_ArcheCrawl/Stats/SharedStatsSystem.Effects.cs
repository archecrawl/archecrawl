using Content.Shared.Damage;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Random;

namespace Content.Shared._ArcheCrawl.Stats;

public abstract partial class SharedStatsSystem
{
    private void InitializeEffects()
    {
        SubscribeLocalEvent<StatScaledDamageComponent, MeleeHitEvent>(AttackDamageEffect);
        SubscribeLocalEvent<ACDodgeComponent, BeforeDamageChangedEvent>(DodgeEffect);
    }

    // so small issue, melee hit events aren't relayed to the attacker if the attacker uses a weapon. I am sad :(.
    private void AttackDamageEffect(EntityUid uid, StatScaledDamageComponent comp, MeleeHitEvent args)
    {
        args.BonusDamage = args.BaseDamage * (comp.CurMultiplier - 1);
    }

    private void DodgeEffect(EntityUid uid, ACDodgeComponent comp, ref BeforeDamageChangedEvent args)
    {
        // This isn't possible, uncomment once it is.
        // if (args.Origin == null || args.Origin == uid)
        //     return;

        if (args.Delta.Total <= 0 || !Random.Prob(Math.Clamp(comp.CurChance, 0, 1)))
            return;

        args.Cancelled = true;

        if (!NetManager.IsClient)
            PopupSystem.PopupEntity(Loc.GetString("ac-dodged-damage", ("damage", args.Delta.Total)), uid);
    }
}
