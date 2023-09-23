using System.Linq;
using Content.Shared._ArcheCrawl.Enchantments.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._ArcheCrawl.Enchantments;

public abstract partial class SharedEnchantmentSystem
{
    private void OnRandomEnchantmentsMapInit(EntityUid uid, RandomEnchantmentsComponent component, MapInitEvent args)
    {
        var budget = component.MaxBudget == null
            ? component.Budget
            : MathF.Round(_random.NextFloat(component.Budget, component.MaxBudget.Value));

        GenerateRandomEnchantments(uid, budget, component);
    }

    public void GenerateRandomEnchantments(EntityUid uid, float budget, RandomEnchantmentsComponent? random = null, EnchantableComponent? component = null)
    {
        if (!Resolve(uid, ref random, ref component, false))
            return;

        while (budget > 0)
        {
            var available = GetValidEnchantments(uid, component).Where(pair => pair.Value.Cost <= budget).ToList();

            if (!available.Any())
                break;

            var (prototype, comp) = _random.Pick(available);
            ApplyEnchantment(uid, prototype, component);
            budget -= comp.Cost;
        }
    }

    public IEnumerable<KeyValuePair<string, EnchantmentComponent>> GetValidEnchantments(EntityUid uid, EnchantableComponent component)
    {
        foreach (var entity in PrototypeManager.EnumeratePrototypes<EntityPrototype>())
        {
            if (!entity.TryGetComponent<EnchantmentComponent>(out var enchantment, EntityManager.ComponentFactory))
                continue;

            if (CanApplyEnchantment(uid, entity, component, enchantment))
                yield return new KeyValuePair<string, EnchantmentComponent>(entity.ID, enchantment);
        }
    }
}
