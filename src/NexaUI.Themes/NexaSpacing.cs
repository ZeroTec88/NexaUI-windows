using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Defines the spacing scale used across NexaUI. All values are in DIPs.
/// </summary>
public sealed class NexaSpacing
{
    private readonly Dictionary<NexaSpacingToken, int> _values;

    public NexaSpacing(
        int none, int xxs, int xs, int sm, int md, int lg, int xl, int xxl, int huge)
    {
        _values = new Dictionary<NexaSpacingToken, int>
        {
            [NexaSpacingToken.None] = none,
            [NexaSpacingToken.Xxs] = xxs,
            [NexaSpacingToken.Xs] = xs,
            [NexaSpacingToken.Sm] = sm,
            [NexaSpacingToken.Md] = md,
            [NexaSpacingToken.Lg] = lg,
            [NexaSpacingToken.Xl] = xl,
            [NexaSpacingToken.Xxl] = xxl,
            [NexaSpacingToken.Huge] = huge
        };
    }

    public int this[NexaSpacingToken token] => _values[token];

    public int Get(NexaSpacingToken token) => _values[token];

    public int ToPixels(NexaSpacingToken token, float dpi) => NexaDpi.Scale(_values[token], dpi);
}