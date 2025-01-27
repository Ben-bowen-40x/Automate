using Automate.Translation.MessageTranslate;

namespace Automate.Translation.QualifiedMessageTranslate;

public interface IQualifiedMessageTranslate : IMsgDTONumberLong
{
    bool ImLead { get; set; }
    bool SalesLead { get; set; }
    long CustomerID { get; set; }
    bool SubIsActive { get; set; }
    DateTime CustomerStartDate { get; set; }
    DateTime CustomerCancelDate { get; set; }
    long SubId { get; set; }
    bool CompletedInitial { get; set; }
    double ContractValue { get; set; }
    DateTime SubStartDate { get; set; }
    DateTime SubCancelDate { get; set; }
    string? Sellers { get; set; }
}
