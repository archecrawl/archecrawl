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
using Content.Shared._ArcheCrawl.PlayerStatus.UI;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Client.Message;
using Robust.Client.UserInterface;
using System.Collections.Immutable;
using System.Linq;
using Content.Client.UserInterface.Systems.Gameplay;

namespace Content.Client._ArcheCrawl.PlayerStatus.UI;

public sealed partial class ACHealthUIController : UIController, IOnStateEntered<GameplayState>
{
    private ACPlayerStatusUI? Gui => UIManager.GetActiveUIWidgetOrNull<ACPlayerStatusUI>();
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntitySystemManager _systemManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ACUpdateStatusUIEvent>(UpdateEvent);
        SubscribeLocalEvent<PlayerAttachedEvent>(PlayerAttach);
        SubscribeLocalEvent<PlayerDetachedEvent>(PlayerDetach);

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
    }

    private void PlayerAttach(PlayerAttachedEvent args)
    {
        UpdateUI(args.Entity);
    }

    private void PlayerDetach(PlayerDetachedEvent args)
    {
        UpdateUI(args.Entity);
    }

    private void OnScreenLoad() // For switching between standard / seperated
    {
        if (_playerManager.LocalPlayer == null)
            return;

        UpdateUI(_playerManager.LocalPlayer.ControlledEntity);
    }

    private void UpdateEvent(ACUpdateStatusUIEvent args) // Used for general updating.
    {
        if (_playerManager.LocalPlayer == null)
            return;

        UpdateUI(_playerManager.LocalPlayer.ControlledEntity);
    }

    public void OnStateEntered(GameplayState state) // Loading into the game (I lost the game.)
    {
        if (_playerManager.LocalPlayer == null)
            return;

        UpdateUI(_playerManager.LocalPlayer.ControlledEntity);
    }

    private void UpdateUI(EntityUid? uid)
    {
        if (Gui == null || uid == null)
            return;

        UpdateUI(uid.Value);
    }

    /// <summary>
    /// Should be called whenever the UI needs to be updated.
    /// </summary>
    /// <param name="uid"></param>
    private void UpdateUI(EntityUid uid)
    {
        if (Gui == null)
            return;

        if (!_entityManager.HasComponent<ACPlayerStatusUIComponent>(uid)) // No component? Likely shouldn't have the UI.
        {
            Gui.Visible = false;
            return;
        }

        Gui.ResetUI(); // Clears all the previous bars.

        Gui.Visible = true; // If it got this far, it should be visible, right?

        Gui.EntNameText.SetMarkup(Loc.GetString("ac-status-ui-entity-name", ("entity", uid)));

        /// <summary>
        /// Data used to add controls to the UI.
        /// </summary>
        /// <param name="priority">The lower the priority, the higher up the element should be.</param>
        /// <param name="control">The element being applied.</param>
        /// <param name="useDefaults">Should it use the "default" settings? This should be used for progress bars.</param>
        var controlData = new List<(int priority, Control control, bool useDefaults)>();

        _entityManager.EventBus.RaiseLocalEvent(uid, new ACGetStatusUIControlsEvent(controlData));

        var orderedData = controlData.OrderBy(i => i.priority);

        foreach (var (_, control, useDefaults) in orderedData)
        {
            Gui.AddControl(control, useDefaults);
        }

        Gui.RescaleUI();
    }
}
