using FluentCx.Comparers;
using FluentCx.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace FluentCx.Components;

public partial class FluentCxTileGrid : FluentComponentBase
{
    private List<FluentCxTileGridItem> _children = [];

    private static readonly Dictionary<ResizeHandle, string> _ltrResize = new(EqualityComparer<ResizeHandle>.Default)
    {
        [ResizeHandle.Horizontally] = "top: 0px; right: 0px; bottom: 0px; width: 9px;",
        [ResizeHandle.Vertically] = "left: 0px; right: 0px; bottom: 0px; height: 9px;",
        [ResizeHandle.Both] = "right: 0px; bottom: 0px; width: 9px; height: 9px;"
    };

    private static readonly Dictionary<ResizeHandle, string> _rtlResize = new(EqualityComparer<ResizeHandle>.Default)
    {
        [ResizeHandle.Horizontally] = "top: 0px; left: 0px; bottom: 0px; width: 9px;",
        [ResizeHandle.Vertically] = "left: 0px; left: 0px; bottom: 0px; height: 9px;",
        [ResizeHandle.Both] = "left: 0px; bottom: 0px; width: 9px; height: 9px;"
    };

    [Inject]
    public GlobalState GlobalState { get; set; } = default!;

    internal Dictionary<ResizeHandle, string> ResizeHandles => GlobalState.Dir == LocalizationDirection.LeftToRight ? _ltrResize : _rtlResize;

    [Parameter]
    public bool CanReorder { get; set; }

    [Parameter]
    public bool CanResize { get; set; }

    /// <summary>
    /// Gets or sets the number of columns in the <see cref="FluentTileGrid"/>.
    /// </summary>
    /// <remarks>
    /// When Columns is set to 0 (or below 0), the grid will use auto-fit to fill the container.
    /// </remarks>
    [Parameter]
    public int Columns { get; set; } = 0;

    /// <summary>
    /// Gets or sets the height of the rows.
    /// </summary>
    [Parameter]
    public string RowHeight { get; set; } = "1fr";

    /// <summary>
    /// Gets or sets the width of the columns.
    /// </summary>
    [Parameter]
    public string ColumnWidth { get; set; } = "1fr";

    [Parameter]
    public int Spacing { get; set; } = 3;

    [Parameter]
    public string? Width { get; set; }

    [Parameter]
    public string? Height { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    internal void Add(FluentCxTileGridItem item)
    {
        _children.Add(item);
        item.Order = _children.Count;
    }

    internal void OnItemParemetersChanged(FluentCxTileGridItem tileGridItem)
    {
        StateHasChanged();
    }

    internal void Remove(FluentCxTileGridItem item)
    {
        _children.Remove(item);
    }

    private string GetParentStyle()
    {
        DefaultInterpolatedStringHandler handler = new();

        if (!string.IsNullOrEmpty(Width))
        {
            handler.AppendLiteral(" width: ");
            handler.AppendFormatted(Width);
            handler.AppendLiteral(";");
        }

        if (!string.IsNullOrEmpty(Height))
        {
            handler.AppendLiteral(" height: ");
            handler.AppendFormatted(Height);
            handler.AppendLiteral(";");
        }

        if(!string.IsNullOrEmpty(Style))
        {
            handler.AppendFormatted(Style);
            handler.AppendLiteral(";");
        }

        return handler.ToStringAndClear();
    }

    private string GetStyle()
    {
        DefaultInterpolatedStringHandler handler = new();

        // Columns
        handler.AppendLiteral("grid-template-columns: repeat(");

        if (Columns > 0)
        {
            handler.AppendFormatted(Columns);
        }
        else
        {
            handler.AppendLiteral("auto-fit");
        }

        handler.AppendLiteral(", minmax(0px, ");
        handler.AppendFormatted(ColumnWidth);
        handler.AppendLiteral("));");

        // Rows
        handler.AppendLiteral(" grid-auto-rows: minmax(0px, ");
        handler.AppendFormatted(RowHeight);
        handler.AppendLiteral(");");

        if (!string.IsNullOrEmpty(Width))
        {
            handler.AppendLiteral(" width: ");
            handler.AppendFormatted(Width);
            handler.AppendLiteral(";");
        }

        if (!string.IsNullOrEmpty(Height))
        {
            handler.AppendLiteral(" height: ");
            handler.AppendFormatted(Height);
            handler.AppendLiteral(";");
        }

        if (!string.IsNullOrEmpty(Style))
        {
            handler.AppendFormatted(Style);
            handler.AppendLiteral(";");
        }

        return handler.ToStringAndClear();
    }

    private void OnDropEnd(FluentDragEventArgs<FluentCxTileGridItem> e)
    {
        if (!string.IsNullOrEmpty(e.Source.Id) &&
            !string.IsNullOrEmpty(e.Target.Id))
        {
            int sourceIndex = _children.FindIndex(x => x.Id == e.Source.Id);
            int destIndex = _children.FindIndex(x => x.Id == e.Target.Id);

            if (sourceIndex >= 0 &&
                destIndex >= 0)
            {
                var firstElement = _children[sourceIndex];
                var lastElement = _children[destIndex];

                (lastElement.Order, firstElement.Order) = (firstElement.Order, lastElement.Order);
                _children.Sort(FluentCxTileGridItemComparer.Default);
                StateHasChanged();
            }
        }
    }
}
