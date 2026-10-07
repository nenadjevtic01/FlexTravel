namespace FlexTravel.Application.Dto
{
    public class TokenRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }
}
