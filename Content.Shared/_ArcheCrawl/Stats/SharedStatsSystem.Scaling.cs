using Content.Shared._ArcheCrawl.Stats.Components;
using Content.Shared.Mobs.Systems;

namespace Content.Shared._ArcheCrawl.Stats;

public abstract partial class SharedStatsSystem
{
    public void InitializeScaling()
    {
        SubscribeLocalEvent<StatScaledHealthComponent, StatChangedEvent>(OnStatVariedDeathThresholdChanged);
    }

    private void OnStatVariedDeathThresholdChanged(EntityUid uid, StatScaledHealthComponent component, ref StatChangedEvent args)
    {
        if (args.Stat.ID != component.ScalingStat)
            return;

        var val = MathF.Round(component.BaseThreshold + args.NewValue * component.ThresholdPerStat);
        MobThresholdSystem.SetMobStateThreshold(uid, val, component.TargetState);
    }
}
