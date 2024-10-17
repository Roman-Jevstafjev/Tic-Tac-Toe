using Jevstafjev.TicTacToe.Server.Models;

namespace Jevstafjev.TicTacToe.Server.Hubs;

public interface ICommunicationHub
{
    Task NotifyGameStartedAsync();

    Task UpdateBoardAsync(Cell[,] cells);

    Task NotifyMoveAsync();

    Task AnnounceWinnerAsync(Cell winnerCell);

    Task AnnounceDrawAsync();

    Task NotifyInvalidMoveAsync(string message);

    Task NotifyOpponentDisconnectedAsync();

    Task NotifyErrorAsync(string message);
}
