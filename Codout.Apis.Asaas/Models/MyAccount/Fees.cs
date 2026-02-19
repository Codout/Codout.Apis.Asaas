using System;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class Fees
{
    public Payment Payment { get; set; }
    public Transfer Transfer { get; set; }
    public Notification Notification { get; set; }
    public CreditBureauReport CreditBureauReport { get; set; }
    public Invoice Invoice { get; set; }
    public Anticipation Anticipation { get; set; }
}

public class Anticipation
{
    public CreditCard CreditCard { get; set; }
    public BankSlip BankSlip { get; set; }
}

public class BankSlip
{
    public decimal? DefaultValue { get; set; }
    public decimal? DiscountValue { get; set; }
    public string? ExpirationDate { get; set; }
    public int? DaysToReceive { get; set; }
    public decimal? MonthlyFeePercentage { get; set; }
}

public class CreditBureauReport
{
    public decimal? NaturalPersonFeeValue { get; set; }
    public decimal? LegalPersonFeeValue { get; set; }
}

public class CreditCard
{
    public decimal? OperationValue { get; set; }
    public decimal? OneInstallmentPercentage { get; set; }
    public decimal? UpToSixInstallmentsPercentage { get; set; }
    public decimal? UpToTwelveInstallmentsPercentage { get; set; }
    public decimal? UpToTwentyOneInstallmentsPercentage { get; set; }
    public decimal? DiscountOneInstallmentPercentage { get; set; }
    public decimal? DiscountUpToSixInstallmentsPercentage { get; set; }
    public decimal? DiscountUpToTwelveInstallmentsPercentage { get; set; }
    public decimal? DiscountUpToTwentyOneInstallmentsPercentage { get; set; }
    public string? DiscountExpiration { get; set; }
    public int? DaysToReceive { get; set; }
    public decimal? DetachedMonthlyFeeValue { get; set; }
    public decimal? InstallmentMonthlyFeeValue { get; set; }
}

public class DebitCard
{
    public decimal? OperationValue { get; set; }
    public decimal? DefaultPercentage { get; set; }
    public int? DaysToReceive { get; set; }
}

public class Invoice
{
    public decimal? FeeValue { get; set; }
}

public class Notification
{
    public decimal? PhoneCallFeeValue { get; set; }
    public decimal? WhatsAppFeeValue { get; set; }
    public decimal? MessagingFeeValue { get; set; }
}

public class Payment
{
    public BankSlip BankSlip { get; set; }
    public CreditCard CreditCard { get; set; }
    public DebitCard DebitCard { get; set; }
    public Pix Pix { get; set; }
}

public class Pix
{
    public decimal? FixedFeeValue { get; set; }
    public decimal? FixedFeeValueWithDiscount { get; set; }
    public decimal? PercentageFee { get; set; }
    public decimal? MinimumFeeValue { get; set; }
    public decimal? MaximumFeeValue { get; set; }
    public decimal? MonthlyCreditsWithoutFee { get; set; }
    public decimal? CreditsReceivedOfCurrentMonth { get; set; }
    public decimal? FeeValue { get; set; }
    public decimal? DiscountValue { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool ConsideredInMonthlyTransfersWithoutFee { get; set; }
}

public class Ted
{
    public decimal? FeeValue { get; set; }
    public bool ConsideredInMonthlyTransfersWithoutFee { get; set; }
}

public class Transfer
{
    public int? MonthlyTransfersWithoutFee { get; set; }
    public Ted Ted { get; set; }
    public Pix Pix { get; set; }
}

