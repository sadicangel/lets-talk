using Validot;
using Validot.Results;

namespace LetsTalk.Channels;

public sealed record class UpdateChannelRequest(string ChannelName, string ChannelIcon);

public static class UpdateChannelRequestValidation
{
    private static readonly IValidator<UpdateChannelRequest> S_Validator = Validator.Factory.Create<UpdateChannelRequest>(spec => spec
        .Member(m => m.ChannelName, m => m
            .NotEmpty()
            .MaxLength(20))
        .Member(m => m.ChannelIcon, m => m
            .NotEmpty()
            .Rule(v => Validation.Url.IsMatch(v!))
            .WithMessage("Invalid URL")));

    public static bool HasErrors(this UpdateChannelRequest request, out IValidationResult result)
    {
        result = S_Validator.Validate(request);
        return result.AnyErrors;
    }
}
