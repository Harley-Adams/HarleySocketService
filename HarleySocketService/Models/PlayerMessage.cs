namespace HarleySocketService.Models
{
    public class PlayerMessage
    {
        public long timeStamp { get; set; }
        public PlayerMessageTypeEnum messageType { get; set; }
        public string messageBody { get; set; }
    }
}
