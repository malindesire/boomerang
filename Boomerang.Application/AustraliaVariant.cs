namespace Boomerang.Application;

using Boomerang.Domain;
using Boomerang.Infrastructure;

public class AustraliaVariant : IGameVariant
{
	private readonly DeckLoader _loader;
	public string Name => "Boomerang Australia";
	public AustraliaVariant(DeckLoader loader)
	{
		_loader = loader;
	}

	public IReadOnlyList<Card> LoadDeck()
	{
		return _loader.LoadFromJson("../Boomerang.Infrastructure/cards_australia.json");
	}
}