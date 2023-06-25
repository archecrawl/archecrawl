

namespace Content.Shared._ArcheCrawl.StatEffects.Events
{
    /// <summary>
    /// Raised every second and a half by default.
    /// </summary>
    public sealed class StatEffectUpdateEvent : EntityEventArgs { }

    /// <summary>
    /// This is used to carry over the original event's entity.
    /// </summary>
    public sealed class StatEffectRelayEvent<TEvent> : EntityEventArgs
    {
        public readonly TEvent Args;
        public EntityUid Victim;

        public StatEffectRelayEvent(TEvent args, EntityUid victim)
        {
            Args = args;
            Victim = victim;
        }
    }

    /// <summary>
    /// For when an effect loses all it's length.
    /// </summary>
    public sealed class StatEffectTimeoutEvent : EntityEventArgs
    {
        public EntityUid Victim;

        public StatEffectTimeoutEvent(EntityUid victim)
        {
            Victim = victim;
        }
    }

    /// <summary>
    /// Used for effects that need to be activated.
    /// </summary>
    public sealed class StatEffectActivateEvent : EntityEventArgs
    {
        public EntityUid Victim;
        public StatEffectActivateEvent(EntityUid victim)
        {
            Victim = victim;
        }
    }
}
