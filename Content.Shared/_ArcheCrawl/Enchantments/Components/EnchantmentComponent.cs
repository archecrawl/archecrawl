using Content.Shared.Tag;
using JetBrains.Annotations;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared._ArcheCrawl.Enchantments.Components;

/// <summary>
/// This is used for an item enchantment, a way of arbitrarily
/// applying extra behavior and visuals to an item.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedEnchantmentSystem))]
public sealed class EnchantmentComponent : Component
{
    /// <summary>
    /// The entity this enchantment is applied to.
    /// </summary>
    [DataField("appliedEntity")]
    public EntityUid AppliedEntity;

    /// <summary>
    /// The cost associated with the random selection of this enchantment
    /// </summary>
    [DataField("cost")]
    public float Cost = 1;

    /// <summary>
    /// Whether or not the <see cref="Descriptor"/> goes in the front or the back
    /// </summary>
    [DataField("placement"), ViewVariables(VVAccess.ReadWrite)]
    public DescriptorPlacement Placement = DescriptorPlacement.Prefix;

    /// <summary>
    /// A prefix or suffix attached to the name of the entity
    /// </summary>
    [DataField("descriptor"), ViewVariables(VVAccess.ReadWrite)]
    public string? Descriptor;

    /// <summary>
    /// A description used when an enchantment is being examined.
    /// </summary>
    [DataField("description"), ViewVariables(VVAccess.ReadWrite)]
    public string? Description;

    /// <summary>
    /// A list of weapon classes this enchantment can be applied to.
    /// </summary>
    [DataField("validWeaponClasses", customTypeSerializer: typeof(PrototypeIdListSerializer<TagPrototype>))]
    public List<string> ValidWeaponClasses = new();

    /// <summary>
    /// A list of enchantments that cannot be on the same entity as this enchantment.
    /// </summary>
    [DataField("exclusiveEnchantments", customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
    public List<string> ExclusiveEnchantments = new();
}

[Serializable, NetSerializable]
public enum DescriptorPlacement : byte
{
    Suffix,
    Prefix
}

/// <summary>
/// Event raised when an entity is enchanted.
/// </summary>
/// <param name="Enchantable"></param>
/// <param name="Enchantment"></param>
[ByRefEvent]
public readonly record struct EnchantmentAppliedEvent(EntityUid Enchantable, EntityUid Enchantment);
