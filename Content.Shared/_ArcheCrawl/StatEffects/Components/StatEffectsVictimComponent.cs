using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Shared._ArcheCrawl.StatEffects.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedStatEffectsSystem))]
public sealed partial class StatEffectsComponent : Component
{
    [DataField("statusContainerId")]
    public string StatusContainerId = "status-effect-container";

    [ViewVariables(VVAccess.ReadWrite)]
    public Container? StatusContainer = default!;

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextActivation = TimeSpan.Zero;
}

