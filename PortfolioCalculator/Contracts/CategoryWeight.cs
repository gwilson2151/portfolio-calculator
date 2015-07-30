﻿using Contracts.Interfaces;

namespace Contracts
{
	public class CategoryWeight : IDomainEntity
	{
		public long Id { get; set; }
		public CategoryValue Value { get; set; }
		public decimal Weight { get; set; }
		public Security Security { get; set; }
	}
}