using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;
using Validot.Results;

namespace LetsTalk;

internal static partial class Validation
{
    public static Regex Url { get; } = GetUrl();

    [GeneratedRegex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$")]
    private static partial Regex GetUrl();

    public static ValidationProblem ToValidationProblem(this IValidationResult result) =>
        TypedResults.ValidationProblem(result.MessageMap.ToDictionary(x => x.Key, x => x.Value.ToArray()));
}