namespace Wirin.Api.Dtos.Response;

public class LoginResponse
{
    public string token { get; set; }
    public DateTime expiration { get; set; }
    public string userId { get; set; }
}
