using Robust.Shared.GameStates;

namespace Content.Shared._ArcheCrawl.Enchantments.Components;

/// <summary>
/// This is used for giving unique visuals to enchanted items
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed class EnchantmentVisualsComponent : Component
{
    [DataField("key")]
    public string Key = string.Empty;

    [DataField("iconVisuals")]
    public List<PrototypeLayerData> IconVisuals = new();

    /// <summary>
    /// A list of sprite layer maps that this entity
    /// has added onto its applied entity
    /// </summary>
    [DataField("spriteLayers")]
    public List<string> SpriteLayers = new();
}
