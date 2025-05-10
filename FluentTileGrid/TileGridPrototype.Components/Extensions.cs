using FluentCx.Components;
using Microsoft.AspNetCore.Components;

namespace FluentCx.Extensions;

public static class Extensions
{
    public static string ToAttributeValue(this ResizeHandle resizeHandle)
    {
        return resizeHandle switch
        {
            ResizeHandle.Horizontally => "ew",
            ResizeHandle.Vertically => "ns",
            _ => "nwse"
        };
    }

    public static bool HasValueChanged<T>(
        this ParameterView parameterView,
        string parameterName,
        T? value)
    {
        return parameterView.TryGetValue(parameterName, out T? newValue) && EqualityComparer<T?>.Default.Equals(newValue, value);
    }
}
