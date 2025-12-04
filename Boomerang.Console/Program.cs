using Boomerang.Infrastructure;
using Boomerang.Application;

var loader = new DeckLoader();
var variant = new AustraliaVariant(loader);
var deck = variant.LoadDeck();

Console.WriteLine($"Loaded {deck.Count} cards for variant: {variant.Name}");

foreach (var card in deck)
{
	Console.WriteLine($"- {card.Name}");
}