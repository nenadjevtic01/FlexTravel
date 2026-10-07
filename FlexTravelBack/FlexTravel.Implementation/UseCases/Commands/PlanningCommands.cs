using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Jobs;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using FluentValidation.Results;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfSetPriceAlertCommand : EfUseCase, ISetPriceAlertCommand
    {
        public const decimal MaxLimit = 100000;
        private readonly IUser _user;
        private readonly PriceAlertService _priceAlerts;

        public EfSetPriceAlertCommand(FlexTravelContext context, IUser user, PriceAlertService priceAlerts) : base(context)
        {
            _user = user;
            _priceAlerts = priceAlerts;
        }

        public int UseCaseId => UseCaseIds.SetPriceAlert;
        public string UseCaseName => "Set price alert";
        public string UseCaseDescription => "Set or remove a price alert for a destination";

        public void Execute(SetPriceAlertDto request)
        {
            if (!Context.Destination.Any(d => d.DestinationId == request.DestinationId && d.IsActive))
            {
                throw new EntityNotFoundException("Destination", request.DestinationId);
            }

            var alert = Context.PriceAlert.FirstOrDefault(a => a.UserId == _user.Id && a.DestinationId == request.DestinationId);

            if (request.MaxPrice == null)
            {
                if (alert != null)
                {
                    Context.PriceAlert.Remove(alert);
                    Context.SaveChanges();
                }

                return;
            }

            if (request.MaxPrice <= 0 || request.MaxPrice > MaxLimit)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("MaxPrice", "Enter the most you want to pay per person, in euros.")
                });
            }

            if (alert == null)
            {
                alert = new PriceAlert { UserId = _user.Id, DestinationId = request.DestinationId, CreatedAt = DateTime.UtcNow };
                Context.PriceAlert.Add(alert);
            }

            alert.MaxPrice = Math.Round(request.MaxPrice.Value, 2);
            var cheapest = _priceAlerts.Cheapest(request.DestinationId);
            alert.LastNotifiedPrice = cheapest != null && cheapest.PriceFrom <= alert.MaxPrice ? cheapest.PriceFrom : null;
            Context.SaveChanges();
        }
    }

    public class EfSetExchangeRateCommand : EfUseCase, ISetExchangeRateCommand
    {
        private readonly IUser _user;

        public EfSetExchangeRateCommand(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.SetExchangeRate;
        public string UseCaseName => "Set exchange rate";
        public string UseCaseDescription => "Admin updates an exchange rate";

        public void Execute(SetExchangeRateDto request)
        {
            var currency = (request.Currency ?? "").Trim().ToUpperInvariant();
            var rate = Context.ExchangeRate.FirstOrDefault(r => r.Currency == currency)
                ?? throw new EntityNotFoundException("Exchange rate", 0);

            if (request.RatePerEuro <= 0 || request.RatePerEuro > 100000)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("RatePerEuro", $"Enter how many {currency} one euro is worth.")
                });
            }

            rate.RatePerEuro = Math.Round(request.RatePerEuro, 4);
            rate.UpdatedAt = DateTime.UtcNow;
            rate.UpdatedByUserId = _user.Id;
            Context.SaveChanges();
        }
    }

    public class EfUpdateAgencyProfileCommand : EfUseCase, IUpdateAgencyProfileCommand
    {
        private readonly CurrentAgency _agency;
        private readonly UpdateAgencyProfileValidator _validator;

        public EfUpdateAgencyProfileCommand(FlexTravelContext context, CurrentAgency agency, UpdateAgencyProfileValidator validator)
            : base(context)
        {
            _agency = agency;
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.UpdateAgencyProfile;
        public string UseCaseName => "Update agency profile";
        public string UseCaseDescription => "Agency edits its public description, contacts and logo";

        public void Execute(UpdateAgencyProfileDto request)
        {
            var agencyId = _agency.Id(UseCaseName);
            _validator.ValidateAndThrow(request);

            var agency = Context.Agency.First(a => a.AgencyId == agencyId);
            agency.Description = Clean(request.Description);
            agency.Address = request.Address.Trim();
            agency.Phone = request.Phone.Trim();
            agency.Email = Clean(request.Email)?.ToLowerInvariant();
            agency.Website = Clean(request.Website);

            agency.Logo = Clean(request.Logo);
            Context.SaveChanges();
        }

        private static string Clean(string text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    public class EfRunScheduledJobsCommand : IRunScheduledJobsCommand
    {
        private readonly ScheduledJobs _jobs;
        private readonly IUser _user;

        public EfRunScheduledJobsCommand(ScheduledJobs jobs, IUser user)
        {
            _jobs = jobs;
            _user = user;
        }

        public int UseCaseId => UseCaseIds.RunScheduledJobs;
        public string UseCaseName => "Run scheduled jobs";
        public string UseCaseDescription => "Admin runs the background jobs now";

        public ScheduledJobRunDto Execute(int request) => _jobs.Run(_user.Identity, DateTime.Now);
    }
}
