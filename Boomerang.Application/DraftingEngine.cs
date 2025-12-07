namespace Boomerang.Application;

using System;
using System.Collections.Generic;
using System.Linq;
using Boomerang.Domain;

public class DraftingEngine
{
	// Perform one round of drafting for a list of players. After call:
	// - each player's Draft list will have 7 cards: [throw, shown1, shown2..., catch]
	// - players' hands will be empty.
	public void ExecuteDraft(List<Player> players, bool interactiveHuman = true)
	{
		int n = players.Count;
		if (n < 2 || n > 4) throw new ArgumentException("Players must be between 2 and 4");

		// 1) Each player selects a Throw card (hidden)
		foreach (var p in players)
		{
			Console.WriteLine($"\n{p.Name} -- Select your THROW card (hidden).");
			var selected = p.ChooseCardFromHand(interactiveHuman);
			p.Draft.Add(selected); // first element is throw (hidden)
								   // remaining in p.Hand are passed in next steps
		}

		// 2) Prepare hands lists (should be 6 cards per player)
		var hands = players.Select(p => new List<Card>(p.Hand)).ToList();

		// Clean player hands; we'll use local hands for passing
		foreach (var p in players) p.Hand.Clear();

		// Loop: while each hand size > 1 => pass then choose one and show
		while (hands.Any(h => h.Count > 1))
		{
			// Pass remaining cards to next player (last -> first)
			var passed = new List<List<Card>>(new List<Card>[n]);
			for (int i = 0; i < n; i++)
			{
				var receiver = (i + 1) % n;
				passed[receiver] = new List<Card>(hands[i]);
			}
			hands = passed;

			// For each player: choose one card from hands[i], show it (add to Draft)
			for (int i = 0; i < n; i++)
			{
				var player = players[i];
				var hand = hands[i];
				if (hand.Count == 0) continue;
				if (hand.Count == 1)
				{
					// if only 1, we will handle catch later
					continue;
				}
				// temporary move the cards into player's Hand to reuse ChooseCardFromHand UI
				player.Hand.AddRange(hand);
				Console.WriteLine($"\n{player.Name} -- Select one card from the passed hand to SHOW:");
				var chosen = player.ChooseCardFromHand(!player.IsBot); // interactive for humans
				player.Draft.Add(chosen); // shown card
										  // after choosing, remaining cards to next hand:
				hands[i] = new List<Card>(player.Hand); // after ChooseCardFromHand, player's Hand contains remaining cards
				player.Hand.Clear();
			}
		}

		// Now each hands[i] should have exactly 1 card (the final candidate)
		// Rule 9: The final card is passed to the previous player and added to that player's shown cards as CATCH
		var finalPassed = new List<Card>(new Card[n]);
		for (int i = 0; i < n; i++)
		{
			// hand i contains one card; it is passed to previous player
			var prev = (i - 1 + n) % n;
			if (hands[i].Count == 1)
				finalPassed[prev] = hands[i].First();
			else
				finalPassed[prev] = null;
		}

		// Add finalPassed to each player's draft as catch
		for (int i = 0; i < n; i++)
		{
			if (finalPassed[i] != null)
				players[i].Draft.Add(finalPassed[i]);
		}

		// Ensure players hands are empty at end
		foreach (var p in players) p.Hand.Clear();
	}
}
