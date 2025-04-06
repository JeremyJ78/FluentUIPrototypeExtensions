using FluentUI.Comparers;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace FluentUI.Components;

public partial class FluentTileGrid : FluentComponentBase
{
    private List<FluentTileGridItem> _children = [];

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

    [Parameter]
    public string? DataId { get; set; }

    /// <summary>
    /// Gets or sets the number of columns in the <see cref="FluentTileGrid"/>.
    /// </summary>
    [Parameter]
    public int Columns { get; set; }

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

    private string? InternalStyle => GetStyle();

    private string? InternalClass => GetClass();

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    internal void Add(FluentTileGridItem item)
    {
        _children.Add(item);
        item.Order = _children.Count;
    }

    internal void OnItemParemetersChanged(FluentTileGridItem tileGridItem)
    {
        StateHasChanged();
    }

    internal void Remove(FluentTileGridItem item)
    {
        _children.Remove(item);
    }

    private string GetClass()
    {
        DefaultInterpolatedStringHandler handler = new();
        handler.AppendLiteral("fluent-tile-grid");

        if (!string.IsNullOrEmpty(Class))
        {
            handler.AppendLiteral(" ");
            handler.AppendFormatted(Class);
        }

        return handler.ToStringAndClear();
    }

    private string GetStyle()
    {
        DefaultInterpolatedStringHandler handler = new();
        
        // Columns
        handler.AppendLiteral("grid-template-columns: repeat(");
        handler.AppendFormatted(Columns);
        handler.AppendLiteral(", minmax(0px, ");
        handler.AppendFormatted(ColumnWidth);
        handler.AppendLiteral("));");

        // Rows
        handler.AppendLiteral(" grid-auto-rows: minmax(0px, ");
        handler.AppendFormatted(RowHeight);
        handler.AppendLiteral(");");
        handler.AppendLiteral("px; padding: ");
        handler.AppendFormatted(Spacing * 4);
        handler.AppendLiteral("px;");

        if(!string.IsNullOrEmpty(Width))
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

        handler.AppendFormatted(Style);

        return handler.ToStringAndClear();
    }

    private void OnDropEnd(FluentDragEventArgs<FluentTileGridItem> e)
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
                _children.Sort(FluentTileGridItemComparer.Default);
                StateHasChanged();
            }
        }
    }

    internal void Refresh()
    {
        StateHasChanged();
    }
}
