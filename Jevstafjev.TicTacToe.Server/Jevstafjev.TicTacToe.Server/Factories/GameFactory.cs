using Jevstafjev.TicTacToe.Server.Models;

namespace Jevstafjev.TicTacToe.Server.Factories
{
    public class GameFactory
    {
        public Game CreateGame(Player player1, Player player2)
        {
            player1.Cell = Cell.X;
            player2.Cell = Cell.O;

            return new Game(player1, player2);
        }
    }
}
