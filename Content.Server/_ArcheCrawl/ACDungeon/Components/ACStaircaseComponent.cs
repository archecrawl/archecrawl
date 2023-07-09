using Robust.Shared.Map;

namespace Content.Server._ArcheCrawl.ACDungeon.Components;

[RegisterComponent]
public sealed class ACStaircaseComponent : Component
{
    /// <summary>
    /// The staircase that is linked to this one, as a way to move backwards.
    /// </summary>
    [DataField("linkedStair")]
    public EntityUid? LinkedStair;

    public bool Generating = false;
}
