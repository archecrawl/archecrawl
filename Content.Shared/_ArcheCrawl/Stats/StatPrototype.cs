using Robust.Shared.Prototypes;

namespace Content.Shared._ArcheCrawl.Stats;

/// <summary>
/// This is a prototype for a stat, a numeric
/// value that represents proficiency in a phyiscal attribute
/// </summary>
[Prototype("stat")]
public sealed class StatPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// The order the stats are displayed in
    /// </summary>
    [DataField("order")]
    public int Order = 0;

    /// <summary>
    /// A player-facing name for the stat
    /// </summary>
    [DataField("name")]
    public string Name = string.Empty;

    /// <summary>
    /// A 3-letter abbreviation for the stat's name.
    /// Used in UI to save space.
    /// </summary>
    [DataField("abbreviation")]
    public string Abbreviation = string.Empty;

    /// <summary>
    /// How low can this stat go?
    /// </summary>
    [DataField("minValue")]
    public int MinValue = 0;

    /// <summary>
    /// How high can this stat go?
    /// </summary>
    [DataField("maxValue")]
    public int MaxValue = 99;
}
