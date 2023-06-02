namespace Content.Shared._ArcheCrawl.Crits.Components;

[RegisterComponent]
public sealed class BonusCritDamageComponent : Component
{
    [DataField("flatModifier")]
    public float FlatModifier;

    [DataField("multiplier")]
    public float Multiplier = 1;
}
