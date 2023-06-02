using Content.Shared.Inventory;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._ArcheCrawl.Crits.Components;

/// <summary>
/// This is used for melee weapons which have
/// a random chance of dealing critical hits.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RandomCritMeleeComponent : Component
{
    /// <summary>
    /// Whether or not the next hit will be a critical hit.
    /// Done for prediction purposes.
    /// </summary>
    [DataField("critQueued"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public bool CritQueued;

    /// <summary>
    /// The base chance that a weapon will crit on a hit.
    /// </summary>
    [DataField("baseCritChance"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public float BaseCritChance = 0.05f;

    /// <summary>
    /// How much the damage is multiplied when there's a crit.
    /// </summary>
    [DataField("damageMultiplier"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public float DamageMultiplier = 3;

    /// <summary>
    /// The sound played when you make a critical hit
    /// </summary>
    [DataField("critSound")]
    public SoundSpecifier? CritSound = new SoundPathSpecifier("/Audio/_ArcheCrawl/crit.ogg")
    {
        Params = new AudioParams
        {
            Volume = 2f,
            Variation = 0.1f
        }
    };
}

/// <summary>
/// Event raised on the weapon when
/// an entity is dealt a critical hit.
/// </summary>
[ByRefEvent]
public readonly record struct CriticalHitEvent(EntityUid Weapon, EntityUid User, EntityUid Target);

/// <summary>
/// Event raised to calculate the chance of getting a critical hit.
/// </summary>
public sealed class GetCritChanceEvent : EntityEventArgs, IInventoryRelayEvent
{
    public SlotFlags TargetSlots => SlotFlags.All & ~SlotFlags.POCKET;

    public EntityUid Weapon;

    public float CritChance;

    public float Multipliers = 1;

    public EntityUid User;

    public GetCritChanceEvent(EntityUid weapon, float critChance, EntityUid user)
    {
        Weapon = weapon;
        CritChance = critChance;
        User = user;
    }
}

/// <summary>
/// Event raised to calculate the damage done by a critical hit
/// </summary>
public sealed class GetCritDamageEvent : EntityEventArgs, IInventoryRelayEvent
{
    public SlotFlags TargetSlots => SlotFlags.All & ~SlotFlags.POCKET;

    public EntityUid Weapon;

    public float DamageMultiplier;

    public float Multipliers = 1;

    public EntityUid User;

    public GetCritDamageEvent(EntityUid weapon, float damageMultiplier, EntityUid user)
    {
        Weapon = weapon;
        DamageMultiplier = damageMultiplier;
        User = user;
    }
}
