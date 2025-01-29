using Automate.Translation.CustomerTranslate;

namespace Automate.Translation.QualifiedMessageTranslate;

public interface IQualifiedMessageTranslate : ICustSubLongIdLongNumberStrSellers
{
    bool ImLead { get; set; }
    bool SalesLead { get; set; }
}
