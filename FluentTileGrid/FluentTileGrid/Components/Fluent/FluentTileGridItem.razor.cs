using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using FluentUI.Extensions;

namespace FluentUI.Components;

public partial class FluentTileGridItem
    : FluentComponentBase, IAsyncDisposable
{
    private bool _isRendered;
    private bool _hasParameterChanged;
    private DotNetObjectReference<FluentTileGridItem>? _dotNetReference;
    private const string JAVASCRIPT_FILE = "./FluentTileGridItem.js";
    private IJSObjectReference? _jsModule;

    [CascadingParameter]
    private FluentTileGrid Parent { get; set; } = default!;

    [Parameter]
    public bool IsVisible { get; set; } = true;

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
    private IJSRuntime Module { get; set; } = default!;

    internal RenderFragment ItemRendered { get; }

    private string? InternalStyle => $"display: grid; grid-column-end: span {ColumnSpan}; grid-row-end: span {RowSpan}; {Style}";

    private string? InternalCardStyle => "padding: 0 !important";

    internal int Order { get; set; }

    private string HeaderClass => !Parent.CanReorder && !Parent.CanResize ? string.Empty : "touch-action-none";

    private string TitleId { get; } = Guid.NewGuid().ToString();

    private string PreviewClass => $"{Class} fluent-tile-grid-item-preview";

    private string PreviewStyle => "position: absolute; opacity: 0.4; display: none; padding: 0 !important;";

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
            _jsModule ??= await Module.InvokeAsync<IJSObjectReference>("import", JAVASCRIPT_FILE);
            _dotNetReference = DotNetObjectReference.Create(this);
            await _jsModule.InvokeVoidAsync("initialize", Id, _dotNetReference);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if(Parent is not null &&
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
    public void Resized(FluentTileGridItemResizeEventArgs e)
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

            switch(e.Orientation)
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

            Parent.Refresh();
        }
        
    }

    public ValueTask DisposeAsync()
    {
        Parent?.Remove(this);
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }
}
