using Content.Shared._ArcheCrawl.StatEffects.Components;
using Content.Shared._ArcheCrawl.StatEffects.Components.Effects.Inflictors;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Server._ArcheCrawl.StatEffects
{
    public sealed partial class StatEffectsSystem
    {
        public void InitializeInflictor()
        {
            SubscribeLocalEvent<InflictEffectOnDamageComponent, MeleeHitEvent>(InflictOnHit);
        }

        private void InflictOnHit(EntityUid uid, InflictEffectOnDamageComponent comp, MeleeHitEvent args)
        {
            foreach (var entity in args.HitEntities)
            {
                if (!TryComp<StatEffectsComponent>(entity, out var statEffects))
                    continue;

                ApplyEffect(entity, comp.Effect, comp.Strength, TimeSpan.FromSeconds(comp.Length), comp.AddOn, comp.Replace, statEffects);
            }
        }
    }
}
