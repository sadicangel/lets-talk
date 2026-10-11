using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Chat;

public readonly record struct SuccessResult;

public readonly record struct SuccessResult<T>(T Value);

public readonly record struct InvalidResult(ImmutableArray<ValidationResult> Errors)
{
    public InvalidResult(ValidationResult error) : this([error]) { }
}

public readonly record struct ChannelNotFoundResult;

public readonly record struct UserAlreadyMemberResult;

public readonly record struct UserNotMemberResult;

public readonly record struct UnauthorizedResult;
