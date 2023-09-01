using Content.Shared.Damage;

namespace Content.Shared._ArcheCrawl.ACMath;

/// <summary>
/// For math functions that can be used everywhere.
/// </summary>
public sealed partial class ArcheCrawlMath : EntitySystem
{
    public DamageModifierSet MultiplyDamageModifier(DamageModifierSet modifierSet, float multiplier)
    {
        DamageModifierSet newModifier = new();

        foreach (var coefficient in modifierSet.Coefficients)
        {
            newModifier.Coefficients[coefficient.Key] = coefficient.Value * multiplier;
        }

        foreach (var flatReduction in modifierSet.FlatReduction)
        {
            newModifier.FlatReduction[flatReduction.Key] = flatReduction.Value * multiplier;
        }

        return newModifier;
    }
}
