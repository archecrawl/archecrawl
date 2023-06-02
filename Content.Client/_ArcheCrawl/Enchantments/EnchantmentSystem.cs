using System.Linq;
using Content.Client.Items.Systems;
using Content.Shared._ArcheCrawl.Enchantments;
using Content.Shared._ArcheCrawl.Enchantments.Components;
using Content.Shared.Hands;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameStates;

namespace Content.Client._ArcheCrawl.Enchantments;

/// <inheritdoc/>
public sealed class EnchantmentSystem : SharedEnchantmentSystem
{
    [Dependency] private readonly ItemSystem _item = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EnchantableComponent, ComponentHandleState>(OnHandleState);
        SubscribeLocalEvent<EnchantableComponent, AppearanceChangeEvent>(OnAppearanceChange);

        SubscribeLocalEvent<EnchantableComponent, GetInhandVisualsEvent>(RelayEvent, after: new []{typeof(ItemSystem)});
        SubscribeLocalEvent<EnchantmentInhandVisualsComponent, GetInhandVisualsEvent>(OnGetInhandVisuals, after: new []{typeof(ItemSystem)});
    }

    private void OnHandleState(EntityUid uid, EnchantableComponent component, ref ComponentHandleState args)
    {
        if (args.Current is not EnchantableComponentState)
            return;

        if (TryComp<AppearanceComponent>(uid, out var appearanceComponent))
            _appearance.MarkDirty(appearanceComponent);
    }

    private void OnAppearanceChange(EntityUid uid, EnchantableComponent component, ref AppearanceChangeEvent args)
    {
        _item.VisualsChanged(uid);
        UpdateIconVisuals(uid, component);
    }

    private void UpdateIconVisuals(EntityUid uid, EnchantableComponent? component = null, SpriteComponent? sprite = null)
    {
        if (!Resolve(uid, ref component, ref sprite))
            return;

        foreach (var ent in component.EnchantmentContainer.ContainedEntities)
        {
            if (!TryComp<EnchantmentVisualsComponent>(ent, out var visuals))
                continue;

            foreach (var added in visuals.SpriteLayers)
            {
                sprite.RemoveLayer(added);
            }

            visuals.SpriteLayers.Clear();

            if (!visuals.IconVisuals.Any())
            {
                var map = $"{ent.ToString()}";
                var layer = sprite.AddLayer(new RSI.StateId(visuals.Key));
                sprite.LayerMapSet(map, layer);
                visuals.SpriteLayers.Add(map);
                continue;
            }

            var i = 0;
            foreach (var layerDatum in visuals.IconVisuals)
            {
                var map = $"{ent.ToString()}-{i}";
                layerDatum.State ??= visuals.Key;
                var layer = sprite.AddLayer(layerDatum);
                sprite.LayerMapSet(map, layer);
                visuals.SpriteLayers.Add(map);
                i++;
            }
        }
    }

    private void OnGetInhandVisuals(EntityUid uid, EnchantmentInhandVisualsComponent component, GetInhandVisualsEvent args)
    {
        var state = $"inhand-{args.Location.ToString().ToLowerInvariant()}-{component.Key}";

        if (!component.InhandVisuals.Any() || !component.InhandVisuals.TryGetValue(args.Location, out var layers))
        {
            var data = new PrototypeLayerData
            {
                State = state
            };
            args.Layers.Add(($"enchantment-{uid.ToString()}-{component.Key}", data));
            return;
        }

        var i = 0;
        foreach (var layer in layers)
        {
            layer.State ??= state;
            args.Layers.Add(($"enchantment-{uid.ToString()}-{component.Key}-{i}", layer));
            i++;
        }
    }
}
