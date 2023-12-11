using Validot;
using Validot.Results;

namespace LetsTalk.Channels.Dtos;

public sealed record class CreateChannelRequest(string ChannelName, string? ChannelIcon);

public static class CreateChannelRequestValidation
{
    private static readonly IValidator<CreateChannelRequest> S_Validator = Validator.Factory.Create<CreateChannelRequest>(spec => spec
        .Member(m => m.ChannelName, m => m
            .NotEmpty()
            .MaxLength(20))
        .Member(m => m.ChannelIcon, m => m
            .Optional()
            .Rule(v => Validation.Url.IsMatch(v!))
            .WithMessage("Invalid URL")));

    public static bool HasErrors(this CreateChannelRequest request, out IValidationResult result)
    {
        result = S_Validator.Validate(request);
        return result.AnyErrors;
    }
}