using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Commands
{
    public interface ISetPriceAlertCommand : ICommand<SetPriceAlertDto>
    {
    }

    public interface ISetExchangeRateCommand : ICommand<SetExchangeRateDto>
    {
    }

    public interface IUpdateAgencyProfileCommand : ICommand<UpdateAgencyProfileDto>
    {
    }

    public interface IRunScheduledJobsCommand : ICommand<int, ScheduledJobRunDto>
    {
    }

    public interface IChangeMyAvatarCommand : ICommand<UploadImageDto, string>
    {
    }

    public interface IRemoveUserAvatarCommand : ICommand<int>
    {
    }

    public interface IUpdateUserCommand : ICommand<UpdateUserDto>
    {
    }
}

namespace FlexTravel.Application.UseCases.Queries
{
    public interface IGetWeatherQuery : IQuery<WeatherRequestDto, WeatherDto>
    {
    }

    public interface IGetMyPriceAlertsQuery : IQuery<int, IEnumerable<PriceAlertDto>>
    {
    }

    public interface IGetExchangeRatesQuery : IQuery<int, IEnumerable<ExchangeRateDto>>
    {
    }

    public interface IGetAgencyCalendarQuery : IQuery<AgencyCalendarRequestDto, AgencyCalendarDto>
    {
    }

    public interface IGetAgencyReportQuery : IQuery<int, AgencyReportDto>
    {
    }

    public interface IGetAgencyPageQuery : IQuery<int, AgencyPageDto>
    {
    }

    public interface IGetScheduledJobRunsQuery : IQuery<int, ScheduledJobsOverviewDto>
    {
    }
}

namespace FlexTravel.Application.Weather
{
    public interface IWeatherProvider
    {
        IReadOnlyList<WeatherDayDto> Forecast(double latitude, double longitude, DateTime from, DateTime to);
        IReadOnlyList<WeatherDayDto> History(double latitude, double longitude, DateTime from, DateTime to);

        int ForecastDays { get; }
    }
}

namespace FlexTravel.Application.Jobs
{
    public interface IJobSchedule
    {
        int IntervalMinutes { get; }
        DateTime? NextRunAt { get; }
    }
}
