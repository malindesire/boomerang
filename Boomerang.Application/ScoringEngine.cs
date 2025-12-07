namespace Boomerang.Application;

using System;
using System.Collections.Generic;
using System.Linq;
using Boomerang.Domain;

public class ScoringEngine
{
	// collection values mapping (Leaves=1, Wildflowers=2, Shells=3, Souvenirs=5)
	private readonly Dictionary<string, int> _collectionValues = new()
		{
			{"Collection:Leaves", 1},
			{"Collection:Wildflowers", 2},
			{"Collection:Shells", 3},
			{"Collection:Souvenirs", 5}
		};

	private readonly Dictionary<string, int> _animalPoints = new()
		{
			{"Animal:Kangaroos", 3},
			{"Animal:Emus", 4},
			{"Animal:Wombats", 5},
			{"Animal:Koalas", 7},
			{"Animal:Platypuses", 9}
		};

	private readonly int[] _activityTable = new[] { 0, 2, 4, 7, 10, 15 }; // index = count

	// Score a single round for a player. Returns round total.
	public int ScoreRound(Player player)
	{
		int throwCatchScore = 0;
		if (player.Draft.Count >= 2)
		{
			// Throw = first element, Catch = last element
			var first = player.Draft.First();
			var last = player.Draft.Last();
			throwCatchScore = Math.Abs(first.Number - last.Number);
		}

		// Tourist sites score: unique site letters/names in this round, add only new ones (store in VisitedSites)
		int newSitePoints = 0;
		foreach (var c in player.Draft)
		{
			// use Name as site id — since JSON may not include letter separately
			if (!player.VisitedSites.Contains(c.Name))
			{
				newSitePoints++;
				player.VisitedSites.Add(c.Name);
			}
		}

		// Collections
		int collectionValue = 0;
		foreach (var c in player.Draft)
		{
			foreach (var col in c.Collection != null ? new[] { c.Collection } : Array.Empty<string>())
			{
				if (_collectionValues.TryGetValue(col, out int v))
					collectionValue += v;
			}
		}
		int collectionScore = collectionValue <= 7 ? collectionValue * 2 : collectionValue;

		// Animals: for each pair of same animal in draft, add points according to table
		int animalScore = 0;
		var animalCounts = new Dictionary<string, int>();
		foreach (var c in player.Draft)
		{
			foreach (var a in c.Animal != null ? new[] { c.Animal } : Array.Empty<string>())
			{
				animalCounts.TryGetValue(a, out int cur);
				animalCounts[a] = cur + 1;
			}
		}
		foreach (var kv in animalCounts)
		{
			var animal = kv.Key;
			var count = kv.Value;
			// pairs: floor(count/2)
			int pairs = count / 2;
			if (pairs > 0 && _animalPoints.TryGetValue(animal, out int p))
			{
				animalScore += pairs * p;
			}
		}

		// Activities: player may choose one activity per round, and each activity only once per game.
		int activityScore = 0;
		// find activities present in draft that have not been scored before
		var available = new Dictionary<string, int>();
		foreach (var c in player.Draft)
		{
			foreach (var act in c.Activity != null ? new[] { c.Activity } : Array.Empty<string>())
			{
				if (!player.ActivitiesScored.ContainsKey(act))
				{
					available.TryGetValue(act, out int cur);
					available[act] = cur + 1;
				}
			}
		}
		if (available.Any())
		{
			// If player is bot, auto-choose highest scoring activity; if human, ask
			string chosenActivity = null;
			if (player.IsBot)
			{
				chosenActivity = available.OrderByDescending(kv => _activityTable[Math.Min(kv.Value, _activityTable.Length - 1)]).First().Key;
			}
			else
			{
				Console.WriteLine($"{player.Name}, available activities to score this round:");
				var idx = 1;
				var list = available.ToList();
				foreach (var kv in list)
				{
					int cnt = kv.Value;
					int score = (cnt >= 1 && cnt <= 5) ? _activityTable[cnt] : _activityTable[_activityTable.Length - 1];
					Console.WriteLine($"{idx}: {kv.Key} (count {cnt}) -> {score} points");
					idx++;
				}
				Console.Write($"Enter number to score an activity this round, or ENTER to skip: ");
				var s = Console.ReadLine();
				if (int.TryParse(s, out int n) && n >= 1 && n <= list.Count)
					chosenActivity = list[n - 1].Key;
			}

			if (chosenActivity != null)
			{
				int cnt = available[chosenActivity];
				int scoreIdx = Math.Min(cnt, _activityTable.Length - 1);
				activityScore = _activityTable[scoreIdx];
				player.ActivitiesScored[chosenActivity] = activityScore;
				Console.WriteLine($"{player.Name} scored activity {chosenActivity} for {activityScore} points.");
			}
		}

		int roundTotal = throwCatchScore + newSitePoints + collectionScore + animalScore + activityScore + player.RegionRoundScore;
		player.RegionRoundScore = 0; // reset region round bonus - it's aggregated in player's visited regions elsewhere if needed
		return roundTotal;
	}
}
