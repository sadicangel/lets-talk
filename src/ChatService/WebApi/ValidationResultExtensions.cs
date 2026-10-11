using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Chat;

internal static class ValidationResultExtensions
{
    extension(IEnumerable<ValidationResult> validationResults)
    {
        public Dictionary<string, string[]> ToDictionary()
        {
            var dictionary = new Dictionary<string, List<string>>();
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    if (!dictionary.TryGetValue(memberName, out var errors))
                        dictionary[memberName] = errors = [];

                    errors.Add(validationResult.ErrorMessage ?? string.Empty);
                }
            }

            return dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }
    }
}
