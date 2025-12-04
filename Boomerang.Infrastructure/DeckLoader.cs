namespace Boomerang.Infrastructure;

using System.Text.Json;

public class DeckLoader
{
	public List<Card> LoadFromJson(string filePath)
	{
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException($"Deck file not found: {filePath}");
		}

		var json = File.ReadAllText(filePath);
		var cards = JsonSerializer.Deserialize<List<Card>>(json, new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		});

		return cards ?? new List<Card>();
	}
}