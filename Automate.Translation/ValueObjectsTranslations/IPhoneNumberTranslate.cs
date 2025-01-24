namespace Automate.Translation.ValueObjectsTranslations;

public interface IPhoneNumberTranslate
{
    public bool IsDefault { get; set; }
    public long Number { get; set; }
}
