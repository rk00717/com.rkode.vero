using UnityEngine;

namespace RKode.VERO.Editor {
public class Palette {
    public Color Base { get; private set; }  // darkest bg
    public Color Surface { get; private set; }  // panel fill
    public Color Elevated { get; private set; }  // active tab, hover
    
    // Accents
    public Color Primary { get; private set; }  // title, borders
    public Color PrimaryDim { get; private set; }  // inactive, muted
    public Color Accent { get; private set; }  // section headers, stripe
    public Color CTANormal { get; private set; }  // buttons
    public Color CTA { get; private set; }  // build button
    
    // Text
    public Color TextHigh { get; private set; }  // active tab label, values
    public Color TextMid { get; private set; }  // subtitle, field labels
    public Color TextLow { get; private set; }  // inactive tab

    public Color EnumField { get; private set; }  // inactive tab

    public Palette(Color tint) {
        Rebuild(tint);
    }

    public void Rebuild(Color tint) {
        // Strip saturation down, keep hue — use as base
        float h, s, v;
        Color.RGBToHSV(tint, out h, out s, out v);

        Base = Color.HSVToRGB(h, s * 0.4f, 0.16f);
        Surface = Color.HSVToRGB(h, s * 0.35f, 0.21f);
        Elevated = Color.HSVToRGB(h, s * 0.3f,  0.26f);

        EnumField = Color.HSVToRGB(h, s * 0.35f, 1f);

        Primary = new Color(tint.r, tint.g, tint.b, 1f);
        PrimaryDim = Color.HSVToRGB(h, s * 0.5f, v * 0.6f);

        // Accent: shift hue by 30° for complementary warmth
        Accent = Color.HSVToRGB((h + 0.06f) % 1f, 0.45f, 0.95f);
        CTA = Color.HSVToRGB((h + 0.03f) % 1f, 0.5f, 0.5f);
        CTANormal = Color.HSVToRGB((h + 0.03f) % 1f, 0.25f, 0.25f);

        TextHigh = Color.HSVToRGB(h, 0.08f, 0.95f);
        TextMid = Color.HSVToRGB(h, 0.15f, 0.65f);
        TextLow = Color.HSVToRGB(h, 0.2f,  0.48f);
    }
}
}