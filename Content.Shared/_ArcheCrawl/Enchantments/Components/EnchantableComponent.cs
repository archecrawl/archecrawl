using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._ArcheCrawl.Enchantments.Components;

/// <summary>
/// This is used for entities which can have enchantments on them.
/// Enchantments are simply arbitrary ways of item stats paired with
/// some neat visuals.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedEnchantmentSystem))]
public sealed class EnchantableComponent : Component
{
    [DataField("enchantmentContainerId")]
    public string EnchantmentContainerId = "enchantment-container";

    public IReadOnlyList<EntityUid> AllEnchantments => EnchantmentContainer!.ContainedEntities;

    /// <summary>
    /// A container that holds all of the enchantments for this entity.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Container? EnchantmentContainer;

    /// <summary>
    /// The original name of the entity. Stored here because it will be modified in the future.
    /// </summary>
    [DataField("originalName")]
    public string OriginalName = string.Empty;
}

[Serializable, NetSerializable]
public sealed class EnchantableComponentState : ComponentState
{

}
