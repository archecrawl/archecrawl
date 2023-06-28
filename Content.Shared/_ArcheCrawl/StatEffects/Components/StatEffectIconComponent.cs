using Content.Shared.StatusIcon;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._ArcheCrawl.StatEffects.Components;

[RegisterComponent]
public sealed class StatEffectIconComponent : Component
{
    [DataField("statusIcon", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<StatusIconPrototype>))]
    public string StatusIcon = string.Empty;
}
