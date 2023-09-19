using System.Numerics;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._ArcheCrawl.PlayerStatus.UI;

/// <summary>
/// The code that manages the UI.
/// </summary>
public sealed partial class ACPlayerStatusUI : UIWidget
{
    #region Variables

    public RichTextLabel EntNameText;
    public BoxContainer ControlContainer;

    #endregion

    #region UI

    public ACPlayerStatusUI()
    {
        Name = "ArcheCrawlStatusUI";
        HorizontalAlignment = HAlignment.Center;
        VerticalAlignment = VAlignment.Top;
        MinSize = new Vector2(256, 256);
        Orientation = LayoutOrientation.Vertical;
        Margin = new Thickness(10);

        // The background.
        var panel = new PanelContainer
        {
            HorizontalAlignment = HAlignment.Stretch,
            VerticalAlignment = VAlignment.Stretch,
            StyleClasses = { "tooltipBox" },
        };

        // The actual box storing stuff.
        var box = new BoxContainer
        {
            Margin = new Thickness(2),
            Orientation = LayoutOrientation.Vertical,
        };

        // The entity label and it's background.
        var stripeBack = new StripeBack
        {
            MaxSize = new Vector2(275, 1000),
            HorizontalExpand = true,
            VerticalExpand = true,
        };

        EntNameText = new RichTextLabel
        {
            HorizontalAlignment = HAlignment.Center,
        };

        // The box that stores bars (or other stuff.)

        ControlContainer = new BoxContainer()
        {
            SeparationOverride = 2,
            Orientation = LayoutOrientation.Vertical,
            Margin = new Thickness(2),
        };

        // And some space so it looks a bit nicer.

        var footer = new VSeparator
        {
            MinHeight = SetHeight = 2,
            Visible = false,
            ReservesSpace = true,
        };

        // Now add all of these together and you get some UI.

        stripeBack.AddChild(EntNameText);

        box.AddChild(stripeBack);
        box.AddChild(ControlContainer);
        box.AddChild(footer);
        panel.AddChild(box);

        AddChild(panel);
    }

    #endregion

    #region Functions
    public void ResetUI()
    {
        ControlContainer.DisposeAllChildren();
    }

    #endregion
}
