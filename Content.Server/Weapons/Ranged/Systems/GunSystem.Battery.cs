using Content.Server.Power.Components;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Projectiles;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    protected override void InitializeBattery()
    {
        base.InitializeBattery();

        // Hitscan
        SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ComponentStartup>(OnBatteryStartup);
        SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ChargeChangedEvent>(OnBatteryChargeChange);
        SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GetVerbsEvent<ExamineVerb>>(OnBatteryExaminableVerb);

        // Projectile
        SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ComponentStartup>(OnBatteryStartup);
        SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ChargeChangedEvent>(OnBatteryChargeChange);
        SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GetVerbsEvent<ExamineVerb>>(OnBatteryExaminableVerb);
    }

    private void OnBatteryStartup(EntityUid uid, BatteryAmmoProviderComponent component, ComponentStartup args)
    {
        UpdateShots(uid, component);
    }

    private void OnBatteryChargeChange(EntityUid uid, BatteryAmmoProviderComponent component, ref ChargeChangedEvent args)
    {
        UpdateShots(uid, component, args.Charge, args.MaxCharge);
    }

    private void UpdateShots(EntityUid uid, BatteryAmmoProviderComponent component)
    {
        if (!TryComp<BatteryComponent>(uid, out var battery))
            return;

        UpdateShots(uid, component, battery.Charge, battery.MaxCharge);
    }

    private void UpdateShots(EntityUid uid, BatteryAmmoProviderComponent component, float charge, float maxCharge)
    {
        var shots = (int) (charge / component.FireCost);
        var maxShots = (int) (maxCharge / component.FireCost);

        if (component.Shots != shots || component.Capacity != maxShots)
        {
            Dirty(component);
        }

        component.Shots = shots;
        component.Capacity = maxShots;
        UpdateBatteryAppearance(uid, component);
    }

    private void OnBatteryExaminableVerb(EntityUid uid, BatteryAmmoProviderComponent component, GetVerbsEvent<ExamineVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var damageSpec = GetDamage(component);

        if (damageSpec == null)
            return;

        string damageType;

        switch (component)
        {
            case HitscanBatteryAmmoProviderComponent:
                damageType = Loc.GetString("damage-hitscan");
                break;
            case ProjectileBatteryAmmoProviderComponent:
                damageType = Loc.GetString("damage-projectile");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var verb = new ExamineVerb()
        {
            Act = () =>
            {
                var markup = Damageable.GetDamageExamine(damageSpec, damageType);
                Examine.SendExamineTooltip(args.User, uid, markup, false, false);
            },
            Text = Loc.GetString("damage-examinable-verb-text"),
            Message = Loc.GetString("damage-examinable-verb-message"),
            Category = VerbCategory.Examine,
            Icon = new SpriteSpecifier.Texture(new ("/Textures/Interface/VerbIcons/smite.svg.192dpi.png")),
        };

        args.Verbs.Add(verb);
    }

    private DamageSpecifier? GetDamage(BatteryAmmoProviderComponent component)
    {
        if (component is ProjectileBatteryAmmoProviderComponent battery)
        {
            if (ProtoManager.Index<EntityPrototype>(battery.Prototype).Components
                .TryGetValue(_factory.GetComponentName(typeof(ProjectileComponent)), out var projectile))
            {
                var p = (ProjectileComponent) projectile.Component;

                if (p.Damage.Total > FixedPoint2.Zero)
                {
                    return p.Damage;
                }
            }

            return null;
        }

        if (component is HitscanBatteryAmmoProviderComponent hitscan)
        {
            return ProtoManager.Index<HitscanPrototype>(hitscan.Prototype).Damage;
        }

        return null;
    }

    protected override void TakeCharge(EntityUid uid, BatteryAmmoProviderComponent component)
    {
        // Will raise ChargeChangedEvent
        _battery.UseCharge(uid, component.FireCost);
    }
}
