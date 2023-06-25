using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Shared._ArcheCrawl.StatEffects
{
    [RegisterComponent, NetworkedComponent, Access(typeof(SharedStatEffectsSystem))]
    public sealed class StatEffectsComponent : Component
    {
        [DataField("statusContainerId")]
        public string StatusContainerId = "status-effect-container";

        [ViewVariables(VVAccess.ReadWrite)]
        public Container StatusContainer = default!;

        [ViewVariables(VVAccess.ReadWrite)]
        public TimeSpan NextActivation = TimeSpan.Zero;
    }
}
