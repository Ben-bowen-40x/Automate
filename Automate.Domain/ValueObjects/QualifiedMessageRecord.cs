namespace Automate.Domain.ValueObjects;

public record QualifiedMessageRecord(IMessage Message, ICustomerSubscription Customer, bool Billable, bool IsSalesLead);

