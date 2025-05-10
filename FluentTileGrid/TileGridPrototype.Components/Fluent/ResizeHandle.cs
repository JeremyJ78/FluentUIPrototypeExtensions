using System.ComponentModel;

namespace FluentCx.Components;

public enum ResizeHandle
{
    [Description("ew")]
    Horizontally,

    [Description("ns")]
    Vertically,

    [Description("nwse")]
    Both
}
