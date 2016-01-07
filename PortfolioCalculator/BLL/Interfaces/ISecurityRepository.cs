using Contracts;

namespace BLL.Interfaces
{
	public interface ISecurityRepository
	{
		Security GetBySymbol(string symbol);
	}
}