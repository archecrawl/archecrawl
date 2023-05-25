using Content.Shared._ArcheCrawl.Stats;
using Content.Shared._ArcheCrawl.Stats.Components;

namespace Content.Client._ArcheCrawl.Stats;

/// <inheritdoc/>
public sealed class StatsSystem : SharedStatsSystem
{
    public event Action<EntityUid>? OnStatsChanged;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<NetworkStatChangedEvent>(OnNetworkStatChanged);
    }

    private void OnNetworkStatChanged(NetworkStatChangedEvent msg, EntitySessionEventArgs args)
    {
        OnStatsChanged?.Invoke(msg.Entity);
    }
}
