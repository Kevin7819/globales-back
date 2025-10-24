namespace Api_Orbis_Project.Dtos
{
    public class RegisterPushTokenDto
    {
        public string Token { get; set; } = default!;
        public string Platform { get; set; } = "android";
    }

    public class SendNotificationDto
    {
        public string? UserId { get; set; }
        public string Title { get; set; } = "Orbis";
        public string Message { get; set; } = default!;
        public Dictionary<string, object>? Data { get; set; }
    }
}