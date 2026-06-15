#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;

using RKode.Utils.Editor;
using ColorUtility = RKode.Utils.ColorUtility;

namespace RKode.VERO.Editor {
public class Style {
    public Palette Colors { private set; get; }

    public RoundedPanelStyle HeaderPanel { private set; get; }
    public RoundedPanelStyle FieldsPanel { private set; get; }
    public GUIStyle Box { private set; get; }

    public RoundedPanelStyle ActiveTabPanel { private set; get; }
    public RoundedPanelStyle HoverTabPanel { private set; get; }
    public GUIStyle ActiveTab { private set; get; }
    public GUIStyle InactiveTab { private set; get; }

    public GUIStyle TitleLabel { private set; get; }
    public GUIStyle SubtitleLabel { private set; get; }
    public GUIStyle HeadingLabel { private set; get; }
    public GUIStyle FieldLabel { private set; get; }

    public GUIStyle InputField { private set; get; }

    public GUIStyle NormalButton  { private set; get; }
    public GUIStyle LinkButton  { private set; get; }
    public GUIStyle SmallButton  { private set; get; }
    public GUIStyle ResetButton  { private set; get; }
    public GUIStyle CTAButton { private set; get; }
    public GUIStyle CancelButton { private set; get; }
    
    public bool IsInitialized { private set; get; } = false;

    public void Initialize(Color? tint = null) {
        PaletteButtonStyle.ClearCache();
        Colors = new Palette(tint?? ColorUtility.HexToColor(Constant.defaultTint));

        HeaderPanel = new RoundedPanelStyle {
            radius = 10f,
            fillColor = Colors.Base,
            borderColor = ColorUtility.WithAlpha(Colors.Primary, .33f),
            borderWidth = 1.5f
        };

        FieldsPanel = new RoundedPanelStyle {
            radius = 8f,
            fillColor = Colors.Surface,
            borderColor = ColorUtility.WithAlpha(Colors.Primary, .2f),
            borderWidth = 1f
        };

        Box = new GUIStyle(GUIStyle.none) {
            padding = new RectOffset(8, 8, 4, 4),
        };
        Box.normal.background = PaletteButtonStyle.MakeTex(Colors.Base);

        TitleLabel = new GUIStyle(EditorStyles.boldLabel) {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };
        TitleLabel.normal.textColor = Colors.Primary;

        SubtitleLabel = new GUIStyle(EditorStyles.label) {
            fontSize  = 11,
            alignment = TextAnchor.MiddleCenter,
        };
        SubtitleLabel.normal.textColor = Colors.TextMid;

        HeadingLabel = new GUIStyle(EditorStyles.boldLabel) {
            fontSize = 13,
        };
        HeadingLabel.normal.textColor = Colors.Accent;

        FieldLabel = new GUIStyle(EditorStyles.boldLabel) {
            fontSize = 11,
            alignment = TextAnchor.MiddleCenter,
        };
        FieldLabel.normal.textColor = Colors.TextMid;

        // Tab system ----
        ActiveTabPanel = new RoundedPanelStyle {
            radius = 6f,
            fillColor = Colors.Elevated,
            borderColor = ColorUtility.WithAlpha(Colors.PrimaryDim, .6f),
            borderWidth = 1f
        };

        HoverTabPanel = new RoundedPanelStyle {
            radius = 6f,
            fillColor = ColorUtility.HexToColor("#FFFFFF0A"),
            borderColor = ColorUtility.HexToColor("#00000000"),
            borderWidth = 0f
        };

        ActiveTab = new GUIStyle(EditorStyles.label) {
            fontSize  = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        ActiveTab.normal.textColor = Colors.TextHigh;

        InactiveTab = new GUIStyle(EditorStyles.label) {
            fontSize  = 12,
            alignment = TextAnchor.MiddleCenter
        };
        InactiveTab.normal.textColor = Colors.TextLow;

        InputField = new GUIStyle(EditorStyles.textField) {
            fontSize = 11,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(6, 6, 2, 2),
            border = new RectOffset(1, 1, 1, 1),
        };
        InputField.normal.background = PaletteButtonStyle.MakeBorderedTex(Colors.Base, ColorUtility.WithAlpha(Colors.PrimaryDim, 0.35f));
        InputField.focused.background = PaletteButtonStyle.MakeBorderedTex(Colors.Surface, ColorUtility.WithAlpha(Colors.PrimaryDim, 0.8f));
        InputField.hover.background = PaletteButtonStyle.MakeBorderedTex(Colors.Surface, ColorUtility.WithAlpha(Colors.PrimaryDim, 0.5f));
        InputField.normal.textColor = Colors.TextHigh;
        InputField.focused.textColor = Colors.TextHigh;
        InputField.hover.textColor = Colors.TextHigh;

        // Buttons ----
        var buttonStyle = new GUIStyle(GUIStyle.none) {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(4, 0, 2, 2),
            margin = new RectOffset(0, 0, 2, 2),
            border = new RectOffset(1, 1, 1, 1),
        };

        var smallBorder = ColorUtility.WithAlpha(Colors.PrimaryDim, .4f);
        var smallHoverBorder = ColorUtility.WithAlpha(Colors.PrimaryDim, .7f);
        SmallButton = new GUIStyle(buttonStyle);
        SmallButton.normal.background = PaletteButtonStyle.MakeBorderedTex(Colors.Elevated, smallBorder);
        SmallButton.hover.background = PaletteButtonStyle.MakeBorderedTex(
            PaletteButtonStyle.AdjustBrightness(Colors.Elevated, 1.3f), 
            smallHoverBorder
        );
        SmallButton.active.background = PaletteButtonStyle.MakeBorderedTex(
            PaletteButtonStyle.AdjustBrightness(Colors.Elevated, 0.75f), 
            smallHoverBorder
        );
        SmallButton.normal.textColor = Colors.TextHigh;
        SmallButton.hover.textColor = Colors.Primary;
        SmallButton.active.textColor = Colors.Primary;

        var resetBorder = ColorUtility.WithAlpha(Colors.TextLow, .4f);
        var resetHoverBorder = ColorUtility.WithAlpha(Colors.Accent, .7f);
        ResetButton = new GUIStyle(buttonStyle);
        ResetButton.normal.background = PaletteButtonStyle.MakeBorderedTex(Colors.Surface, resetBorder);
        ResetButton.hover.background = PaletteButtonStyle.MakeBorderedTex(PaletteButtonStyle.AdjustBrightness(Colors.Surface, 1.35f), resetHoverBorder);
        ResetButton.active.background = PaletteButtonStyle.MakeBorderedTex(
            PaletteButtonStyle.AdjustBrightness(Colors.Elevated, 0.75f), 
            resetBorder
        );
        ResetButton.normal.textColor = Colors.TextMid;
        ResetButton.hover.textColor = Colors.Accent;
        ResetButton.active.textColor = Colors.Accent;

        CTAButton = PaletteButtonStyle.Create(Colors.CTA, false, 30, 12);
        CTAButton.alignment = TextAnchor.MiddleCenter;
        CTAButton.fontStyle = FontStyle.Bold;

        var cancelColor = ColorUtility.HexToColor("#591E23FF");
        CancelButton = PaletteButtonStyle.Create(cancelColor, false, 30, 12);
        CancelButton.alignment = TextAnchor.MiddleCenter;
        CancelButton.normal.textColor = ColorUtility.HexToColor("#E6A6A8FF");
        CancelButton.hover.textColor = ColorUtility.HexToColor("#FFB8BAFF");

        NormalButton = new GUIStyle(buttonStyle) {
            padding = new RectOffset(14, 10, 10, 10),
        };
        NormalButton.normal.background = PaletteButtonStyle.MakeBorderedTex(Colors.CTANormal, smallBorder);
        NormalButton.hover.background = PaletteButtonStyle.MakeBorderedTex(
            PaletteButtonStyle.AdjustBrightness(Colors.CTANormal, 1.3f), 
            smallHoverBorder
        );
        NormalButton.active.background = PaletteButtonStyle.MakeBorderedTex(
            PaletteButtonStyle.AdjustBrightness(Colors.CTANormal, 0.75f), 
            smallHoverBorder
        );
        NormalButton.normal.textColor = Colors.TextHigh;
        NormalButton.hover.textColor = Colors.Accent;
        NormalButton.active.textColor = Colors.Primary;

        LinkButton = new GUIStyle(buttonStyle) {
            padding = new RectOffset(14, 10, 10, 10),
        };
        LinkButton.normal.textColor = Colors.TextMid;
        LinkButton.hover.textColor = Colors.Primary;
        LinkButton.active.textColor = Colors.Accent;

        IsInitialized = true;
    }
}
}
#endif
