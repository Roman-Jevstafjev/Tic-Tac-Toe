namespace Jevstafjev.TicTacToe.Server.Models
{
    public class Player
    {
        public string ConnectionId { get; set; } = null!;

        public bool LookingForOpponent { get; set; }

        public Cell Cell { get; set; }
    }
}
