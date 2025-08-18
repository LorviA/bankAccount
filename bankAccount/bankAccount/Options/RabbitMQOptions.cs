namespace bankAccount.Options
{
    // ReSharper disable once InconsistentNaming
    public class RabbitMQOptions
    {
        public string Host { get; set; } = "rabbitMQ";
        public int Port { get; set; } = 5672;
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string Exchange { get; set; } = "account.events";
    }
}
