namespace MVCDemo.Models.Configuration
{
    public class AppSettings
    {
        public string apiUrlRoot { get; set; }
        public string authCookieName { get; set; }
        public string authCookiePath { get; set; }
        public bool blazorDetailedErrors { get; set; }
    }
}
