using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfSetFavoriteCommand : EfUseCase, ISetFavoriteCommand
    {
        private readonly IUser _user;

        public EfSetFavoriteCommand(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.SetFavorite;
        public string UseCaseName => "Set favorite";
        public string UseCaseDescription => "Add or remove a favorite destination";

        public void Execute(SetFavoriteDto request)
        {
            if (!Context.Destination.Any(d => d.DestinationId == request.DestinationId && d.IsActive))
            {
                throw new EntityNotFoundException("Destination", request.DestinationId);
            }

            var existing = Context.UserFavorite
                .FirstOrDefault(f => f.UserId == _user.Id && f.DestinationId == request.DestinationId);

            if (request.IsFavorite && existing == null)
            {
                Context.UserFavorite.Add(new UserFavorites { UserId = _user.Id, DestinationId = request.DestinationId });
            }
            else if (!request.IsFavorite && existing != null)
            {
                Context.UserFavorite.Remove(existing);
            }

            Context.SaveChanges();
        }
    }
}
