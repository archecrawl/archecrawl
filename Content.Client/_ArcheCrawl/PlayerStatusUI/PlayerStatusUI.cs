using System.Numerics;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._ArcheCrawl.PlayerStatus.UI;

/// <summary>
/// The code that manages the UI.
/// </summary>
public sealed partial class ACPlayerStatusUI : UIWidget
{
    #region Variables

    public RichTextLabel EntNameText;

    private readonly PanelContainer _panel;
    private readonly BoxContainer _barContainer;

    #endregion

    #region UI

    public ACPlayerStatusUI()
    {
        Margin = new Thickness(10); // The panel touching the edge of the screen looks... meh.
        VerticalAlignment = VAlignment.Top;
        Orientation = LayoutOrientation.Vertical;

        // The background.
        _panel = new PanelContainer
        {
            VerticalAlignment = VAlignment.Top,
            StyleClasses = { "tooltipBox" }
        };

        // The actual box storing stuff.
        var box = new BoxContainer
        {
            Margin = new Thickness(2),
            Orientation = LayoutOrientation.Vertical,
        };

        // The entity label and it's background.
        var stripeBack = new StripeBack { };

        EntNameText = new RichTextLabel
        {
            HorizontalAlignment = HAlignment.Center,
        };

        // The box that stores bars (or other stuff.)

        _barContainer = new BoxContainer()
        {
            SeparationOverride = 2,
            Orientation = LayoutOrientation.Vertical,
            Margin = new Thickness(2),
        };

        // And some space so it looks a bit nicer.

        var footer = new VSeparator
        {
            MinHeight = SetHeight = 2
        };

        // Now add all of these together and you get some UI.

        stripeBack.AddChild(EntNameText);

        box.AddChild(stripeBack);
        box.AddChild(_barContainer);
        box.AddChild(footer);

        _panel.AddChild(box);

        AddChild(_panel);
    }

    #endregion

    #region Functions
    public void ResetUI()
    {
        _barContainer.DisposeAllChildren();
    }

    public void AddControl(Control bar, bool useDefaults)
    {
        if (useDefaults)
        {
            bar.SetSize = bar.MaxSize = new(250, 12); // Should have a min size just so it doesn't look shit without bars.
            bar.HorizontalAlignment = HAlignment.Center;
            bar.VerticalAlignment = VAlignment.Top;
        }

        _barContainer.AddChild(bar);
    }

    public void RescaleUI()
    {
        _panel.Measure(Vector2Helpers.Infinity);
        _panel.SetSize = _panel.DesiredSize;
    }

    #endregion
}
