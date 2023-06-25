namespace Content.Shared._ArcheCrawl.Enchantments.Components;

/// <summary>
/// This is used for giving an entity with <see cref="EnchantableComponent"/>
/// a bunch of random chosen enchantments based on a budget.
/// </summary>
[RegisterComponent]
public sealed class RandomEnchantmentsComponent : Component
{
    /// <summary>
    /// The minimum budget allocated for enchantments.
    /// </summary>
    [DataField("budget", required: true)]
    public float Budget;

    /// <summary>
    /// The maximum budget. The true budget is a random number between <see cref="Budget"/> and <see cref="MaxBudget"/>
    /// If null, Budget will be solely used.
    /// </summary>
    [DataField("maxBudget")]
    public float? MaxBudget;
}
