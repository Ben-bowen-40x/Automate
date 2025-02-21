namespace Automate.Translation.CustomerTranslate;

public interface ICustSubLongIdLongNumberStrSellers
{
    long Phone1 { get; set; }
    long Phone2 { get; set; }
    long CustomerID { get; set; }
    long SubId { get; set; }
    bool SubIsActive { get; set; }
    DateTimeOffset CustomerStartDate { get; set; }
    DateTimeOffset CustomerCancelDate { get; set; }
    bool CompletedInitial { get; set; }
    double ContractValue { get; set; }
    DateTimeOffset SubStartDate { get; set; }
    DateTimeOffset SubCancelDate { get; set; }
    string? Sellers { get; set; }
}
