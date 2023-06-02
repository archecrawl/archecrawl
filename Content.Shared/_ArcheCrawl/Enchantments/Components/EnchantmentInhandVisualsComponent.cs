using Content.Shared.Hands.Components;
using Robust.Shared.GameStates;

namespace Content.Shared._ArcheCrawl.Enchantments.Components;

/// <summary>
/// This is used for giving unique inhand visuals to enchanted items
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed class EnchantmentInhandVisualsComponent : Component
{
    [DataField("key")]
    public string Key = string.Empty;

    [DataField("inhandVisuals")]
    public Dictionary<HandLocation, List<PrototypeLayerData>> InhandVisuals = new();
}
