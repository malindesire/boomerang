namespace Boomerang.Domain;

public interface IGameVariant
{
	string Name { get; }
	IReadOnlyList<Card> LoadDeck();
}
