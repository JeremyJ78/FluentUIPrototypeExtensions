using FluentUI.Components;

namespace FluentUI.Comparers;

internal sealed class FluentTileGridItemComparer
    : IComparer<FluentTileGridItem>
{
    public static FluentTileGridItemComparer Default { get; } = new();

    public int Compare(FluentTileGridItem? x, FluentTileGridItem? y)
    {
        if (x is null)
        {
            return y is null ? 0 : 1;
        }

        if (y is null)
        {
            return x is null ? 0 : -1;
        }

        return x.Order.CompareTo(y.Order);
    }
}
