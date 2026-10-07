using FlexTravel.Api.Core;
using FlexTravel.Application.Constants;
using FlexTravel.Tests.Infrastructure;

namespace FlexTravel.Tests
{
    public class SignInTests : IDisposable
    {
        private const string Password = "Correct123";
        private readonly TestDatabase _db = new TestDatabase();

        public SignInTests()
        {
            _db.World.Customer.Password = BCrypt.Net.BCrypt.HashPassword(Password);
            _db.Context.SaveChanges();
        }

        public void Dispose() => _db.Dispose();

        private JwtManager Jwt() => new JwtManager(_db.NewContext(), new JwtSettings
        {
            Issuer = "tests",
            SecretKey = "a-secret-key-that-is-long-enough-for-hmac-sha256",
            Minutes = 30
        });

        private string Email => _db.World.Customer.Email;

        [Fact]
        public void The_account_is_locked_after_too_many_wrong_passwords()
        {
            for (var i = 1; i < SignInRules.MaxFailedAttempts; i++)
            {
                Assert.Throws<UnauthorizedAccessException>(() => Jwt().MakeToken(Email, "wrong"));
            }

            var locked = Assert.Throws<AccountLockedException>(() => Jwt().MakeToken(Email, "wrong"));
            Assert.Contains($"{SignInRules.LockoutMinutes} minutes", locked.Message);

            Assert.Throws<AccountLockedException>(() => Jwt().MakeToken(Email, Password));
        }

        [Fact]
        public void A_successful_sign_in_resets_the_count_and_the_lock_expires()
        {
            Assert.Throws<UnauthorizedAccessException>(() => Jwt().MakeToken(Email, "wrong"));
            Assert.NotEmpty(Jwt().MakeToken(Email, Password));
            Assert.Equal(0, _db.NewContext().User.Single(u => u.Email == Email).FailedLoginAttempts);

            var context = _db.NewContext();
            context.User.Single(u => u.Email == Email).LockedUntil = DateTime.UtcNow.AddMinutes(-1);
            context.SaveChanges();

            Assert.NotEmpty(Jwt().MakeToken(Email, Password));
        }
    }
}
