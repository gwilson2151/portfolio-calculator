using System;

namespace BLL.Exceptions
{
	public class SecurityNotFoundException : Exception
	{
		public SecurityNotFoundException() {}
		public SecurityNotFoundException(string message) : base(message) {}
		public SecurityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
	}
}