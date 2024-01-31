namespace WebApp
{
    public class SmtpSettings
    {
        public string sender { get; set; } = string.Empty;
        
        public string password { get; set; } = string.Empty;

        public string host { get; set; } = string.Empty;

        public int port { get; set; }

    }
}
