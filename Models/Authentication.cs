namespace ChatGptProxyApi.Models
{
  public class UserRegistrationDto
  {
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
  }

  public class UserLoginDto
  {
    public string Username { get; set; }
    public string Password { get; set; }
  }
}
