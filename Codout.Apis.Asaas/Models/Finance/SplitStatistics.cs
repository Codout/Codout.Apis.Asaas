namespace Codout.Apis.Asaas.Models.Finance;

/// <summary>
/// Schema oficial: {income: number, value: number}. Antes o modelo tinha
/// {TotalPendingValue, TotalReceivedValue} (inventados — nao existem
/// na FinanceGetSplitStatisticsResponseDTO).
/// </summary>
public class SplitStatistics
{
    /// <summary>Valores a receber.</summary>
    public decimal Income { get; set; }

    /// <summary>Valores a enviar.</summary>
    public decimal Value { get; set; }
}
