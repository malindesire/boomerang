namespace Boomerang.Domain;

public class Card
{
	public required string Name { get; set; }
	public required string Letter { get; set; }
	public required string Region { get; set; }
	public int Number { get; set; }
	public string? Collection { get; set; }
	public string? Animal { get; set; }
	public string? Activity { get; set; }
}
