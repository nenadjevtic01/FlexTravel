using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Domain;
using FluentValidation.Results;

namespace FlexTravel.Implementation.Booking
{
    public static class CancellationRules
    {
        public static bool CanCancel(ReservationStatus status, DateTime startDate, DateTime today) =>
            BookingPricing.HoldsCapacity(status) && startDate.Date > today.Date;

        public static int FeePercent(ReservationStatus status, DateTime startDate, DateTime today)
        {
            if (status == ReservationStatus.Pending)
            {
                return 0;
            }

            var daysBefore = (startDate.Date - today.Date).Days;
            foreach (var (minDaysBefore, feePercent) in BookingRules.CancellationFees)
            {
                if (daysBefore >= minDaysBefore)
                {
                    return feePercent;
                }
            }

            return 100;
        }

        public static decimal Fee(decimal totalPrice, int feePercent) => BookingPricing.Discount(totalPrice, feePercent);

        public static DateTime FreeUntil(DateTime startDate) => startDate.Date.AddDays(-BookingRules.FreeCancellationDays);

        public static List<CancellationTierDto> Policy(DateTime startDate) => BookingRules.CancellationFees
            .Select(t => new CancellationTierDto
            {
                MinDaysBefore = t.MinDaysBefore,
                FeePercent = t.FeePercent,
                Until = startDate.Date.AddDays(-t.MinDaysBefore)
            })
            .ToList();
    }

    public static class PaymentRules
    {
        public static decimal Deposit(decimal totalPrice) => BookingPricing.Discount(totalPrice, BookingRules.DepositPercent);

        public static PaymentSummaryDto Summary(ReservationStatus status, decimal totalPrice, decimal? cancellationFee,
            DateTime startDate, DateTime? confirmedAt, decimal paid, DateTime today)
        {
            var amountDue = status switch
            {
                ReservationStatus.Rejected => 0,
                ReservationStatus.Expired => 0,
                ReservationStatus.Cancelled => cancellationFee ?? 0,
                _ => totalPrice
            };

            var summary = new PaymentSummaryDto
            {
                AmountDue = amountDue,
                Paid = paid,
                Balance = Math.Max(0, amountDue - paid),
                Deposit = Deposit(totalPrice)
            };

            switch (status)
            {
                case ReservationStatus.Pending:

                    summary.Status = "Not due yet";
                    break;

                case ReservationStatus.Confirmed:
                    var confirmed = (confirmedAt ?? today).Date;
                    var lastDay = startDate.Date.AddDays(-1);
                    var balanceDue = startDate.Date.AddDays(-BookingRules.BalanceDueDaysBeforeDeparture);
                    if (balanceDue < confirmed)
                    {
                        balanceDue = Min(confirmed.AddDays(3), lastDay);
                    }

                    var depositDue = Min(confirmed.AddDays(BookingRules.DepositDueDaysAfterConfirmation), balanceDue);
                    summary.DepositDueDate = depositDue;
                    summary.BalanceDueDate = balanceDue;
                    summary.Status = paid >= amountDue ? "Paid" : paid >= summary.Deposit ? "Deposit paid" : "Unpaid";
                    summary.IsOverdue = (paid < summary.Deposit && today.Date > depositDue)
                        || (paid < amountDue && today.Date > balanceDue);
                    break;

                default:
                    summary.RefundDue = Math.Max(0, paid - amountDue);
                    summary.Status = summary.RefundDue > 0 ? "Refund due" : paid < amountDue ? "Unpaid" : "Settled";
                    break;
            }

            return summary;
        }

        private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
    }

    public static class PendingRules
    {
        public static DateTime DecideBy(DateTime bookedAt, DateTime startDate)
        {
            var deadline = bookedAt.AddDays(BookingRules.PendingExpiryDays);
            return deadline < startDate.Date ? deadline : startDate.Date;
        }

        public static bool IsExpired(DateTime bookedAt, DateTime startDate, DateTime now) => now >= DecideBy(bookedAt, startDate);
    }

    public static class TravelerRules
    {
        public static int AgeOn(DateTime dateOfBirth, DateTime day)
        {
            var age = day.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > day.Date.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        public static List<ValidationFailure> Check(IReadOnlyList<TravelerDto> travelers, int adults, int children,
            DateTime startDate, DateTime today)
        {
            var problems = new List<ValidationFailure>();
            var total = adults + children;

            if (travelers == null || travelers.Count != total)
            {
                problems.Add(new ValidationFailure("Travelers", $"Enter the names of all {total} travelers."));
                return problems;
            }

            for (var i = 0; i < travelers.Count; i++)
            {
                var t = travelers[i];
                var who = $"Traveler {i + 1}";

                if (string.IsNullOrWhiteSpace(t.FirstName) || string.IsNullOrWhiteSpace(t.LastName))
                {
                    problems.Add(new ValidationFailure($"Travelers[{i}]", $"{who}: enter the first and last name."));
                }
                else if (t.FirstName.Trim().Length > 50 || t.LastName.Trim().Length > 50)
                {
                    problems.Add(new ValidationFailure($"Travelers[{i}]", $"{who}: the name is too long."));
                }

                if (t.DateOfBirth.Date > today.Date || t.DateOfBirth.Date < today.Date.AddYears(-120))
                {
                    problems.Add(new ValidationFailure($"Travelers[{i}]", $"{who}: enter a valid date of birth."));
                }
            }

            if (problems.Count == 0)
            {
                var childCount = travelers.Count(t => AgeOn(t.DateOfBirth, startDate) < BookingRules.ChildUntilAge);
                if (childCount != children)
                {
                    problems.Add(new ValidationFailure("Travelers",
                        $"The dates of birth give {childCount} {(childCount == 1 ? "child" : "children")} under {BookingRules.ChildUntilAge} on departure, but {children} {(children == 1 ? "was" : "were")} booked."));
                }
            }

            return problems;
        }

        public static List<ReservationTraveler> ToEntities(IEnumerable<TravelerDto> travelers, DateTime startDate) => travelers
            .Select(t => new ReservationTraveler
            {
                FirstName = t.FirstName.Trim(),
                LastName = t.LastName.Trim(),
                DateOfBirth = t.DateOfBirth.Date,
                IsChild = AgeOn(t.DateOfBirth, startDate) < BookingRules.ChildUntilAge
            })
            .ToList();
    }
}
