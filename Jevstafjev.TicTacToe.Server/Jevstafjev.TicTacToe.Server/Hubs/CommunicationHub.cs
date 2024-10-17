using Jevstafjev.TicTacToe.Server.Factories;
using Jevstafjev.TicTacToe.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace Jevstafjev.TicTacToe.Server.Hubs;

public class CommunicationHub(GameFactory gameFactory)
    : Hub<ICommunicationHub>
{
    private static readonly List<Game> _games = new();
    private static readonly List<Player> _players = new();

    public override Task OnConnectedAsync()
    {
        var player = new Player
        {
            ConnectionId = Context.ConnectionId
        };

        _players.Add(player);
        return Task.CompletedTask;
    }

    public async Task FindGameAsync()
    {
        var player = _players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (player is null)
        {
            await Clients.Caller.NotifyErrorAsync("Player is not found");
            return;
        }

        if (player.LookingForOpponent is true)
        {
            await Clients.Caller.NotifyErrorAsync("The player is already looking for a game");
            return;
        }

        if (_games.Any(x => x.Player1 == player || x.Player2 == player))
        {
            await Clients.Caller.NotifyErrorAsync("The player is already in the game.");
            return;
        }
        
        var opponent = _players.FirstOrDefault(x => x.LookingForOpponent is true);
        if (opponent is null)
        {
            player.LookingForOpponent = true;
            return;
        }

        opponent.LookingForOpponent = false;

        var game = gameFactory.CreateGame(player, opponent);
        _games.Add(game);

        await Clients.Clients(game.GetPlayers().Select(x => x.ConnectionId)).NotifyGameStartedAsync();
        await Clients.Clients(game.GetPlayers().Select(x => x.ConnectionId)).UpdateBoardAsync(game.Cells);
        await Clients.Client(game.CurrentPlayer.ConnectionId).NotifyMoveAsync();
    }

    public async Task Move(int row, int column)
    {
        var game = _games.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId
                                           || x.Player2.ConnectionId == Context.ConnectionId);
        if (game is null)
        {
            await Clients.Caller.NotifyErrorAsync("Game is not found");
            return;
        }

        var result = game.Move(row, column);
        if (!result.IsSuccess)
        {
            await Clients.Caller.NotifyInvalidMoveAsync(result.ErrorMessage!);
            return;
        }

        await Clients.Clients(game.GetPlayers().Select(x => x.ConnectionId)).UpdateBoardAsync(game.Cells);

        if (result.HasWinner)
        {
            await Clients.Clients(game.GetPlayers().Select(x => x.ConnectionId)).AnnounceWinnerAsync(result.Winner!.Cell);
            _games.Remove(game);

            return;
        }

        if (result.IsDraw)
        {
            await Clients.Clients(game.GetPlayers().Select(x => x.ConnectionId)).AnnounceDrawAsync();
            _games.Remove(game);

            return;
        }

        await Clients.Client(game.CurrentPlayer.ConnectionId).NotifyMoveAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var player = _players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (player is null)
        {
            return Task.CompletedTask;
        }

        var game = _games.FirstOrDefault(x => x.Player1 == player || x.Player2 == player);
        if (game is not null)
        {
            _games.Remove(game);

            var opponent = game.Player1 == player ? game.Player2 : game.Player1;
            Clients.Client(opponent.ConnectionId).NotifyOpponentDisconnectedAsync();       
        }

        _players.Remove(player);
        return Task.CompletedTask;
    }
}
