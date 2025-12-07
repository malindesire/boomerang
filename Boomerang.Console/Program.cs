using System;
using System.Collections.Generic;
using Boomerang.Infrastructure;
using Boomerang.Application;
using Boomerang.Domain;

try
{
	Console.WriteLine("=== Boomerang Console (Demo) ===");

	// Setup variant and loader
	var loader = new DeckLoader();
	var variant = new AustraliaVariant(loader);

	// Engines
	var drafte = new DraftingEngine();
	var scoring = new ScoringEngine();

	var game = new Game(variant, drafte, scoring);

	// Ask how many players and bots
	Console.Write("Number of human players (1-4): ");
	var s = Console.ReadLine();
	int humans = 1;
	if (!int.TryParse(s, out humans) || humans < 1) humans = 1;
	Console.Write("Number of bots (0-3): ");
	s = Console.ReadLine();
	int bots = 0;
	if (!int.TryParse(s, out bots) || bots < 0) bots = 0;

	// Ensure total 2-4 players
	int total = humans + bots;
	if (total < 2) { bots = 1; total = humans + bots; Console.WriteLine("Added 1 bot to reach minimum players."); }
	if (total > 4) { bots = 4 - humans; Console.WriteLine($"Adjusted bots to {bots} to reach max 4 players."); }

	for (int i = 0; i < humans; i++)
	{
		// var name = $"Player{i + 1}";
		// Console.Write($"Enter name for human player #{i + 1}: ");
		// var name = Console.ReadLine();
		// if (string.IsNullOrWhiteSpace(name)) name = $"Player{i + 1}";
		game.AddPlayer(new Player($"Player{i + 1}", isBot: false));
	}
	for (int b = 0; b < bots; b++)
	{
		game.AddPlayer(new Player($"Bot{b + 1}", isBot: true));
	}

	// Play match
	game.PlayMatch();
}
catch (Exception ex)
{
	Console.WriteLine($"Fatal error: {ex.Message}\n{ex.StackTrace}");
}
Console.WriteLine("Press ENTER to exit.");
Console.ReadLine();