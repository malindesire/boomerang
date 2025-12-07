namespace Boomerang.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class Player
{
	private readonly Random _rnd = new Random();

	public string Name { get; set; }
	public bool IsBot { get; set; }
	public List<Card> Hand { get; set; } = new();
	public List<Card> Draft { get; set; } = new();
	public HashSet<string> VisitedSites { get; set; } = new();
	public Dictionary<string, int> ActivitiesScored { get; set; } = new();
	public int RegionRoundScore { get; set; } = 0;
	public int FinalScore { get; set; } = 0;

	public Player(string name, bool isBot)
	{
		Name = name;
		IsBot = isBot;
	}

	// Choose a card from current hand. If human, ask ui; if bot, random.
	public Card ChooseCardFromHand(bool interactive = true)
	{
		if (!IsBot && interactive)
		{
			// Ask UI to choose card
			Console.WriteLine($"\n{this.Name}, your hand:");
			for (int i = 0; i < Hand.Count; i++)
			{
				var card = Hand[i];
				Console.WriteLine($"{i + 1}: {card.Name} ({card.Region}) #:{card.Number} Animal:{card.Animal} Collection:{card.Collection} Activity:{card.Activity}");
			}
			while (true)
			{
				Console.Write($"Type the number (1-{Hand.Count}) of the card you select: ");
				var s = Console.ReadLine();
				if (int.TryParse(s, out int n) && n >= 1 && n <= Hand.Count)
				{
					var card = Hand[n - 1];
					Hand.RemoveAt(n - 1);
					return card;
				}
				Console.WriteLine("Invalid input, try again.");
			}
		}
		else
		{
			// Bot chooses random card
			int index = _rnd.Next(Hand.Count);
			Hand.RemoveAt(index);
			return Hand[index];
		}
	}

}
