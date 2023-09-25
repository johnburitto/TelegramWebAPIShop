namespace Infrastructure.StateMachine
{
    public class State
    {
        public long UserTelegramId { get; set; }
        public string? CurrentState { get; set; }
        public object? StateObject { get; set; }
    }
}
