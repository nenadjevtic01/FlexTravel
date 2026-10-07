using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetPromoCodesQuery : EfUseCase, IGetPromoCodesQuery
    {
        private readonly CurrentAgency _agency;

        public EfGetPromoCodesQuery(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.GetPromoCodes;
        public string UseCaseName => "Get promo codes";
        public string UseCaseDescription => "Promo codes of the agency with usage";

        public IEnumerable<PromoCodeDto> Execute(int request)
        {
            var agencyId = _agency.Id(UseCaseName);
            var today = DateTime.Today;

            return Context.PromoCode
                .Where(p => p.AgencyId == agencyId)
                .OrderByDescending(p => p.ValidTo)
                .Select(p => new
                {
                    p.PromoCodeId,
                    p.Code,
                    p.Description,
                    p.DiscountPercent,
                    p.ValidFrom,
                    p.ValidTo,
                    p.MaxUses,
                    p.IsActive,
                    p.CreatedAt,
                    CreatedBy = p.CreatedBy == null ? null : p.CreatedBy.FirstName + " " + p.CreatedBy.LastName,
                    Used = p.Reservations
                        .Where(r => r.IsActive && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                        .Select(r => new { r.Subtotal, r.PromoPercent })
                        .ToList()
                })
                .ToList()
                .Select(p =>
                {
                    var uses = p.Used.Count;
                    return new PromoCodeDto
                    {
                        Id = p.PromoCodeId,
                        Code = p.Code,
                        Description = p.Description,
                        DiscountPercent = p.DiscountPercent,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        MaxUses = p.MaxUses,
                        Uses = uses,
                        DiscountGiven = p.Used.Sum(r => BookingPricing.Discount(r.Subtotal, r.PromoPercent)),
                        IsActive = p.IsActive,
                        Status = !p.IsActive ? "Switched off"
                            : today > p.ValidTo.Date ? "Expired"
                            : p.MaxUses.HasValue && uses >= p.MaxUses.Value ? "Used up"
                            : today < p.ValidFrom.Date ? "Scheduled"
                            : "Active",
                        CreatedBy = p.CreatedBy,
                        CreatedAt = p.CreatedAt
                    };
                })
                .ToList();
        }
    }
}
