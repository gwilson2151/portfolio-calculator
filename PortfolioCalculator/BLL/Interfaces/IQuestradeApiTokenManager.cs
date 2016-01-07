using QuestradeAPI;

namespace BLL.Interfaces
{
	public interface IQuestradeApiTokenManager
	{
		AuthenticationInfoImplementation GetAuthToken();
	}
}