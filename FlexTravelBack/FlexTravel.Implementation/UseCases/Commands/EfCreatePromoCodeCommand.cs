using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Validators;
using FluentValidation;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfCreatePromoCodeCommand : EfUseCase, ICreatePromoCodeCommand
    {
        private readonly CurrentAgency _agency;
        private readonly IUser _user;
        private readonly CreatePromoCodeValidator _validator;

        public EfCreatePromoCodeCommand(FlexTravelContext context, CurrentAgency agency, IUser user,
            CreatePromoCodeValidator validator) : base(context)
        {
            _agency = agency;
            _user = user;
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.CreatePromoCode;
        public string UseCaseName => "Create promo code";
        public string UseCaseDescription => "Agency issues a promo code";

        public int Execute(CreatePromoCodeDto request)
        {
            var agencyId = _agency.Id(UseCaseName);
            _validator.ValidateAndThrow(request);

            var promo = new PromoCode
            {
                AgencyId = agencyId,
                Code = BookingPricing.NormalizePromoCode(request.Code),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                DiscountPercent = request.DiscountPercent,
                ValidFrom = request.ValidFrom.Date,
                ValidTo = request.ValidTo.Date,
                MaxUses = request.MaxUses,
                CreatedByUserId = _user.Id
            };

            Context.PromoCode.Add(promo);
            Context.SaveChanges();

            return promo.PromoCodeId;
        }
    }
}
