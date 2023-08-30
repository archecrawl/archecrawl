using Robust.Shared.GameStates;

namespace Content.Shared._ArcheCrawl.StatEffects.Components
{
    [RegisterComponent, NetworkedComponent, Access(typeof(SharedStatEffectsSystem))]
    public sealed partial class StatEffectComponent : Component
    {
        /// <summary>
        /// Anything less than 1 will make it so that the effect can have infinite strength.
        /// </summary>
        [DataField("maxStrength")]
        public int MaxStrength = -1;

        /// <summary>
        /// Should be set with SharedStatEffectsSystem via SetStrength, not manually!
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        public int OverallStrength = 1;

        #region Timer

        /// <summary>
        /// Some effects may not want to be on a timer, so the option is left here.
        /// </summary>
        [DataField("isTimed")]
        public bool IsTimed = true;

        /// <summary>
        /// The default start time when the effect is first spawned in, in seconds.
        /// </summary>
        [DataField("defaultLength")]
        public float DefaultLength = 0f;

        [ViewVariables(VVAccess.ReadWrite)]
        public TimeSpan Length = TimeSpan.Zero;

        #endregion
    }
}
