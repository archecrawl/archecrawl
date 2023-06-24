using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._ArcheCrawl.CCVar
{
    public sealed class ACCCVars : CVars
    {
        /// <summary>
        /// Update interval of status effects, in seconds
        /// </summary>
        /// <returns></returns>
        public static readonly CVarDef<float> StatusEffectUpdateInterval =
            CVarDef.Create<float>("status_effect.update_interval", 2f, CVar.SERVER);
    }
}
