using Content.Shared.Damage;

namespace Content.Shared._ArcheCrawl.StatEffects.Effects
{
    [RegisterComponent]
    public sealed class AttackDamageEffectComponent : Component
    {
        [DataField("modifiers", required: true)]
        public DamageModifierSet Modifiers = default!;
    }
}
