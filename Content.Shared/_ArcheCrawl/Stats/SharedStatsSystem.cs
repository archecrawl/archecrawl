using Content.Shared._ArcheCrawl.Stats.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using JetBrains.Annotations;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._ArcheCrawl.Stats;

/// <summary>
/// This handles modifying and accessing <see cref="StatPrototype"/>
/// values from <see cref="StatsComponent"/>
/// </summary>
public abstract partial class SharedStatsSystem : EntitySystem
{
    [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;
    [Dependency] protected readonly MobThresholdSystem MobThresholdSystem = default!;
    [Dependency] protected readonly IRobustRandom Random = default!;
    [Dependency] protected readonly SharedPopupSystem PopupSystem = default!;
    [Dependency] protected readonly INetManager NetManager = default!;

    private ISawmill _sawmill = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        InitializeScaling();
        InitializeEffects();

        SubscribeLocalEvent<StatsComponent, ComponentInit>(OnCompInit);

        _sawmill = Logger.GetSawmill("stat");
    }

    private void OnCompInit(EntityUid uid, StatsComponent component, ComponentInit args)
    {
        foreach (var (key, val) in component.InitialStats)
        {
            component.Stats[key] = default;
            SetStatValue(uid, key, val, component);
        }
        Dirty(component);
    }

    #region Stat Setters
    [PublicAPI]
    public void ModifyStatValue(EntityUid uid, string stat, int delta, StatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        if (!component.Stats.TryGetValue(stat, out var val))
            return;

        SetStatValue(uid, stat, val + delta, component);
    }

    [PublicAPI]
    public void ModifyStatValue(EntityUid uid, StatPrototype stat, int delta, StatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        if (!component.Stats.TryGetValue(stat.ID, out var val))
            return;

        SetStatValue(uid, stat, val + delta, component);
    }

    public void SetStatValue(EntityUid uid, string stat, int value, StatsComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        if (!PrototypeManager.TryIndex<StatPrototype>(stat, out var statPrototype))
        {
            _sawmill.Error($"Invalid stat prototype ID: \"{stat}\"");
            return;
        }

        SetStatValue(uid, statPrototype, value, component);
    }

    public void SetStatValue(EntityUid uid, StatPrototype stat, int value, StatsComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        if (!component.Stats.ContainsKey(stat.ID))
            return;

        var clampedVal = Math.Clamp(value, stat.MinValue, stat.MaxValue);
        var ev = new StatChangedEvent(uid, stat, component.Stats[stat.ID], clampedVal);
        var netEv = new NetworkStatChangedEvent(GetNetEntity(uid), component.Stats[stat.ID], clampedVal);
        component.Stats[stat.ID] = clampedVal;
        Dirty(uid, component);
        RaiseLocalEvent(uid, ref ev, true);
        RaiseNetworkEvent(netEv, uid);
    }
    #endregion

    #region Stat Getters
    [PublicAPI]
    public int GetStat(EntityUid uid, StatPrototype stat, StatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return 0;

        return GetStat(uid, stat.ID, component);
    }

    public int GetStat(EntityUid uid, string stat, StatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return 0;

        if (!component.Stats.ContainsKey(stat))
            return 0;

        return component.Stats.GetValueOrDefault(stat);
    }
    #endregion
}
