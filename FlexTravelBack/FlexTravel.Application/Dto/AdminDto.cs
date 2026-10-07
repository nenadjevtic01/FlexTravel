namespace FlexTravel.Application.Dto
{
    public class AdminStatsDto
    {
        public int Customers { get; set; }
        public int AgencyEmployees { get; set; }
        public int Admins { get; set; }
        public int BlockedUsers { get; set; }
        public int Agencies { get; set; }
        public int TripsOnSale { get; set; }
        public int ReservationsPending { get; set; }
        public int ReservationsConfirmed { get; set; }
        public int ReservationsCancelled { get; set; }
        public int ReservationsRejected { get; set; }
        public decimal ConfirmedRevenue { get; set; }
        public decimal CollectedPayments { get; set; }
        public decimal PendingValue { get; set; }
        public IEnumerable<MonthlyBookingsDto> BookingsByMonth { get; set; } = new List<MonthlyBookingsDto>();
        public IEnumerable<TopDestinationDto> TopDestinations { get; set; } = new List<TopDestinationDto>();
        public IEnumerable<AgencyResultDto> AgencyResults { get; set; } = new List<AgencyResultDto>();
    }

    public class MonthlyBookingsDto
    {
        public string Month { get; set; }
        public int Reservations { get; set; }
        public decimal Value { get; set; }
    }

    public class TopDestinationDto
    {
        public string Destination { get; set; }
        public string Country { get; set; }
        public int Reservations { get; set; }
        public int Travelers { get; set; }
    }

    public class AgencyResultDto
    {
        public string Agency { get; set; }
        public int Trips { get; set; }
        public int Reservations { get; set; }
        public decimal Revenue { get; set; }
        public double? Rating { get; set; }
        public int Reviews { get; set; }
    }

    public class UserSearchDto : PagedSearch
    {
        public string Keyword { get; set; }

        public string Role { get; set; }
    }

    public class UserListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string City { get; set; }
        public string Role { get; set; }
        public string Agency { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UseCases { get; set; }
    }

    public class UserDetailDto : UserListDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Reservations { get; set; }
        public int TripsCreated { get; set; }
        public DateTime? LockedUntil { get; set; }
        public List<int> UseCaseIds { get; set; } = new List<int>();
        public IEnumerable<UseCaseInfoDto> AllUseCases { get; set; } = new List<UseCaseInfoDto>();
        public List<int> DefaultUseCaseIds { get; set; } = new List<int>();
    }

    public class UseCaseInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateUserUseCasesDto
    {
        public int UserId { get; set; }
        public List<int> UseCaseIds { get; set; } = new List<int>();
    }

    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }

    public class SetUserActiveDto
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }

    public class AgencyAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int Trips { get; set; }
        public int TripsOnSale { get; set; }
        public int Reservations { get; set; }
        public decimal Revenue { get; set; }
        public IEnumerable<AgencyTeamMemberDto> Employees { get; set; } = new List<AgencyTeamMemberDto>();
    }

    public class CreateAgencyDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public class CreateAgencyEmployeeDto : RegisterUserDto
    {
        public int AgencyId { get; set; }
    }
}
