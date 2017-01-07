using Contracts;

namespace BLL.Interfaces
{
	public interface ISecurityRepository
	{
		Security Add(Security security);
		Security GetOrCreate(string exchange, string symbol);
	}
}