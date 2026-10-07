using FlexTravel.Application.Constants;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using System.Security.Cryptography;
using System.Text;

namespace FlexTravel.Implementation.Account
{
    public class SessionStore
    {
        public static readonly TimeSpan ReuseGrace = TimeSpan.FromSeconds(20);

        private readonly FlexTravelContext _context;

        public SessionStore(FlexTravelContext context)
        {
            _context = context;
        }

        public string Issue(int userId, bool rememberMe, DateTime now)
        {
            var token = NewToken();
            _context.RefreshToken.Add(new RefreshToken
            {
                UserId = userId,
                TokenHash = Hash(token),
                CreatedAt = now,
                ExpiresAt = now.AddDays(rememberMe ? SessionRules.RememberMeDays : SessionRules.RefreshTokenDays),
                RememberMe = rememberMe
            });
            _context.SaveChanges();
            return token;
        }

        public (int UserId, string Token, bool RememberMe)? Rotate(string token, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var hash = Hash(token);
            var stored = _context.RefreshToken.FirstOrDefault(t => t.TokenHash == hash);
            if (stored == null)
            {
                return null;
            }

            if (stored.RevokedAt.HasValue)
            {
                if (stored.WasReplaced && now - stored.RevokedAt.Value > ReuseGrace)
                {
                    RevokeAll(_context, stored.UserId);
                    _context.SaveChanges();
                }

                return null;
            }

            if (stored.ExpiresAt <= now || !_context.User.Any(u => u.UserId == stored.UserId && u.IsActive))
            {
                return null;
            }

            stored.RevokedAt = now;
            stored.WasReplaced = true;
            var next = Issue(stored.UserId, stored.RememberMe, now);
            return (stored.UserId, next, stored.RememberMe);
        }

        public void Revoke(string token, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            var hash = Hash(token);
            var stored = _context.RefreshToken.FirstOrDefault(t => t.TokenHash == hash && t.RevokedAt == null);
            if (stored != null)
            {
                stored.RevokedAt = now;
                _context.SaveChanges();
            }
        }

        public static int RevokeAll(FlexTravelContext context, int userId, string keepToken = null)
        {
            var keep = string.IsNullOrWhiteSpace(keepToken) ? null : Hash(keepToken);
            var tokens = context.RefreshToken
                .Where(t => t.UserId == userId && t.RevokedAt == null && t.TokenHash != keep)
                .ToList();

            foreach (var t in tokens)
            {
                t.RevokedAt = DateTime.UtcNow;
            }

            return tokens.Count;
        }

        public static int Purge(FlexTravelContext context, DateTime now)
        {
            var old = context.RefreshToken.Where(t => t.ExpiresAt < now.AddDays(-7)).ToList();
            context.RefreshToken.RemoveRange(old);
            context.SaveChanges();
            return old.Count;
        }

        public static string Hash(string token) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();

        private static string NewToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}
