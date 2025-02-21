namespace Automate.Domain.ValueObjects;

public enum CallBillability
{
    AlreadyBilled,
    Billable,
    MarkedIncorrectly,
    ServiceNotOffered,
    Renter,
    CurrentCustomer, 
    MissedCall,
    HangUp,
    Unknown,
    WrongArea,
    Spanish,
    RepeatCaller,
    Referral
}