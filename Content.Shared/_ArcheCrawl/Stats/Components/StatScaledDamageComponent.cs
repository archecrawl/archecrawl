using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._ArcheCrawl.Stats;

[RegisterComponent, Access(typeof(SharedStatsSystem))]

/// <summary>
/// Scales damage on a stat specified.
/// </summary>
public sealed partial class StatScaledDamageComponent : Component
{
    [DataField("baseMultiplier")]
    public float BaseMultiplier = 1f;

    [DataField("valueAdded")]
    public float ValueAdded = 0.1f;

    /// <summary>
    /// The stat that scales the threshold
    /// </summary>
    [DataField("scalingStat", customTypeSerializer: typeof(PrototypeIdSerializer<StatPrototype>))]
    public string ScalingStat = "Strength";
    public float CurMultiplier = 1f;
}
