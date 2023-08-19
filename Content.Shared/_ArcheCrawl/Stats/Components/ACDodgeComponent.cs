using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._ArcheCrawl.Stats;

[RegisterComponent, Access(typeof(SharedStatsSystem))]

/// <summary>
/// Allows the user to dodge attacks. Chance is based on a stat.
/// </summary>
public sealed partial class ACDodgeComponent : Component
{
    [DataField("baseChance")]
    public float BaseChance = 0;

    [DataField("valueAdded")]
    public float ValueAdded = 0.005f;

    /// <summary>
    /// The stat that scales the threshold
    /// </summary>
    [DataField("scalingStat", customTypeSerializer: typeof(PrototypeIdSerializer<StatPrototype>))]
    public string ScalingStat = "Dexterity"; // Maybe it could be a mix of dexterity and inteligience? Probs needs a discussion.
    public float CurChance = 0f;
}
