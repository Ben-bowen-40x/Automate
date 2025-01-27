using CSharpFunctionalExtensions;

namespace Automate.Domain.ValueObjects;

public record UpdateResult(Result UploadedContacts, Result<DirectoryInfo> ContactLocation);
