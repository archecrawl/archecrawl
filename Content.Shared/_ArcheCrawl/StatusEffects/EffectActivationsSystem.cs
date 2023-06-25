using Content.Shared._ArcheCrawl.StatEffects.Events;
using Content.Shared._ArcheCrawl.StatEffects.Effects;
using Content.Shared.Damage;

namespace Content.Shared._ArcheCrawl.StatEffects
{
    public sealed class EffectActivationSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ActivateEffectEveryUpdateComponent, StatEffectRelayEvent<StatEffectUpdateEvent>>(RelayedActivation);
            SubscribeLocalEvent<ActivateEffectOnTimeoutComponent, StatEffectTimeoutEvent>(TimeoutActivation);
            SubscribeLocalEvent<ActivateEffectOnDamageComponent, StatEffectRelayEvent<DamageChangedEvent>>(HurtActivation);
        }

        private void RelayedActivation<TComp, TEvent>(EntityUid uid, TComp comp, StatEffectRelayEvent<TEvent> args)
        {
            var ev = new StatEffectActivateEvent(args.Victim);
            RaiseLocalEvent(uid, ev);
        }

        private void TimeoutActivation(EntityUid uid, ActivateEffectOnTimeoutComponent comp, StatEffectTimeoutEvent args)
        {
            var ev = new StatEffectActivateEvent(args.Victim);
            RaiseLocalEvent(uid, ev);
        }

        private void HurtActivation(EntityUid uid, ActivateEffectOnDamageComponent comp, StatEffectRelayEvent<DamageChangedEvent> args)
        {
            if (args.Args.Origin == args.Victim || // This should prevent an infinite loop that causes the engine to crash.
                !args.Args.DamageIncreased)
                return;

            var ev = new StatEffectActivateEvent(args.Victim);
            RaiseLocalEvent(uid, ev);
        }
    }
}
