using System.Runtime.Serialization;

namespace PortfolioSmarts.Questrade.Model
{
    [DataContract]
    public class PositionDto
    {
        [DataMember(Name="symbol")]
        public string Symbol { get; set; }
        [DataMember(Name="symbolId")]
        public int SymbolId { get; set; }
        [DataMember(Name="openQuantity")]
        public double OpenQuantity { get; set; }
        [DataMember(Name="closedQuantity")]
        public double ClosedQuantity { get; set; }
        [DataMember(Name="currentMarketValue")]
        public double CurrentMarketValue { get; set; }
        [DataMember(Name="currentPrice")]
        public double CurrentPrice { get; set; }
        [DataMember(Name="averageEntryPrice")]
        public double AverageEntryPrice { get; set; }
        [DataMember(Name="closedPnL")]
        public double ClosedProfitAndLoss { get; set; }
        [DataMember(Name="openPnL")]
        public double OpenProfitAndLoss { get; set; }
        [DataMember(Name="totalCost")]
        public double TotalCost { get; set; }
        [DataMember(Name="isRealTime")]
        public bool IsRealTime { get; set; }
        [DataMember(Name="isUnderReorg")]
        public bool IsUnderReorganisation { get; set; }
    }
}