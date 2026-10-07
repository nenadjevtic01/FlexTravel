using System.Security.Cryptography;

namespace FlexTravel.Implementation.Booking
{
    public static class ReservationCodes
    {
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public static string New()
        {
            var chars = new char[6];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
            }

            return $"TA-{new string(chars)}";
        }
    }
}
