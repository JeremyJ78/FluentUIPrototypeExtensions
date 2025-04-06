namespace FluentUI.Components;

public record FluentTileGridItemResizeEventArgs(
    ResizeHandle Orientation,
    RectF Original,
    PointF MousePosition,
    SizeF NewSize,
    SizeF Parent)
{
}
