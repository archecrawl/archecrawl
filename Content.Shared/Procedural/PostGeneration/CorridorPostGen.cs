using Content.Shared.Maps;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Procedural.PostGeneration;

/// <summary>
/// Connects room entrances via corridor segments.
/// </summary>
public sealed partial class CorridorPostGen : IPostDunGen
{
    /// <summary>
    /// How far we're allowed to generate a corridor before calling it.
    /// </summary>
    /// <remarks>
    /// Given the heavy weightings this needs to be fairly large for larger dungeons.
    /// </remarks>
    [DataField("pathLimit")]
    public int PathLimit = 2048;

    [DataField("method")]
    public CorridorPostGenMethod Method = CorridorPostGenMethod.MinimumSpanningTree;

    /// <summary>
    /// How wide to make the corridor.
    /// </summary>
    [DataField("width")]
    public int Width = 3;

    [DataField("corridorTile", customTypeSerializer: typeof(PrototypeIdSerializer<ContentTileDefinition>))]
    public string CorridorTile = "FloorSteel";
}

public enum CorridorPostGenMethod : byte
{
    Invalid,
    MinimumSpanningTree,
}
