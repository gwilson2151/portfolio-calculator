using PortfolioSmarts.Domain.Enumerations;
using PortfolioSmarts.Domain.Interfaces;

namespace PortfolioSmarts.Domain
{
	public class Balance : IDomainEntity
	{
		public long Id { get; set; }
		public decimal Amount { get; set;}
		public Currency Currency { get; set;}

		public Balance() {}
	}
}
