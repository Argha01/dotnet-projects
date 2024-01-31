namespace WebApp_UnderTheHood
{
    public interface ISample
    {
        public string CurrentTime { get; set; }
    }

    public class Sample : ISample
    {
        private readonly ILogger<Sample> logger;
        public Sample(ILogger<Sample> logger)
        {
            this.logger = logger;
            this.logger.LogInformation("Sample");
        }
        public string CurrentTime { get; set; } = DateTime.Now.ToString();
    }

}
