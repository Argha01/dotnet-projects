namespace WebApp_UnderTheHood.Models
{
    public class JwtToken
    {
        public string access_token {  get; set; } = string.Empty;

        public DateTime expires_at { get; set; } 
    }
}
