using System.Numerics;
using Content.Client.Gameplay;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Player;
using Robust.Client.State;
using Content.Shared.Mobs;
using Robust.Client.UserInterface;
using Content.Shared.Mobs.Systems;
using Content.Shared._ArcheCrawl.Stats;
using Content.Shared._ArcheCrawl.Stats.Components;
using Content.Shared._ArcheCrawl.PlayerStatus.UI;
using Content.Client.Message;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Damage.Events;

namespace Content.Client._ArcheCrawl.PlayerStatus.UI;

/// <summary>
/// This system basically helps in updating the UI, however it also contains code to add bars to the UI.
/// </summary>
public sealed partial class ACPlayerStatusUISystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly MobThresholdSystem _thresholdSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        // Events to update the bars go here.
        SubscribeLocalEvent<ACPlayerStatusUIComponent, DamageChangedEvent>(FireResetUIEvent);
        SubscribeLocalEvent<ACPlayerStatusUIComponent, StatChangedEvent>(RefFireResetUIEvent);
        // TODO: Events for after stamina changes.

        // Events to add bars go here.
        SubscribeLocalEvent<DamageableComponent, ACGetStatusUIControlsEvent>(AddHealthBar);
        // TODO: Add a stamina bar here. I would do this now but it's literally impossible without it being shit. I love upstream.
    }

    private void FireResetUIEvent<TEvent>(EntityUid uid, ACPlayerStatusUIComponent comp, TEvent args)
    {
        if (_playerManager.LocalPlayer == null
        || _playerManager.LocalPlayer.ControlledEntity != uid)
            return;

        RaiseLocalEvent(new ACUpdateStatusUIEvent());
    }

    private void RefFireResetUIEvent<TEvent>(EntityUid uid, ACPlayerStatusUIComponent comp, ref TEvent args)
    {
        if (_playerManager.LocalPlayer == null
        || _playerManager.LocalPlayer.ControlledEntity != uid)
            return;

        RaiseLocalEvent(new ACUpdateStatusUIEvent());
    }

    /// <summary>
    /// Adds a healthbar if the entity has damageable component and a death threshold.
    /// </summary>
    private void AddHealthBar(EntityUid uid, DamageableComponent comp, ACGetStatusUIControlsEvent args)
    {
        if (_playerManager.LocalPlayer == null
        || _playerManager.LocalPlayer.ControlledEntity != uid)
            return;

        if (!_entityManager.TryGetComponent<DamageableComponent>(uid, out var damageComp)
        || !_thresholdSystem.TryGetDeadThreshold(uid, out var maxHP))
            return;

        var hp = (float) maxHP - (float) damageComp.Damage.Total;

        var bar = new ProgressBar
        {
            ForegroundStyleBoxOverride = new StyleBoxFlat { BackgroundColor = Color.FromHex("#207020") },
            MaxValue = (float) maxHP,
            Value = hp,
            MaxSize = new Vector2(250, 12),
            SetSize = new Vector2(250, 12),
        };

        bar.MaxValue = (float) maxHP;

        var label = new RichTextLabel
        {
            HorizontalAlignment = Control.HAlignment.Center,
            VerticalAlignment = Control.VAlignment.Top, // Having this set to center won't center it... why????
            Margin = new Thickness(-8f), // I need to do this cursed thing to center it. God help me.
        };

        label.SetMarkup(
            Loc.GetString("ac-status-ui-health-value",
            ("value", Math.Ceiling(hp)),
            ("maxValue", Math.Ceiling((float) maxHP))));

        bar.AddChild(label);

        args.Controls.Add((0, bar));
    }
}

/// <summary>
/// Used for saying "Hey! Stuff has changed and I want to update the UI now!"
/// </summary>
public sealed partial class ACUpdateStatusUIEvent : EntityEventArgs { }

/// <summary>
/// Used for putting stuff into the UI. Just add a control!
/// </summary>
public sealed partial class ACGetStatusUIControlsEvent : EntityEventArgs
{
    public List<(int order, Control control)> Controls;

    public ACGetStatusUIControlsEvent(List<(int order, Control control)> controls)
    {
        Controls = controls;
    }
}
