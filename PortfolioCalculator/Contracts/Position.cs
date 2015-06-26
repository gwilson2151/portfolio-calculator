namespace Contracts
{
	public class Position
	{
		public int Id { get; set; }
		public Account Account { get; set; }
		public Security Security { get; set; }
		public int Count { get; set; }
	}
}