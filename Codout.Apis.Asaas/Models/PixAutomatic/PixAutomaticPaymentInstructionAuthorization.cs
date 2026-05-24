namespace Codout.Apis.Asaas.Models.PixAutomatic;

/// <summary>
/// Subobjeto com referencia minima a autorizacao linkada a uma payment instruction.
/// Aparece dentro de <see cref="PixAutomaticPaymentInstruction.Authorization"/>.
/// </summary>
public class PixAutomaticPaymentInstructionAuthorization
{
    public string Id { get; set; }
    public string EndToEndIdentifier { get; set; }
    public string CustomerId { get; set; }
}
