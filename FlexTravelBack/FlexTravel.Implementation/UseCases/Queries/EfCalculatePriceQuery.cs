using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfCalculatePriceQuery : ICalculatePriceQuery
    {
        private readonly BookingCalculator _calculator;

        public EfCalculatePriceQuery(BookingCalculator calculator)
        {
            _calculator = calculator;
        }

        public int UseCaseId => UseCaseIds.CalculatePrice;
        public string UseCaseName => "Calculate price";
        public string UseCaseDescription => "Price quote for a booking without saving it";

        public PriceQuoteDto Execute(BookingRequestDto request)
        {
            return _calculator.Quote(request);
        }
    }
}
