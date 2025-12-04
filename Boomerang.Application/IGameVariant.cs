namespace Boomerang.Application;

using Boomerang.Domain;

public interface IGameVariant
{
	IReadOnlyList<Card> LoadDeck();
	string Name { get; }
}