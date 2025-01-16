using CSharpFunctionalExtensions;

namespace Automate.Domain.ValueObjects;

public record UpdateResult(bool UploadedContacts, Result<DirectoryInfo> ContactLocation);
