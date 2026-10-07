using FlexTravel.Application.Constants;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlexTravel.Api.Core
{
    public class TokenPair
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class JwtManager
    {
        private readonly FlexTravelContext _context;
        private readonly JwtSettings _settings;
        private readonly SessionStore _sessions;

        public JwtManager(FlexTravelContext context, JwtSettings settings)
        {
            _context = context;
            _settings = settings;
            _sessions = new SessionStore(context);
        }

        public TokenPair SignIn(string email, string password, bool rememberMe)
        {
            var token = MakeToken(email, password);
            var userId = _context.User.Where(u => u.Email == email).Select(u => u.UserId).First();
            return Pair(token, _sessions.Issue(userId, rememberMe, DateTime.UtcNow));
        }

        public TokenPair Refresh(string refreshToken)
        {
            var rotated = _sessions.Rotate(refreshToken, DateTime.UtcNow);
            return rotated == null ? null : Pair(MakeTokenFor(rotated.Value.UserId), rotated.Value.Token);
        }

        public void SignOut(string refreshToken) => _sessions.Revoke(refreshToken, DateTime.UtcNow);

        private TokenPair Pair(string token, string refreshToken) => new TokenPair
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.Minutes)
        };

        public string MakeToken(string email, string password)
        {
            var user = _context.User.Include(x => x.UseCases).FirstOrDefault(x => x.Email == email);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException();
            }

            var now = DateTime.UtcNow;
            if (user.LockedUntil > now)
            {
                throw new AccountLockedException(user.LockedUntil.Value - now);
            }

            var valid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!valid)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= SignInRules.MaxFailedAttempts)
                {
                    user.FailedLoginAttempts = 0;
                    user.LockedUntil = now.AddMinutes(SignInRules.LockoutMinutes);
                    _context.SaveChanges();
                    throw new AccountLockedException(TimeSpan.FromMinutes(SignInRules.LockoutMinutes));
                }

                _context.SaveChanges();
                throw new UnauthorizedAccessException();
            }

            if (user.FailedLoginAttempts > 0 || user.LockedUntil != null)
            {
                user.FailedLoginAttempts = 0;
                user.LockedUntil = null;
                _context.SaveChanges();
            }

            return MakeToken(user);
        }

        public string MakeTokenFor(int userId)
        {
            var user = _context.User.Include(x => x.UseCases).First(x => x.UserId == userId);
            return MakeToken(user);
        }

        private string MakeToken(Domain.User user)
        {
            var actor = new JwtUser
            {
                Id = user.UserId,
                UseCaseIds = user.UseCases.Select(x => x.UseCaseId).ToList(),
                Identity = user.FirstName + ' ' + user.LastName,
                Email = user.Email
            };

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64, _settings.Issuer),
                new Claim("UserId", actor.Id.ToString(),ClaimValueTypes.String, _settings.Issuer),
                new Claim("UseCases",JsonConvert.SerializeObject(actor.UseCaseIds)),
                new Claim("Email", user.Email),
                new Claim("Identity", actor.Identity)
            };

            if (!string.IsNullOrEmpty(user.Avatar))
            {
                claims.Add(new Claim("Avatar", user.Avatar));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: "Any",
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_settings.Minutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

namespace FlexTravel.Api.Core
{
    public class AccountLockedException : Exception
    {
        public AccountLockedException(TimeSpan remaining)
            : base($"Too many wrong passwords. Try again in {Math.Max(1, (int)Math.Ceiling(remaining.TotalMinutes))} minutes.")
        {
        }
    }
}
