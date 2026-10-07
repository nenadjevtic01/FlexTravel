using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetMyFavoritesQuery : EfUseCase, IGetMyFavoritesQuery
    {
        private readonly IUser _user;

        public EfGetMyFavoritesQuery(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetMyFavorites;
        public string UseCaseName => "Get my favorites";
        public string UseCaseDescription => "Favorite destinations of the customer";

        public IEnumerable<FavoriteDestinationDto> Execute(int request)
        {
            var today = DateTime.Today;

            return Context.UserFavorite
                .Where(f => f.UserId == _user.Id && f.Destination.IsActive)
                .OrderBy(f => f.Destination.DestinationName)
                .Select(f => new FavoriteDestinationDto
                {
                    Id = f.DestinationId,
                    Name = f.Destination.DestinationName,
                    Country = f.Destination.Country.CountryName,
                    Image = f.Destination.Images.OrderBy(i => i.DestinationImageId).Select(i => i.Src).FirstOrDefault(),
                    UpcomingTrips = f.Destination.Trips.Count(t => t.IsActive && t.StartDate > today),
                    NextDeparture = f.Destination.Trips
                        .Where(t => t.IsActive && t.StartDate > today)
                        .Min(t => (DateTime?)t.StartDate)
                })
                .ToList();
        }
    }
}
