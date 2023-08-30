namespace Content.Shared._ArcheCrawl.StatEffects.Components.Effects.Active
{
    [RegisterComponent]
    public sealed partial class AdjustEffectStrengthEffectComponent : Component
    {
        /// <summary>
        /// Multiplied by the strength, and should be rounded down.
        /// </summary>
        [DataField("multiplier")]
        public float Multipler = 1f;

        /// <summary>
        /// How many stacks are added each effect
        /// </summary>
        [DataField("addedOn")]
        public int AddedOn = 0;
    }
}
