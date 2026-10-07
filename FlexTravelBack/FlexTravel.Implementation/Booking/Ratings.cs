using FlexTravel.Application.Dto;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.Booking
{
    public static class Ratings
    {
        public static RatingDto Trip(IQueryable<Review> reviews) => Summarize(reviews.Where(r => r.IsActive).Select(r => r.TripRating));

        public static RatingDto Hotel(IQueryable<Review> reviews) => Summarize(reviews.Where(r => r.IsActive).Select(r => r.HotelRating));

        public static RatingDto Summarize(IQueryable<int> ratings)
        {
            var summary = ratings
                .GroupBy(x => 1)
                .Select(g => new { Count = g.Count(), Average = g.Average(x => (double)x) })
                .FirstOrDefault();

            return Create(summary?.Average, summary?.Count ?? 0);
        }

        public static RatingDto Create(double? average, int count) => new RatingDto
        {
            Count = count,
            Average = count == 0 || average == null ? null : Math.Round(average.Value, 1)
        };

        public static double? Round(double? average) => average == null ? null : Math.Round(average.Value, 1);

        public static RatingBreakdownDto Breakdown(IQueryable<Review> reviews)
        {
            var rows = reviews
                .Where(r => r.IsActive)
                .Select(r => new { r.TripRating, r.HotelRating, r.GuideRating, r.OrganisationRating, r.ValueRating })
                .ToList();

            RatingCategoryDto Category(string name, IEnumerable<int?> values)
            {
                var given = values.Where(v => v.HasValue).Select(v => (double)v.Value).ToList();
                return new RatingCategoryDto
                {
                    Name = name,
                    Count = given.Count,
                    Average = given.Count == 0 ? null : Math.Round(given.Average(), 1)
                };
            }

            return new RatingBreakdownDto
            {
                Count = rows.Count,
                Average = rows.Count == 0 ? null : Math.Round(rows.Average(r => (double)r.TripRating), 1),
                Stars = Enumerable.Range(0, 5).Select(i => rows.Count(r => r.TripRating == 5 - i)).ToArray(),
                Categories = new[]
                {
                    Category("Hotel", rows.Select(r => (int?)r.HotelRating)),
                    Category("Guide", rows.Select(r => r.GuideRating)),
                    Category("Organisation", rows.Select(r => r.OrganisationRating)),
                    Category("Value for money", rows.Select(r => r.ValueRating))
                }
                .Where(c => c.Count > 0)
                .ToList()
            };
        }
    }
}
