using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._ArcheCrawl.Stats;

[RegisterComponent, Access(typeof(SharedStatsSystem))]

/// <summary>
/// Allows the user to dodge attacks. Chance is based on stats.
/// </summary>
public sealed partial class ACDodgeComponent : Component
{
    /// <summary>
    /// Base chance for a successful dodging
    /// </summary>
    [DataField("baseChance")]
    public float BaseChance = 0;

    /// <summary>
    /// Chance thats going to multiplied by the amount of levels the entity has.
    /// </summary>
    [DataField("valueAdded")]
    public float ValueAdded = 0.005f;

    /// <summary>
    /// The stat that scales the chance
    /// </summary>
    [DataField("scalingStat", customTypeSerializer: typeof(PrototypeIdSerializer<StatPrototype>))]
    public string ScalingStat = "Dexterity"; // Maybe it could be a mix of dexterity and inteligience? Probs needs a discussion.
}
