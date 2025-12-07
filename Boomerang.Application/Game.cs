namespace Boomerang.Application;

using System;
using System.Collections.Generic;
using System.Linq;
using Boomerang.Domain;
using Boomerang.Application;

public class Game
{
	private readonly IGameVariant _variant;
	private readonly DraftingEngine _drafte;
	private readonly ScoringEngine _scoring;
	public List<Player> Players { get; } = new();

	public Game(IGameVariant variant, DraftingEngine drafte, ScoringEngine scoring)
	{
		_variant = variant;
		_drafte = drafte;
		_scoring = scoring;
	}

	public void AddPlayer(Player p) => Players.Add(p);

	// Setup deals 7 cards per player using deck from variant (random shuffle via RNG)
	public void SetupRound(Random rng)
	{
		var deck = _variant.LoadDeck().ToList();
		// shuffle
		for (int i = deck.Count - 1; i > 0; i--)
		{
			int j = rng.Next(i + 1);
			var tmp = deck[i]; deck[i] = deck[j]; deck[j] = tmp;
		}

		// deal 7 cards each
		for (int i = 0; i < Players.Count; i++) Players[i].Hand.Clear();
		int playerIndex = 0;
		while (deck.Any() && Players.Any(p => p.Hand.Count < 7))
		{
			var card = deck[0]; deck.RemoveAt(0);
			Players[playerIndex].Hand.Add(card);
			playerIndex = (playerIndex + 1) % Players.Count;
			// stop once everyone has 7
			if (Players.All(p => p.Hand.Count >= 7)) break;
		}
	}

	public void PlayMatch()
	{
		// Use fixed seed for determinism in tests; but here use random
		var rng = new Random();

		// 4 rounds
		for (int round = 1; round <= 4; round++)
		{
			Console.WriteLine($"\n=== ROUND {round} ===");
			// Clear drafts
			foreach (var p in Players)
			{
				p.Draft.Clear();
			}

			SetupRound(rng);

			// Perform drafting
			_drafte.ExecuteDraft(Players);

			// Show drafts (first is hidden to others, but we will show to owner)
			foreach (var p in Players)
			{
				Console.WriteLine($"\n{p.Name} YOUR DRAFT:");
				// show throw as hidden info to owner only
				Console.WriteLine($"Throw (hidden): {p.Draft.First().Name} (kept secret to others)");
				for (int i = 1; i < p.Draft.Count; i++)
				{
					Console.WriteLine($"Shown {i}: {p.Draft[i].Name} (Animal:{p.Draft[i].Animal} Collection:{p.Draft[i].Collection} Activity:{p.Draft[i].Activity})");
				}
			}

			// Score each player
			foreach (var p in Players)
			{
				var roundScore = _scoring.ScoreRound(p);
				p.FinalScore += roundScore;
				Console.WriteLine($"\n{p.Name} scored {roundScore} this round. Total: {p.FinalScore}");
			}
		}

		// Game end: determine winner
		var winner = Players.OrderByDescending(p => p.FinalScore).First();
		Console.WriteLine($"\n=== GAME OVER ===");
		Console.WriteLine($"Winner: {winner.Name} with {winner.FinalScore} points.");
		// tie-breaker by Throw & Catch points not separately tracked here; could be extended.
	}
}
