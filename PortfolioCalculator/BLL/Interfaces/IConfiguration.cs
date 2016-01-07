namespace BLL.Interfaces
{
	public interface IConfiguration
	{
		string DataDirectoryPath { get; }
		string QuestradeApiKeyFileName { get; }
	}
}