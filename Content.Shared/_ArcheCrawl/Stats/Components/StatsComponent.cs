using JetBrains.Annotations;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared._ArcheCrawl.Stats.Components;

/// <summary>
/// This is used for storing the <see cref="StatPrototype"/> values of an entity.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedStatsSystem)), AutoGenerateComponentState]
public sealed partial class StatsComponent : Component
{
    /// <summary>
    /// The current stat values of the entity.
    /// </summary>
    [AutoNetworkedField(true)]
    [DataField("stats", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<int, StatPrototype>))]
    public Dictionary<string, int> Stats = new();

    /// <summary>
    /// The starting values of the entity's stats.
    /// Determines what stats are supported.
    /// </summary>
    [AutoNetworkedField(true)]
    [DataField("initialStats", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<int, StatPrototype>), required: true)]
    public Dictionary<string, int> InitialStats = new();
}

/// <summary>
/// Event raised on an entity when one of its stats changes.
/// </summary>
/// <param name="Entity">The entity whose stats changed</param>
/// <param name="Stat">The stat that changed</param>
/// <param name="OldValue">The old value of the stat</param>
/// <param name="NewValue">The new value of the stat</param>
[ByRefEvent]
public readonly record struct StatChangedEvent(EntityUid Entity, StatPrototype Stat, int OldValue, int NewValue)
{
    /// <summary>
    /// The change in value.
    /// </summary>
    [PublicAPI]
    public int Delta => NewValue - OldValue;
}

[Serializable, NetSerializable]
public sealed class NetworkStatChangedEvent : EntityEventArgs
{
    public NetEntity Entity;

    public int OldValue;

    public int NewValue;

    public int Delta => NewValue - OldValue;

    public NetworkStatChangedEvent(NetEntity entity, int oldValue, int newValue)
    {
        Entity = entity;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
