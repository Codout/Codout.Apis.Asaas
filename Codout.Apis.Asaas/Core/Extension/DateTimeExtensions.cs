using System;

namespace Codout.Apis.Asaas.Core.Extension;

internal static class DateTimeExtensions
{
    public static string ToApiRequest(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd");
    }
}