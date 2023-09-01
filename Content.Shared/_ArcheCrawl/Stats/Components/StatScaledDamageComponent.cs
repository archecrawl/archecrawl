using Content.Shared.Damage;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._ArcheCrawl.Stats;

[RegisterComponent, Access(typeof(SharedStatsSystem))]

/// <summary>
/// Scales damage based on the attacker's stats.
/// </summary>
public sealed partial class StatScaledDamageComponent : Component
{
    [DataField("modifiers", required: true)]
    public DamageModifierSet Modifiers = default!;

    [DataField("baseMultiplier")]
    public float BaseMultiplier = 1f;

    [DataField("valueAdded")]
    public float ValueAdded = 0.1f;

    /// <summary>
    /// The stat that scales the multiplier.
    /// </summary>
    [DataField("scalingStat", customTypeSerializer: typeof(PrototypeIdSerializer<StatPrototype>))]
    public string ScalingStat = "Strength";
}
