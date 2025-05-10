namespace FluentCx.Components;

public record FluentCxTileGridItemResizeEventArgs(
    ResizeHandle Orientation,
    RectF Original,
    PointF MousePosition,
    SizeF NewSize,
    SizeF Parent)
{
}
