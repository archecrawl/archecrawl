using Content.Shared.Hands.Components;

namespace Content.Shared._ArcheCrawl.Enchantments.Components;

/// <summary>
/// This is used for giving unique visuals to enchanted items
/// </summary>
[RegisterComponent]
public sealed class EnchantmentVisualsComponent : Component
{
    [DataField("key")]
    public string Key = string.Empty;

    [DataField("iconVisuals")]
    public List<PrototypeLayerData> IconVisuals = new();

    [DataField("inhandVisuals")]
    public Dictionary<HandLocation, List<PrototypeLayerData>> InhandVisuals = new();

    /// <summary>
    /// A list of sprite layer maps that this entity
    /// has added onto its applied entity
    /// </summary>
    [DataField("spriteLayers")]
    public List<string> SpriteLayers = new();
}
