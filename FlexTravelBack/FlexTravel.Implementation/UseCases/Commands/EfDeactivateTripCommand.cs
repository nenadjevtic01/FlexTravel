using FlexTravel.Application.Constants;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfDeactivateTripCommand : EfUseCase, IDeactivateTripCommand
    {
        private readonly CurrentAgency _agency;

        public EfDeactivateTripCommand(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.DeactivateTrip;
        public string UseCaseName => "Deactivate trip";
        public string UseCaseDescription => "Agency withdraws a trip";

        public void Execute(int tripId)
        {
            var agencyId = _agency.Id(UseCaseName);
            var trip = Context.Trip.FirstOrDefault(x => x.TripId == tripId && x.AgencyId == agencyId);

            if (trip == null)
            {
                throw new EntityNotFoundException("Trip", tripId);
            }

            if (!trip.IsActive)
            {
                throw new ConflictException("This trip is already withdrawn.");
            }

            var activeReservations = Context.Reservation.Count(r => r.TripId == tripId && r.IsActive
                && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed));

            if (activeReservations > 0)
            {
                throw new ConflictException(
                    $"The trip has {activeReservations} active reservation(s). Reject or resolve them before withdrawing it.");
            }

            trip.IsActive = false;
            Context.SaveChanges();
        }
    }
}
