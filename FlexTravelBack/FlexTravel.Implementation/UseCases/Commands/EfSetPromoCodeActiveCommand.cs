using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfSetPromoCodeActiveCommand : EfUseCase, ISetPromoCodeActiveCommand
    {
        private readonly CurrentAgency _agency;

        public EfSetPromoCodeActiveCommand(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.SetPromoCodeActive;
        public string UseCaseName => "Set promo code active";
        public string UseCaseDescription => "Agency switches a promo code on or off";

        public void Execute(SetPromoCodeActiveDto request)
        {
            var agencyId = _agency.Id(UseCaseName);
            var promo = Context.PromoCode.FirstOrDefault(p => p.PromoCodeId == request.Id && p.AgencyId == agencyId);

            if (promo == null)
            {
                throw new EntityNotFoundException("Promo code", request.Id);
            }

            promo.IsActive = request.IsActive;
            Context.SaveChanges();
        }
    }
}
