using FlexTravel.Application.Dto;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.Booking
{
    public static class ItineraryRules
    {
        public static bool IsEmpty(ItineraryDayInputDto day) =>
            day == null || (string.IsNullOrWhiteSpace(day.Title) && string.IsNullOrWhiteSpace(day.Activity));

        public static List<ItineraryActivity> ToActivities(IReadOnlyList<ItineraryDayInputDto> days)
        {
            var activities = new List<ItineraryActivity>();
            for (var i = 0; i < days.Count; i++)
            {
                var day = days[i];
                if (IsEmpty(day))
                {
                    continue;
                }

                activities.Add(new ItineraryActivity
                {
                    Day = i + 1,
                    Title = Clean(day.Title),

                    Activity = Clean(day.Activity) ?? day.Title.Trim(),
                    Meals = (day.Breakfast ? Meals.Breakfast : Meals.None)
                        | (day.Lunch ? Meals.Lunch : Meals.None)
                        | (day.Dinner ? Meals.Dinner : Meals.None),
                    Overnight = Clean(day.Overnight)
                });
            }

            return activities;
        }

        public static ItineraryActivity Copy(ItineraryActivity a) => new ItineraryActivity
        {
            Day = a.Day,
            Title = a.Title,
            Activity = a.Activity,
            Meals = a.Meals,
            Overnight = a.Overnight
        };

        public static ItineraryDayDto ToDto(ItineraryActivity a) => new ItineraryDayDto
        {
            Day = a.Day,
            Title = a.Title,
            Activity = a.Activity,
            Breakfast = a.Meals.HasFlag(Meals.Breakfast),
            Lunch = a.Meals.HasFlag(Meals.Lunch),
            Dinner = a.Meals.HasFlag(Meals.Dinner),
            Overnight = a.Overnight
        };

        public static List<ItineraryDayInputDto> ToForm(IEnumerable<ItineraryActivity> activities)
        {
            var list = activities.ToList();
            var lastDay = list.Count == 0 ? 1 : list.Max(a => a.Day);

            return Enumerable.Range(1, lastDay)
                .Select(day =>
                {
                    var a = list.FirstOrDefault(x => x.Day == day);
                    return a == null
                        ? new ItineraryDayInputDto()
                        : new ItineraryDayInputDto
                        {
                            Title = a.Title,
                            Activity = a.Activity,
                            Breakfast = a.Meals.HasFlag(Meals.Breakfast),
                            Lunch = a.Meals.HasFlag(Meals.Lunch),
                            Dinner = a.Meals.HasFlag(Meals.Dinner),
                            Overnight = a.Overnight
                        };
                })
                .ToList();
        }

        private static string Clean(string text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    public static class ExtraRules
    {
        public static ExtraKind ParseKind(string kind) =>
            Enum.TryParse<ExtraKind>(kind, true, out var parsed) && Enum.IsDefined(parsed) ? parsed : ExtraKind.Other;

        public static TripExtra ToEntity(TripExtraInputDto input) => new TripExtra
        {
            Kind = ParseKind(input.Kind),
            Name = input.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
            Price = Math.Round(input.Price, 2),
            PerPerson = input.PerPerson
        };

        public static TripExtra Copy(TripExtra extra) => new TripExtra
        {
            Kind = extra.Kind,
            Name = extra.Name,
            Description = extra.Description,
            Price = extra.Price,
            PerPerson = extra.PerPerson
        };

        public static TripExtraDto ToDto(TripExtra extra) => new TripExtraDto
        {
            Id = extra.TripExtraId,
            Kind = extra.Kind.ToString(),
            Name = extra.Name,
            Description = extra.Description,
            Price = extra.Price,
            PerPerson = extra.PerPerson
        };

        public static void Apply(Trip trip, IEnumerable<TripExtraInputDto> inputs)
        {
            var wanted = inputs.ToList();
            var keptIds = wanted.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet();

            foreach (var extra in trip.Extras.Where(e => e.IsActive && !keptIds.Contains(e.TripExtraId)))
            {
                extra.IsActive = false;
            }

            foreach (var input in wanted)
            {
                var existing = input.Id.HasValue ? trip.Extras.FirstOrDefault(e => e.TripExtraId == input.Id.Value) : null;
                if (existing == null)
                {
                    trip.Extras.Add(ToEntity(input));
                    continue;
                }

                var changed = ToEntity(input);
                existing.Kind = changed.Kind;
                existing.Name = changed.Name;
                existing.Description = changed.Description;
                existing.Price = changed.Price;
                existing.PerPerson = changed.PerPerson;
                existing.IsActive = true;
            }
        }

        public static decimal Total(decimal unitPrice, int quantity) => Math.Round(unitPrice * quantity, 2);
    }
}
