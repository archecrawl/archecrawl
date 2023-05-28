using Content.Shared.Mobs;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._ArcheCrawl.Stats.Components;

/// <summary>
/// This is used for scaling the death
/// threshold of an entity based on a stat
/// </summary>
[RegisterComponent]
public sealed class StatScaledHealthComponent : Component
{
    /// <summary>
    /// The base value of the threshold when stat equals 0.
    /// </summary>
    [DataField("baseThreshold")]
    public float BaseThreshold = 20f;

    /// <summary>
    /// The increase in the threshold per stat
    /// </summary>
    [DataField("thresholdPerStat")]
    public float ThresholdPerStat = 2f;

    /// <summary>
    /// The stat that scales the threshold
    /// </summary>
    [DataField("scalingStat", customTypeSerializer: typeof(PrototypeIdSerializer<StatPrototype>))]
    public string ScalingStat = "Vigor";

    /// <summary>
    /// The threshold for this state is being scaled.
    /// Idk why you wouldn't want this to be Dead but here ya go.
    /// </summary>
    [DataField("targetState")]
    public MobState TargetState = MobState.Dead;
}
