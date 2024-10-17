namespace Jevstafjev.TicTacToe.Server.Models;

public class Game
{
    private bool _isFirstPlayerTurn = true;

    public Game(Player player1, Player player2)
    {
        Player1 = player1;
        Player2 = player2;
    }

    public MoveResult Move(int row, int column)
    {
        if (row < 0 || column < 0 || row > Cells.GetLength(0) - 1 || column > Cells.GetLength(1) - 1)
            return MoveResult.Error("Invalid coordinates.");

        if (Cells[row, column] is not Cell.None)
            return MoveResult.Error("This cell is occupied.");

        Cells[row, column] = CurrentPlayer.Cell;

        var hasWinner = IsPlayerWin(CurrentPlayer.Cell);
        if (hasWinner)
        {
            return MoveResult.Win(CurrentPlayer);
        }

        var isDraw = IsDraw();
        if (isDraw)
        {
            return MoveResult.Draw();
        }

        _isFirstPlayerTurn = !_isFirstPlayerTurn;
        return MoveResult.Success();
    }

    public IEnumerable<Player> GetPlayers()
    {
        yield return Player1;
        yield return Player2;
    }

    private bool IsPlayerWin(Cell cell)
    {
        const int boardHeight = 3;
        for (var i = 0; i < boardHeight; i++)
            if (Cells[i, 0] == cell && Cells[i, 1] == cell && Cells[i, 2] == cell)
                return true;

        const int boardWidth = 3;
        for (var i = 0; i < boardWidth; i++)
            if (Cells[0, i] == cell && Cells[1, i] == cell && Cells[2, i] == cell)
                return true;

        if (Cells[0, 0] == cell && Cells[1, 1] == cell && Cells[2, 2] == cell)
            return true;

        if (Cells[0, 2] == cell && Cells[1, 1] == cell && Cells[2, 0] == cell)
            return true;

        return false;
    }

    private bool IsDraw()
    {
        foreach (var cell in Cells)
            if (cell is Cell.None)
                return false;

        return true;
    }

    public Player Player1 { get; }

    public Player Player2 { get; }

    public Player CurrentPlayer =>
        _isFirstPlayerTurn ? Player1 : Player2;

    public Cell[,] Cells { get; } = new Cell[3, 3];
}
