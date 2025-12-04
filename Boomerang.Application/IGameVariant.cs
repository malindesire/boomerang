namespace Boomerang.Application;

public interface IGameVariant
{
	IReadOnlyList<Card> LoadDeck();
	string Name { get; }
}