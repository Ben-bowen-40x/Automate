using Automate.Translation.CustomerTranslate;
using Automate.Translation.MessageTranslate;

namespace Automate.Translation.QualifiedMessageTranslate;

public interface IQualifiedMessageTranslate : ICustSubLongIdLongNumberStrSellers
{
    bool ImLead { get; set; }
    bool SalesLead { get; set; }
}
