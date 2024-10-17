namespace Jevstafjev.TicTacToe.Server.Models;

public class MoveResult
{
    private MoveResult(Player? winner, bool isDraw, string? errorMessage)
    {
        Winner = winner;
        IsDraw = isDraw;
        ErrorMessage = errorMessage;
    }

    public static MoveResult Success()
        => new MoveResult(null, false, null);

    public static MoveResult Win(Player winner)
        => new MoveResult(winner, false, null);

    public static MoveResult Draw()
        => new MoveResult(null, true, null);

    public static MoveResult Error(string errorMessage)
        => new MoveResult(null, false, errorMessage);

    public Player? Winner { get; }

    public bool IsDraw { get; }

    public string? ErrorMessage { get; }

    public bool HasWinner => Winner is not null;

    public bool IsSuccess => ErrorMessage is null;
}
