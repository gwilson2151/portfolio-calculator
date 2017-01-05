using System;

namespace BLL.Exceptions
{
	public class CategoryNotSupportedException : Exception
	{
		public CategoryNotSupportedException() {}
		public CategoryNotSupportedException(string message) : base(message) {}
		public CategoryNotSupportedException(string message, Exception innerException) : base(message, innerException) {}
	}
}
