namespace Content.Shared._ArcheCrawl.StatEffects.Components.Effects.Inflictors
{
    [RegisterComponent]
    public sealed partial class InflictEffectOnDamageComponent : Component
    {
        /// <summary>
        /// The effect that's going to be applied
        /// </summary>
        [DataField("effect", required: true)]
        public string Effect = string.Empty;

        /// <summary>
        /// The strength of the effect
        /// </summary>
        [DataField("strength")]
        public int Strength = 1;

        /// <summary>
        /// The length of the effect that will be applied, in seconds.
        /// </summary>
        [DataField("length")]
        public float Length = 0;

        /// <summary>
        /// Should the effect add ontop of the victim's current effect?
        /// </summary>
        [DataField("addOn")]
        public bool AddOn = false;

        /// <summary>
        /// Should the effect replace the current one?
        /// </summary>
        [DataField("replace")]
        public bool Replace = false;
    }
}
