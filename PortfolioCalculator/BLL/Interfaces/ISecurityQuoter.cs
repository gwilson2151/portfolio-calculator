using Contracts;

namespace BLL.Interfaces
{
	public interface ISecurityQuoter
	{
		decimal GetQuote(Security security);
	}
}