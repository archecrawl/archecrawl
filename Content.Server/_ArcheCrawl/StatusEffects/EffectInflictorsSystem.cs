using Content.Shared._ArcheCrawl.StatEffects;
using Content.Shared._ArcheCrawl.StatEffects.Effects;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Server._ArcheCrawl.StatEffects
{
    public sealed class EffectInflictorsSystem : EntitySystem
    {
        [Dependency] private readonly SharedStatEffectsSystem _statEffectsSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<InflictEffectOnDamageComponent, MeleeHitEvent>(InflictOnHit);
        }

        private void InflictOnHit(EntityUid uid, InflictEffectOnDamageComponent comp, MeleeHitEvent args)
        {
            foreach (var entity in args.HitEntities)
            {
                if (!TryComp<StatEffectsComponent>(entity, out var statEffects))
                    continue;

                _statEffectsSystem.ApplyEffect(entity, comp.Effect, comp.Strength, TimeSpan.FromSeconds(comp.Length), comp.AddOn, comp.Replace, statEffects);
            }
        }
    }
}
