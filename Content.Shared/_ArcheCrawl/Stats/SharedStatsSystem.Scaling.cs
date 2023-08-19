using Content.Shared._ArcheCrawl.Stats.Components;
using Content.Shared.Mobs.Systems;

namespace Content.Shared._ArcheCrawl.Stats;

public abstract partial class SharedStatsSystem
{
    public void InitializeScaling()
    {
        SubscribeLocalEvent<StatScaledHealthComponent, StatChangedEvent>(OnStatVariedDeathThresholdChanged);
        SubscribeLocalEvent<StatScaledDamageComponent, StatChangedEvent>(OnStatChangedDamageScaling);
        SubscribeLocalEvent<ACDodgeComponent, StatChangedEvent>(OnStatChangedDodgeScaling);
    }

    private void OnStatVariedDeathThresholdChanged(EntityUid uid, StatScaledHealthComponent component, ref StatChangedEvent args)
    {
        if (args.Stat.ID != component.ScalingStat)
            return;

        var val = MathF.Round(component.BaseThreshold + args.NewValue * component.ThresholdPerStat);
        MobThresholdSystem.SetMobStateThreshold(uid, val, component.TargetState);
    }

    private void OnStatChangedDamageScaling(EntityUid uid, StatScaledDamageComponent comp, ref StatChangedEvent args)
    {
        if (args.Stat.ID != comp.ScalingStat)
            return;

        comp.CurMultiplier = comp.BaseMultiplier + args.NewValue * comp.ValueAdded; // Hello everybody my name is multiplier.
    }

    private void OnStatChangedDodgeScaling(EntityUid uid, ACDodgeComponent comp, ref StatChangedEvent args)
    {
        if (args.Stat.ID != comp.ScalingStat)
            return;

        comp.CurChance = comp.BaseChance + args.NewValue * comp.ValueAdded;
    }
}
