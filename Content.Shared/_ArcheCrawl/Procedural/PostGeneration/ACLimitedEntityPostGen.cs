using Content.Shared.Procedural.PostGeneration;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._ArcheCrawl.Procedural.PostGeneration;

/// <summary>
/// Spawns a single instance of a given entity
/// </summary>
public sealed partial class ACLimitedEntityPostGen : IPostDunGen
{
    /// <summary>
    /// How many are we spawning
    /// </summary>
    [DataField("limit")]
    public int Limit = 1;

    [DataField("entity", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Entity = string.Empty;
}
