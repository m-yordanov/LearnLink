using System.Text.RegularExpressions;
using static LearnLink.Core.Constants.MessageConstants;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerExtensions
    {
        private static readonly Dictionary<string, string> FilterLabels = new(StringComparer.OrdinalIgnoreCase)
        {
            ["DateAfter"] = "after date",
            ["DateBefore"] = "before date",
            ["SelectedStudent"] = "student",
            ["SelectedTeacher"] = "teacher",
            ["SelectedSubject"] = "subject",
            ["SelectedStatus"] = "status",
            ["SortBy"] = "sort column",
            ["SortDescending"] = "sort direction",
            ["PageNumber"] = "page number",
            ["PageSize"] = "page size",
            ["page"] = "page number",
            ["searchString"] = "search term"
        };

        public static void WarnAboutIgnoredFilters(this Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                return;
            }

            var unreadable = controller.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0
                    && !string.IsNullOrWhiteSpace(entry.Value.AttemptedValue))
                .Select(entry => Label(entry.Key))
                .Distinct()
                .ToList();

            if (!unreadable.Any())
            {
                return;
            }

            controller.TempData[UserMessageError] = unreadable.Count == 1
                ? $"Invalid {unreadable[0]} - that filter was ignored."
                : $"Invalid {string.Join(", ", unreadable)} - those filters were ignored.";
        }

        private static string Label(string key)
        {
            var name = key.Split('.').Last();

            return FilterLabels.TryGetValue(name, out var label)
                ? label
                : Regex.Replace(name, "(?<!^)([A-Z])", " $1").ToLowerInvariant();
        }
    }
}
