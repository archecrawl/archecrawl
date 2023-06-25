using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared._ArcheCrawl.StatEffects.Events;
using Content.Shared._ArcheCrawl.StatEffects;
using Content.Shared._ArcheCrawl.StatEffects.Effects;
using Robust.Shared.Timing;
using Content.Shared.Weapons.Melee.Events;

// For effects that do spawn/remove stuff.

namespace Content.Server._ArcheCrawl.StatEffects.Effects
{
    public sealed class EffectsSystem : EntitySystem
    {
        [Dependency] private readonly IGameTiming _gameTiming = default!;
        [Dependency] private readonly SharedStatEffectsSystem _sharedStatEffects = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<StatEffectComponent, StatEffectRelayEvent<StatEffectUpdateEvent>>(EffectUpdate);
            SubscribeLocalEvent<AdjustEffectStrengthEffectComponent, StatEffectActivateEvent>(AdjustStrengthEffect);
        }

        private void EffectUpdate(EntityUid uid, StatEffectComponent comp, StatEffectRelayEvent<StatEffectUpdateEvent> args)
        {
            if (!comp.IsTimed)
                return;

            var curTime = _gameTiming.CurTime;

            if (curTime > comp.Length)
            {
                RaiseLocalEvent(uid, new StatEffectTimeoutEvent(args.Victim));
                QueueDel(uid);
            }
        }



        private void AdjustStrengthEffect(EntityUid uid, AdjustEffectStrengthEffectComponent comp, StatEffectActivateEvent args)
        {
            if (!TryComp<StatEffectComponent>(uid, out var effectComp))
                return;

            var newStrength = (int) (effectComp.OverallStrength * comp.Multipler + comp.AddedOn);

            _sharedStatEffects.ModifyEffect(uid, newStrength, overrideEffect: true, comp: effectComp);
        }
    }
}
