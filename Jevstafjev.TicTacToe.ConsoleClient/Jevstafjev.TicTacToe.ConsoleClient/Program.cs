using Jevstafjev.TicTacToe.ConsoleClient;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:5000/communication")
    .AddNewtonsoftJsonProtocol()
    .Build();

connection.On("NotifyGameStartedAsync", () =>
{
    Console.WriteLine("Game started!");
});

connection.On<Cell[,]>("UpdateBoardAsync", (cells) =>
{
    Console.Clear();

    for (var i = 0; i < cells.GetLength(0); i++)
    {
        for (var j = 0; j < cells.GetLength(1); j++)
        {
            var cell = cells[i, j];
            if (cell is Cell.None)
            {
                Console.Write('#');
                continue;
            }

            Console.Write(cell);
        }

        Console.WriteLine();
    }
});

connection.On("NotifyMoveAsync", MoveAsync);

connection.On<string>("NotifyInvalidMoveAsync", async (errorMessage) =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Invalid move");
    Console.WriteLine(errorMessage);
    Console.ResetColor();

    await MoveAsync();
});

connection.On("AnnounceWinnerAsync", (Cell cell) =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Player {cell} wins!");
    Console.ResetColor();
});

connection.On("AnnounceDrawAsync", () =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"It's a draw!");
    Console.ResetColor();
});

connection.On("NotifyOpponentDisconnectedAsync", () =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Opponent disconnected. The game is over.");
    Console.ResetColor();
});

connection.On<string>("NotifyErrorAsync", (errorMessage) =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error");
    Console.WriteLine(errorMessage);
    Console.ResetColor();
});

await connection.StartAsync();

await connection.SendAsync("FindGameAsync");
Console.WriteLine("Looking for an enemy..");

await Task.Delay(-1);

async Task MoveAsync()
{
    Console.WriteLine("Enter row(1-3):");
    var row = Convert.ToInt32(Console.ReadLine()) - 1;

    Console.WriteLine("Enter column(1-3):");
    var column = Convert.ToInt32(Console.ReadLine()) - 1;

    await connection.SendAsync("Move", row, column);
}