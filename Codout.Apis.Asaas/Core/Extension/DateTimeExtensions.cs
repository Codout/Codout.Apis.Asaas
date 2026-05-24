using System;
using System.Globalization;

namespace Codout.Apis.Asaas.Core.Extension;

internal static class DateTimeExtensions
{
    /// <summary>
    /// Formata uma data no formato ISO YYYY-MM-DD usado pela API Asaas em
    /// query params (filtros como paymentDate, dueDate, startDate).
    /// Usa InvariantCulture explicitamente para garantir formato consistente
    /// mesmo em ambientes pt-BR, es-ES, etc.
    /// </summary>
    public static string ToApiRequest(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}