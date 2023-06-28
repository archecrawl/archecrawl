using Content.Shared.Damage;

namespace Content.Shared._ArcheCrawl.StatEffects.Components.Effects.Passive
{
    [RegisterComponent]
    public sealed class AttackDamageEffectComponent : Component
    {
        [DataField("modifiers", required: true)]
        public DamageModifierSet Modifiers = default!;
    }
}
