namespace FlexTravel.Domain
{
    public class User : Entity
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        public string Avatar { get; set; }

        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public virtual ICollection<UserUseCase> UseCases { get; set; } = new List<UserUseCase>();
    }

    public class AgencyEmployee : User
    {
        public int AgencyId { get; set; }

        public virtual Agency Agency { get; set; }
    }

    public class RegularUser : User
    {
        public virtual ICollection<UserFavorites> Favorites { get; set; } = new List<UserFavorites>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<WaitlistEntry> Waitlist { get; set; } = new List<WaitlistEntry>();
        public virtual ICollection<PriceAlert> PriceAlerts { get; set; } = new List<PriceAlert>();
    }

    public class Admin : User
    {
    }
}
