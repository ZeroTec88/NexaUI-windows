using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Centralized typography definition for a theme. Provides strongly typed access to every
/// <see cref="NexaTypographyRole"/> used in the NexaUI design system.
/// </summary>
public sealed class NexaTypography
{
    private readonly Dictionary<NexaTypographyRole, NexaTypographyStyle> _styles;

    public NexaTypography(
        NexaTypographyStyle body,
        NexaTypographyStyle bodyStrong,
        NexaTypographyStyle caption,
        NexaTypographyStyle label,
        NexaTypographyStyle button,
        NexaTypographyStyle heading,
        NexaTypographyStyle title,
        NexaTypographyStyle display)
    {
        _styles = new Dictionary<NexaTypographyRole, NexaTypographyStyle>
        {
            [NexaTypographyRole.Body] = body,
            [NexaTypographyRole.BodyStrong] = bodyStrong,
            [NexaTypographyRole.Caption] = caption,
            [NexaTypographyRole.Label] = label,
            [NexaTypographyRole.Button] = button,
            [NexaTypographyRole.Heading] = heading,
            [NexaTypographyRole.Title] = title,
            [NexaTypographyRole.Display] = display
        };
    }

    public NexaTypographyStyle this[NexaTypographyRole role] => _styles[role];

    public NexaTypographyStyle Get(NexaTypographyRole role) => _styles[role];

    public Font ToFont(NexaTypographyRole role, float dpi) => _styles[role].ToFont(dpi);
}