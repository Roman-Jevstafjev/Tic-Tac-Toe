using Jevstafjev.TicTacToe.Server.Factories;
using Jevstafjev.TicTacToe.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR()
    .AddNewtonsoftJsonProtocol();

builder.Services.AddTransient<GameFactory>();

var app = builder.Build();

app.MapHub<CommunicationHub>("/communication");

app.MapGet("/", () => "Tic Tac Toe");

app.Run();
