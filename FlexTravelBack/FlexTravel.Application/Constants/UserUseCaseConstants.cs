namespace FlexTravel.Application.Constants
{
    public static class UseCaseIds
    {
        public const int FillDatabase = 1;
        public const int GetContinents = 2;
        public const int GetCountries = 3;
        public const int GetDestinations = 4;
        public const int GetDestination = 5;
        public const int GetAuditLogs = 6;

        public const int SearchTrips = 7;
        public const int GetTrip = 8;
        public const int CalculatePrice = 9;
        public const int RegisterUser = 10;

        public const int CreateReservation = 11;
        public const int GetMyReservations = 12;
        public const int CancelReservation = 13;

        public const int GetAgencyTrips = 14;
        public const int CreateTrip = 15;
        public const int DeactivateTrip = 16;
        public const int GetAgencyReservations = 17;
        public const int DecideReservation = 18;
        public const int GetTripFormOptions = 19;

        public const int GetTripDestinations = 20;
        public const int GetReservation = 21;
        public const int UpdateTrip = 22;
        public const int GetTripForEdit = 23;

        public const int GetAdminStats = 24;
        public const int SearchUsers = 25;
        public const int GetUser = 26;
        public const int UpdateUserUseCases = 27;
        public const int SetUserActive = 28;
        public const int GetAgencies = 29;
        public const int CreateAgency = 30;
        public const int CreateAgencyEmployee = 31;

        public const int GetMyNotifications = 32;
        public const int MarkNotificationsRead = 33;
        public const int GetMyFavorites = 34;
        public const int SetFavorite = 35;
        public const int GetReviews = 36;
        public const int CreateReview = 37;
        public const int ReplyToReview = 38;
        public const int ModerateReview = 39;
        public const int JoinWaitlist = 40;
        public const int LeaveWaitlist = 41;
        public const int GetMyWaitlist = 42;
        public const int GetPromoCodes = 43;
        public const int CreatePromoCode = 44;
        public const int SetPromoCodeActive = 45;

        public const int SaveCountry = 46;
        public const int SaveDestination = 47;
        public const int GetDestinationForEdit = 48;
        public const int SaveHotel = 49;
        public const int SaveSight = 50;
        public const int DeleteSight = 51;
        public const int GetCatalogOptions = 52;
        public const int UploadImage = 53;

        public const int GetMyProfile = 54;
        public const int UpdateMyProfile = 55;
        public const int ChangePassword = 56;
        public const int UpdateTravelers = 57;
        public const int GetTripManifest = 58;
        public const int RecordPayment = 59;
        public const int GetReservationMessages = 60;
        public const int SendReservationMessage = 61;
        public const int GetSimilarTrips = 62;
        public const int SetDestinationActive = 63;
        public const int SetHotelActive = 64;
        public const int CleanUpImages = 65;
        public const int AddDeparture = 66;

        public const int GetWeather = 67;
        public const int GetMyPriceAlerts = 68;
        public const int SetPriceAlert = 69;
        public const int GetExchangeRates = 70;
        public const int SetExchangeRate = 71;
        public const int GetAgencyCalendar = 72;
        public const int GetAgencyReport = 73;
        public const int GetAgencyPage = 74;
        public const int UpdateAgencyProfile = 75;
        public const int RunScheduledJobs = 76;
        public const int GetScheduledJobRuns = 77;

        public const int ChangeMyAvatar = 78;
        public const int RemoveUserAvatar = 79;
        public const int UpdateUser = 80;
    }

    public static class UserUseCaseConstants
    {
        private static readonly int[] Browse =
        {
            UseCaseIds.GetContinents, UseCaseIds.GetCountries, UseCaseIds.GetDestinations, UseCaseIds.GetDestination,
            UseCaseIds.SearchTrips, UseCaseIds.GetTrip, UseCaseIds.CalculatePrice, UseCaseIds.GetTripDestinations,
            UseCaseIds.GetReviews, UseCaseIds.GetSimilarTrips, UseCaseIds.GetWeather, UseCaseIds.GetExchangeRates,
            UseCaseIds.GetAgencyPage
        };

        private static readonly int[] Account =
        {
            UseCaseIds.GetMyProfile, UseCaseIds.UpdateMyProfile, UseCaseIds.ChangePassword, UseCaseIds.ChangeMyAvatar
        };

        public static List<int> AnonymousUserUseCaseIds =>
            new List<int>(Browse) { UseCaseIds.FillDatabase, UseCaseIds.RegisterUser };

        public static List<int> RegularUserUseCaseIds =>
            new List<int>(Browse)
            {
                UseCaseIds.CreateReservation, UseCaseIds.GetMyReservations, UseCaseIds.CancelReservation,
                UseCaseIds.GetReservation, UseCaseIds.GetMyNotifications, UseCaseIds.MarkNotificationsRead,
                UseCaseIds.GetMyFavorites, UseCaseIds.SetFavorite, UseCaseIds.CreateReview,
                UseCaseIds.JoinWaitlist, UseCaseIds.LeaveWaitlist, UseCaseIds.GetMyWaitlist,
                UseCaseIds.UpdateTravelers, UseCaseIds.GetReservationMessages, UseCaseIds.SendReservationMessage,
                UseCaseIds.GetMyPriceAlerts, UseCaseIds.SetPriceAlert
            }.Concat(Account).ToList();

        public static List<int> AgencyEmployeeUseCaseIds =>
            new List<int>(Browse)
            {
                UseCaseIds.GetAgencyTrips, UseCaseIds.CreateTrip, UseCaseIds.DeactivateTrip,
                UseCaseIds.GetAgencyReservations, UseCaseIds.DecideReservation, UseCaseIds.GetTripFormOptions,
                UseCaseIds.GetReservation, UseCaseIds.UpdateTrip, UseCaseIds.GetTripForEdit,
                UseCaseIds.GetMyNotifications, UseCaseIds.MarkNotificationsRead, UseCaseIds.ReplyToReview,
                UseCaseIds.GetPromoCodes, UseCaseIds.CreatePromoCode, UseCaseIds.SetPromoCodeActive,
                UseCaseIds.GetTripManifest, UseCaseIds.RecordPayment, UseCaseIds.GetReservationMessages,
                UseCaseIds.SendReservationMessage, UseCaseIds.AddDeparture, UseCaseIds.GetAgencyCalendar,
                UseCaseIds.GetAgencyReport, UseCaseIds.UpdateAgencyProfile, UseCaseIds.UploadImage
            }.Concat(Account).ToList();

        public static List<int> AdminUseCaseIds =>
            new List<int>(Browse)
            {
                UseCaseIds.FillDatabase, UseCaseIds.GetAuditLogs, UseCaseIds.GetAdminStats, UseCaseIds.SearchUsers,
                UseCaseIds.GetUser, UseCaseIds.UpdateUserUseCases, UseCaseIds.SetUserActive, UseCaseIds.GetAgencies,
                UseCaseIds.CreateAgency, UseCaseIds.CreateAgencyEmployee, UseCaseIds.ModerateReview,
                UseCaseIds.SaveCountry, UseCaseIds.SaveDestination, UseCaseIds.GetDestinationForEdit,
                UseCaseIds.SaveHotel, UseCaseIds.SaveSight, UseCaseIds.DeleteSight, UseCaseIds.GetCatalogOptions,
                UseCaseIds.UploadImage, UseCaseIds.SetDestinationActive, UseCaseIds.SetHotelActive,
                UseCaseIds.CleanUpImages, UseCaseIds.SetExchangeRate, UseCaseIds.RunScheduledJobs,
                UseCaseIds.GetScheduledJobRuns, UseCaseIds.RemoveUserAvatar, UseCaseIds.UpdateUser
            }.Concat(Account).ToList();

        public static readonly int[] AdminLockoutGuard = { UseCaseIds.GetUser, UseCaseIds.UpdateUserUseCases };
    }

    public static class BookingRules
    {
        public const int MaxTravelersPerReservation = 10;

        public const int ChildUntilAge = 12;

        public static readonly (int MinDaysBefore, int FeePercent)[] CancellationFees =
        {
            (30, 0), (15, 30), (7, 50), (0, 100)
        };

        public static int FreeCancellationDays => CancellationFees[0].MinDaysBefore;

        public const int DepositPercent = 30;
        public const int DepositDueDaysAfterConfirmation = 7;
        public const int BalanceDueDaysBeforeDeparture = 21;

        public const int MaxDiscountPercent = 50;

        public const int PendingExpiryDays = 3;

        public const int PaymentReminderDaysBefore = 3;

        public const int ReviewInvitationWithinDays = 30;

        public const int MaxExtrasPerTrip = 10;
    }

    public static class SessionRules
    {
        public const int RefreshTokenDays = 1;
        public const int RememberMeDays = 30;
    }

    public static class SignInRules
    {
        public const int MaxFailedAttempts = 5;
        public const int LockoutMinutes = 15;
    }
}
