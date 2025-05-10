using FluentCx.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace FluentCx.Components;

public partial class FluentCxTileGridItem
    : FluentComponentBase, IAsyncDisposable
{
    private bool _isRendered;
    private bool _hasParameterChanged;
    private DotNetObjectReference<FluentCxTileGridItem>? _dotNetReference;
    private const string JAVASCRIPT_FILE = "./_content/TileGridPrototype.Components/FluentCxTileGridItem.js";
    private IJSObjectReference? _module;

    [CascadingParameter]
    private FluentCxTileGrid Parent { get; set; } = default!;

    [Parameter]
    public bool IsVisible { get; set; } = true;

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnDoubleClick { get; set; }

    [Parameter]
    public int RowSpan { get; set; } = 1;

    [Parameter]
    public int ColumnSpan { get; set; } = 1;

    [Parameter]
    public string? Header { get; set; }

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    internal RenderFragment ItemRendered { get; }

    private string? InternalStyle => GetInternalStyle();

    internal int Order { get; set; }

    private string GetInternalStyle()
    {
        DefaultInterpolatedStringHandler s = new();

        s.AppendLiteral("display: grid; ");
        s.AppendLiteral($" grid-column-end: span {ColumnSpan}; ");
        s.AppendLiteral($"grid-row-end: span {RowSpan}; ");

        if(!string.IsNullOrEmpty(Style))
        {
            s.AppendFormatted(Style);
            s.AppendLiteral("; ");
        }

        return s.ToStringAndClear();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Parent?.Add(this);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            _isRendered = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _dotNetReference ??= DotNetObjectReference.Create(this);
            _module ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", JAVASCRIPT_FILE);
            await _module.InvokeVoidAsync("initialize", Id, _dotNetReference);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Parent is not null &&
            _isRendered &&
            _hasParameterChanged)
        {
            Parent.OnItemParemetersChanged(this);
        }
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        _hasParameterChanged = parameters.HasValueChanged(nameof(RowSpan), RowSpan) ||
                               parameters.HasValueChanged(nameof(ColumnSpan), ColumnSpan) ||
                               parameters.HasValueChanged(nameof(Header), Header) ||
                               parameters.HasValueChanged(nameof(Class), Class) ||
                               parameters.HasValueChanged(nameof(IsVisible), IsVisible);

        return base.SetParametersAsync(parameters);
    }

    [JSInvokable]
    public async Task Resized(FluentCxTileGridItemResizeEventArgs e)
    {
        if (Parent is not null)
        {
            var columnWidth = e.Parent.Width / Parent.Columns;
            var rowHeight = e.Original.Height / RowSpan;

            void UpdateColumnCount()
            {
                if (e.Original.Width - e.NewSize.Width < 0)
                {
                    var newSpan = e.NewSize.Width / columnWidth;
                    ColumnSpan = (int)Math.Round(newSpan);
                    ColumnSpan = Math.Min(ColumnSpan, Parent.Columns);
                }
                else
                {
                    var newSpan = e.NewSize.Width / columnWidth;
                    ColumnSpan = Math.Max(1, (int)Math.Round(newSpan));
                }
            }

            void UpdateRowCount()
            {
                var newSpan = e.NewSize.Height / rowHeight;

                if (e.Original.Height - e.NewSize.Height < 0)
                {
                    RowSpan = (int)Math.Round(newSpan);
                }
                else
                {
                    RowSpan = Math.Max(1, (int)Math.Round(newSpan));
                }
            }

            switch (e.Orientation)
            {
                case ResizeHandle.Horizontally:
                    {
                        UpdateColumnCount();
                    }
                    break;

                case ResizeHandle.Vertically:
                    {
                        UpdateRowCount();
                    }
                    break;

                case ResizeHandle.Both:
                    {
                        UpdateColumnCount();
                        UpdateRowCount();
                    }
                    break;
            }

            if (_module is not null)
            {
                await _module.InvokeVoidAsync("resized", Id, ColumnSpan, RowSpan);
            }
        }

    }

    public async ValueTask DisposeAsync()
    {
        Parent?.Remove(this);

        if (_module is not null)
        {
            await _module.DisposeAsync();
            _module = null;
        }

        GC.SuppressFinalize(this);
    }
}
