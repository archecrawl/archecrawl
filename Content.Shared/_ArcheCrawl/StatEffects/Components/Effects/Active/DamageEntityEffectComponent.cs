using Content.Shared.Damage;

namespace Content.Shared._ArcheCrawl.StatEffects.Components.Effects.Active
{
    [RegisterComponent]
    public sealed class DamageEntityEffectComponent : Component
    {
        /// <summary>
        /// Multiplied by the strength.
        /// </summary>
        [DataField("damage", required: true)]
        public DamageSpecifier Damage = new();

        /// <summary>
        /// Should the damage be multiplied by the strength?
        /// </summary>
        [DataField("scaleWithStrength")]
        public bool ScaleWithStrength = true;

        /// <summary>
        /// Should the damage be fatal? Will avoid dealing damage if it isn't and the player would die to the next hit.
        /// </summary>
        [DataField("isFatalDamage")]
        public bool IsFatalDamage = true;
    }
}
