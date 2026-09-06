namespace NexaUI.Core;

/// <summary>
/// Provides DPI-aware scaling utilities for NexaUI consumers.
/// All measurements stored in NexaUI are expressed in DIPs (1 DIP = 1/96 inch).
/// </summary>
public static class NexaDpi
{
    public const float BaseDpi = 96F;

    public static float ScaleFactor(float dpi)
    {
        if (dpi <= 0F) return 1F;
        return dpi / BaseDpi;
    }

    public static int Scale(int dips, float dpi)
    {
        if (dips <= 0) return 0;
        return (int)Math.Round(dips * ScaleFactor(dpi));
    }

    public static int Scale(int dips, int dpi) => Scale(dips, (float)dpi);

    public static float ScaleF(float dips, float dpi)
    {
        if (dips <= 0F) return 0F;
        return dips * ScaleFactor(dpi);
    }
}