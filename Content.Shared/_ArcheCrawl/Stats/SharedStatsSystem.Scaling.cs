using Content.Shared._ArcheCrawl.Stats.Components;
using Content.Shared.Mobs.Systems;

namespace Content.Shared._ArcheCrawl.Stats;

public abstract partial class SharedStatsSystem
{
    [Dependency] private readonly MobThresholdSystem _mobThreshold = default!;

    public void InitializeScaling()
    {
        SubscribeLocalEvent<StatVariedThresholdComponent, StatChangedEvent>(OnStatVariedDeathThresholdChanged);
    }

    private void OnStatVariedDeathThresholdChanged(EntityUid uid, StatVariedThresholdComponent component, ref StatChangedEvent args)
    {
        if (args.Stat.ID != component.ScalingStat)
            return;

        var val = MathF.Round(component.BaseThreshold + args.NewValue * component.ThresholdPerStat);
        _mobThreshold.SetMobStateThreshold(uid, val, component.TargetState);
    }
}
