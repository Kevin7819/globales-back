namespace Api_Orbis_Project.Dtos
{
    public class SendNotificationDto
    {
        public string Title { get; set; } = "Orbis Notification";
        public string Message { get; set; } = "Mensaje de prueba";
        public object? Data { get; set; }
    }
}