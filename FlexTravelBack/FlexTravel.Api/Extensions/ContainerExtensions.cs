using FlexTravel.Api.Core;
using FlexTravel.Api.Hubs;
using FlexTravel.Application.Notifications;
using FlexTravel.Implementation.Notifications;
using Microsoft.AspNetCore.SignalR;
using FlexTravel.Application.Loggers;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Logging;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation.UseCases.Commands;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.UseCases.Commands.Administration;
using FlexTravel.Implementation.UseCases.Commands.Account;
using FlexTravel.Implementation.UseCases.Commands.Catalog;
using FlexTravel.Implementation.UseCases.Queries.Catalog;
using FlexTravel.Application.Files;
using FlexTravel.Implementation.UseCases.Queries;
using FlexTravel.Implementation.UseCases.Queries.Administration;
using FlexTravel.Implementation.Validators;
using FlexTravel.Implementation.Jobs;
using FlexTravel.Application.Jobs;
using FlexTravel.Application.Weather;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace FlexTravel.Api.Extensions
{
    public static class ContainerExtensions
    {
        public static void AddJwt(this IServiceCollection services, AppSettings settings)
        {
            services.AddTransient(x =>
            {
                var context = x.GetRequiredService<FlexTravelContext>();

                return new JwtManager(context, settings.JwtSettings);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = settings.JwtSettings.Issuer,
                    ValidateIssuer = true,
                    ValidAudience = "Any",
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSettings.SecretKey)),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                cfg.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token) && context.HttpContext.Request.Path.StartsWithSegments(NotificationsHub.Path))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void AddNotifications(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            services.AddSingleton<INotificationPublisher, SignalRNotificationPublisher>();

            services.AddScoped<NotificationService>();
        }

        public static void AddApplicationUser(this IServiceCollection services)
        {
            services.AddTransient<IUser>(x =>
            {
                var accessor = x.GetRequiredService<IHttpContextAccessor>();
                var claims = accessor.HttpContext?.User;

                if (claims == null || claims.FindFirst("UserId") == null)
                {
                    return new AnonymousUser();
                }

                var actor = new JwtUser
                {
                    Email = claims.FindFirst("Email").Value,
                    Id = int.Parse(claims.FindFirst("UserId").Value),
                    Identity = claims.FindFirst("Identity")?.Value ?? claims.FindFirst("Email").Value,
                    UseCaseIds = JsonConvert.DeserializeObject<List<int>>(claims.FindFirst("UseCases").Value)
                };

                var context = x.GetRequiredService<FlexTravelContext>();
                if (!context.User.Any(u => u.UserId == actor.Id && u.IsActive))
                {
                    return new AnonymousUser();
                }

                return actor;
            });
        }

        public static void AddFlexTravelContext(this IServiceCollection services, AppSettings settings)
        {
            services.AddDbContext<FlexTravelContext>(options =>
                options.UseSqlServer(settings.ConnectionString));
        }

        public static void AddLoggers(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionLogger, ConsoleExceptionLogger>();
            services.AddTransient<IUseCaseLogger, EfUseCaseLogger>();
        }

        public static void AddUseCases(this IServiceCollection services)
        {
            services.AddTransient<IFillDatabaseCommand, FillDatabaseCommandEf>();
            services.AddTransient<IGetContinentsQuery, EfGetContinentsQuery>();
            services.AddTransient<IGetCountriesQuery, EfGetCountriesQuery>();
            services.AddTransient<IGetDestinationsQuery, EfGetDestinationsQuery>();
            services.AddTransient<IGetDestinationQuery, EfGetDestinationQuery>();
            services.AddTransient<IGetAuditLogsQuery, EfGetAuditLogsQuery>();

            services.AddTransient<BookingCalculator>();
            services.AddTransient<CurrentAgency>();
            services.AddTransient<WaitlistService>();
            services.AddTransient<ISearchTripsQuery, EfSearchTripsQuery>();
            services.AddTransient<IGetTripQuery, EfGetTripQuery>();
            services.AddTransient<ICalculatePriceQuery, EfCalculatePriceQuery>();
            services.AddTransient<IRegisterUserCommand, EfRegisterUserCommand>();
            services.AddTransient<ICreateReservationCommand, EfCreateReservationCommand>();
            services.AddTransient<IGetMyReservationsQuery, EfGetMyReservationsQuery>();
            services.AddTransient<ICancelReservationCommand, EfCancelReservationCommand>();

            services.AddTransient<IGetAgencyTripsQuery, EfGetAgencyTripsQuery>();
            services.AddTransient<ICreateTripCommand, EfCreateTripCommand>();
            services.AddTransient<IDeactivateTripCommand, EfDeactivateTripCommand>();
            services.AddTransient<IGetAgencyReservationsQuery, EfGetAgencyReservationsQuery>();
            services.AddTransient<IDecideReservationCommand, EfDecideReservationCommand>();
            services.AddTransient<IGetTripFormOptionsQuery, EfGetTripFormOptionsQuery>();
            services.AddTransient<IGetTripForEditQuery, EfGetTripForEditQuery>();
            services.AddTransient<IUpdateTripCommand, EfUpdateTripCommand>();

            services.AddTransient<IGetTripDestinationsQuery, EfGetTripDestinationsQuery>();
            services.AddTransient<IGetReservationQuery, EfGetReservationQuery>();

            services.AddTransient<IGetAdminStatsQuery, EfGetAdminStatsQuery>();
            services.AddTransient<ISearchUsersQuery, EfSearchUsersQuery>();
            services.AddTransient<IGetUserQuery, EfGetUserQuery>();
            services.AddTransient<IUpdateUserUseCasesCommand, EfUpdateUserUseCasesCommand>();
            services.AddTransient<ISetUserActiveCommand, EfSetUserActiveCommand>();
            services.AddTransient<IGetAgenciesQuery, EfGetAgenciesQuery>();
            services.AddTransient<ICreateAgencyCommand, EfCreateAgencyCommand>();
            services.AddTransient<ICreateAgencyEmployeeCommand, EfCreateAgencyEmployeeCommand>();

            services.AddTransient<IGetMyNotificationsQuery, EfGetMyNotificationsQuery>();
            services.AddTransient<IMarkNotificationsReadCommand, EfMarkNotificationsReadCommand>();
            services.AddTransient<IGetMyFavoritesQuery, EfGetMyFavoritesQuery>();
            services.AddTransient<ISetFavoriteCommand, EfSetFavoriteCommand>();
            services.AddTransient<IGetReviewsQuery, EfGetReviewsQuery>();
            services.AddTransient<ICreateReviewCommand, EfCreateReviewCommand>();
            services.AddTransient<IReplyToReviewCommand, EfReplyToReviewCommand>();
            services.AddTransient<IModerateReviewCommand, EfModerateReviewCommand>();
            services.AddTransient<IJoinWaitlistCommand, EfJoinWaitlistCommand>();
            services.AddTransient<ILeaveWaitlistCommand, EfLeaveWaitlistCommand>();
            services.AddTransient<IGetMyWaitlistQuery, EfGetMyWaitlistQuery>();
            services.AddTransient<IGetPromoCodesQuery, EfGetPromoCodesQuery>();
            services.AddTransient<ICreatePromoCodeCommand, EfCreatePromoCodeCommand>();
            services.AddTransient<ISetPromoCodeActiveCommand, EfSetPromoCodeActiveCommand>();

            services.AddSingleton<IFileStorage, LocalFileStorage>();
            services.AddTransient<IGetCatalogOptionsQuery, EfGetCatalogOptionsQuery>();
            services.AddTransient<IGetDestinationForEditQuery, EfGetDestinationForEditQuery>();
            services.AddTransient<ISaveCountryCommand, EfSaveCountryCommand>();
            services.AddTransient<ISaveDestinationCommand, EfSaveDestinationCommand>();
            services.AddTransient<ISaveHotelCommand, EfSaveHotelCommand>();
            services.AddTransient<ISaveSightCommand, EfSaveSightCommand>();
            services.AddTransient<IDeleteSightCommand, EfDeleteSightCommand>();
            services.AddTransient<IUploadImageCommand, EfUploadImageCommand>();
            services.AddTransient<ISetDestinationActiveCommand, EfSetDestinationActiveCommand>();
            services.AddTransient<ISetHotelActiveCommand, EfSetHotelActiveCommand>();
            services.AddTransient<ICleanUpImagesCommand, EfCleanUpImagesCommand>();

            services.AddTransient<ReservationAccess>();
            services.AddTransient<IGetMyProfileQuery, EfGetMyProfileQuery>();
            services.AddTransient<IUpdateMyProfileCommand, EfUpdateMyProfileCommand>();
            services.AddTransient<IChangePasswordCommand, EfChangePasswordCommand>();
            services.AddTransient<IUpdateTravelersCommand, EfUpdateTravelersCommand>();
            services.AddTransient<IGetTripManifestQuery, EfGetTripManifestQuery>();
            services.AddTransient<IRecordPaymentCommand, EfRecordPaymentCommand>();
            services.AddTransient<IGetReservationMessagesQuery, EfGetReservationMessagesQuery>();
            services.AddTransient<ISendReservationMessageCommand, EfSendReservationMessageCommand>();
            services.AddTransient<IGetSimilarTripsQuery, EfGetSimilarTripsQuery>();
            services.AddTransient<IAddDepartureCommand, EfAddDepartureCommand>();

            services.AddTransient<PriceAlertService>();
            services.AddTransient<ScheduledJobs>();
            services.AddTransient<IGetWeatherQuery, EfGetWeatherQuery>();
            services.AddTransient<IGetMyPriceAlertsQuery, EfGetMyPriceAlertsQuery>();
            services.AddTransient<ISetPriceAlertCommand, EfSetPriceAlertCommand>();
            services.AddTransient<IGetExchangeRatesQuery, EfGetExchangeRatesQuery>();
            services.AddTransient<ISetExchangeRateCommand, EfSetExchangeRateCommand>();
            services.AddTransient<IGetAgencyCalendarQuery, EfGetAgencyCalendarQuery>();
            services.AddTransient<IGetAgencyReportQuery, EfGetAgencyReportQuery>();
            services.AddTransient<IGetAgencyPageQuery, EfGetAgencyPageQuery>();
            services.AddTransient<IUpdateAgencyProfileCommand, EfUpdateAgencyProfileCommand>();
            services.AddTransient<IRunScheduledJobsCommand, EfRunScheduledJobsCommand>();
            services.AddTransient<IGetScheduledJobRunsQuery, EfGetScheduledJobRunsQuery>();

            services.AddTransient<IChangeMyAvatarCommand, EfChangeMyAvatarCommand>();
            services.AddTransient<IRemoveUserAvatarCommand, EfRemoveUserAvatarCommand>();
            services.AddTransient<IUpdateUserCommand, EfUpdateUserCommand>();
        }

        public static void AddWeather(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient(OpenMeteoWeatherProvider.ClientName, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(8);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("FlexTravel/1.0");
            });
            services.AddSingleton<IWeatherProvider, OpenMeteoWeatherProvider>();
        }

        public static void AddScheduledJobs(this IServiceCollection services)
        {
            services.AddSingleton<ScheduledJobsHostedService>();
            services.AddSingleton<IJobSchedule>(x => x.GetRequiredService<ScheduledJobsHostedService>());
            services.AddHostedService(x => x.GetRequiredService<ScheduledJobsHostedService>());
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.AddTransient<BookingRequestValidator>();
            services.AddTransient<RegisterUserValidator>();
            services.AddTransient<CreateTripValidator>();
            services.AddTransient<CreateAgencyValidator>();
            services.AddTransient<CreateAgencyEmployeeValidator>();
            services.AddTransient<UpdateUserUseCasesValidator>();
            services.AddTransient<CreateReviewValidator>();
            services.AddTransient<ReplyToReviewValidator>();
            services.AddTransient<ModerateReviewValidator>();
            services.AddTransient<JoinWaitlistValidator>();
            services.AddTransient<CreatePromoCodeValidator>();
            services.AddTransient<SaveCountryValidator>();
            services.AddTransient<SaveDestinationValidator>();
            services.AddTransient<SaveHotelValidator>();
            services.AddTransient<SaveSightValidator>();
            services.AddTransient<UploadImageValidator>();
            services.AddTransient<UpdateProfileValidator>();
            services.AddTransient<ChangePasswordValidator>();
            services.AddTransient<UpdateAgencyProfileValidator>();
            services.AddTransient<UpdateUserValidator>();
        }
    }
}
