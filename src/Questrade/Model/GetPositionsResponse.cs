using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PortfolioSmarts.Questrade.Model
{
	[DataContract]
	public class GetPositionsResponse
	{
		[DataMember(Name="positions")]
		public IEnumerable<PositionDto> Positions { get; set; }
	}
}
