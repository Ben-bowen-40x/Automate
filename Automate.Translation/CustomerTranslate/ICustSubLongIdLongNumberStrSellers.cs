using Automate.Translation.MessageTranslate;

namespace Automate.Translation.CustomerTranslate;

public interface ICustSubLongIdLongNumberStrSellers : IMsgDTONumberLong
{
    long CustomerID { get; set; }
    long SubId { get; set; }
    bool SubIsActive { get; set; }
    DateTime CustomerStartDate { get; set; }
    DateTime CustomerCancelDate { get; set; }
    bool CompletedInitial { get; set; }
    double ContractValue { get; set; }
    DateTime SubStartDate { get; set; }
    DateTime SubCancelDate { get; set; }
    string? Sellers { get; set; }
}
