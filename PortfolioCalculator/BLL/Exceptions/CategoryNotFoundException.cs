using System;

namespace BLL.Exceptions
{
	public class CategoryNotFoundException : Exception
	{
		public CategoryNotFoundException() {}
		public CategoryNotFoundException(string message) : base(message) {}
		public CategoryNotFoundException(string message, Exception innerException) : base(message, innerException) {}
	}
}