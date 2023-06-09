﻿using System.Linq;
using Content.Shared._ArcheCrawl.Crits.Components;
using Content.Shared._ArcheCrawl.Enchantments.Components;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Prototypes;
using Content.Shared.Tag;
using Content.Shared.Weapons.Melee.Events;
using JetBrains.Annotations;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._ArcheCrawl.Enchantments;

/// <summary>
/// This handles logic and relates relating to
/// <see cref="EnchantableComponent"/> and <see cref="EnchantmentComponent"/>
/// </summary>
public abstract partial class SharedEnchantmentSystem : EntitySystem
{
    [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly TagSystem _tag = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<EnchantableComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<EnchantableComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<EnchantableComponent, ExaminedEvent>(OnExamined);

        SubscribeLocalEvent<RandomEnchantmentsComponent, MapInitEvent>(OnRandomEnchantmentsMapInit);

        SubscribeLocalEvent<EnchantableComponent, MeleeHitEvent>(RelayEvent);
        SubscribeLocalEvent<EnchantableComponent, GetMeleeDamageEvent>(RelayEventByRef);
        SubscribeLocalEvent<EnchantableComponent, GetCritChanceEvent>(RelayEvent);
        SubscribeLocalEvent<EnchantableComponent, InventoryRelayedEvent<GetCritChanceEvent>>(RelayEvent);
        SubscribeLocalEvent<EnchantableComponent, GetCritDamageEvent>(RelayEvent);
        SubscribeLocalEvent<EnchantableComponent, InventoryRelayedEvent<GetCritDamageEvent>>(RelayEvent);
    }

    private void OnMapInit(EntityUid uid, EnchantableComponent component, MapInitEvent args)
    {
        component.OriginalName = Name(uid);
    }

    private void OnStartup(EntityUid uid, EnchantableComponent component, ComponentStartup args)
    {
        component.EnchantmentContainer = _container.EnsureContainer<Container>(uid, component.EnchantmentContainerId);
        component.EnchantmentContainer.OccludesLight = false;
    }

    private void OnExamined(EntityUid uid, EnchantableComponent component, ExaminedEvent args)
    {
        var allDescriptions = new Dictionary<string, int>();
        foreach (var ent in component.AllEnchantments)
        {
            if (!TryComp<EnchantmentComponent>(ent, out var enchantment))
                continue;

            foreach (var desc in enchantment.Descriptions)
            {
                allDescriptions[desc] = allDescriptions.GetValueOrDefault(desc) + 1;
            }
        }

        foreach (var (desc, amount) in allDescriptions)
        {
            // i'm hardcoding this because idgaf
            var color = amount switch
            {
                2 => Color.LimeGreen,
                3 => Color.CornflowerBlue,
                4 => Color.Violet,
                _ => Color.Gold
            };

            var fullString = Loc.GetString("enchantment-description-wrapper",
                    ("description", Loc.GetString(desc)),
                    ("color", color),
                    ("amount", amount));
            args.PushMarkup(fullString);
        }

        RelayEvent(uid, component, args);
    }

    [PublicAPI]
    public bool TryApplyEnchantment(EntityUid uid, string enchantmentId, EnchantableComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;

        if (!CanApplyEnchantment(uid, enchantmentId, component))
            return false;

        ApplyEnchantment(uid, enchantmentId, component);
        return true;
    }

    public void ApplyEnchantment(EntityUid uid, string enchantmentId, EnchantableComponent? component = null)
    {
        if (!Resolve(uid, ref component) || component.EnchantmentContainer == null)
            return;

        if (!PrototypeManager.TryIndex<EntityPrototype>(enchantmentId, out var prototype) || !prototype.HasComponent<EnchantmentComponent>())
        {
            Log.Error($"Attmped to apply invalid enchantment prototype: {enchantmentId}");
            return;
        }

        var enchantmentUid = Spawn(enchantmentId, Transform(uid).Coordinates);
        var enchantment = Comp<EnchantmentComponent>(enchantmentUid);

        enchantment.AppliedEntity = uid;
        component.EnchantmentContainer.Insert(enchantmentUid);

        var ev = new EnchantmentAppliedEvent(uid, enchantmentUid);
        RaiseLocalEvent(uid, ref ev, true);

        UpdateEnchantableName(uid, component);
        Dirty(component);
    }

    public bool CanApplyEnchantment(EntityUid uid, string enchantmentId, EnchantableComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;

        if (!PrototypeManager.TryIndex<EntityPrototype>(enchantmentId, out var prototype))
            return false;

        return CanApplyEnchantment(uid, prototype, component);
    }

    public bool CanApplyEnchantment(EntityUid uid, EntityPrototype prototype, EnchantableComponent? component = null, EnchantmentComponent? enchantment = null)
    {
        if (!Resolve(uid, ref component))
            return false;

        if (enchantment == null && !prototype.TryGetComponent(out enchantment, EntityManager.ComponentFactory))
            return false;

        foreach (var entity in component.AllEnchantments)
        {
            if (Prototype(entity)?.ID is { } id && enchantment.ExclusiveEnchantments.Contains(id))
                return false;
        }

        return _tag.HasAnyTag(uid, enchantment.ValidWeaponClasses);
    }

    private void UpdateEnchantableName(EntityUid uid, EnchantableComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        var prefixList = new List<string>();
        var suffixList = new List<string>();
        foreach (var entity in component.AllEnchantments)
        {
            if (!TryComp<EnchantmentComponent>(entity, out var enchantComp))
                continue;

            foreach (var (placement, descriptors) in enchantComp.NameModifiers)
            {
                switch (placement)
                {
                    case DescriptorPlacement.Prefix:
                        prefixList.AddRange(descriptors.Select(Loc.GetString));
                        break;
                    case DescriptorPlacement.Suffix:
                        suffixList.AddRange(descriptors.Select(Loc.GetString));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        prefixList = prefixList.Distinct().ToList();
        suffixList = suffixList.Distinct().ToList();
        _metaData.SetEntityName(uid, $"{string.Join(" ", prefixList)} {component.OriginalName} {string.Join(" ", suffixList)}".Trim());
    }

    #region Relays
    [PublicAPI]
    protected void RelayEvent(EntityUid uid, EnchantableComponent component, object args)
    {
        if (component.EnchantmentContainer == null)
            return;

        foreach (var enchantment in component.EnchantmentContainer.ContainedEntities)
        {
            RaiseLocalEvent(enchantment, args);
        }
    }

    [PublicAPI]
    protected void RelayEventByRef<T>(EntityUid uid, EnchantableComponent component, ref T args) where T : notnull
    {
        if (component.EnchantmentContainer == null)
            return;

        foreach (var enchantment in component.EnchantmentContainer.ContainedEntities)
        {
            RaiseLocalEvent(enchantment, ref args);
        }
    }
    #endregion
}
