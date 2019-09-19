using System.Runtime.Serialization;

namespace PortfolioSmarts.Questrade.Model
{
    [DataContract]
    public class BalanceDto
    {
        [DataMember(Name="currency")]
        public string Currency { get; set; }
        [DataMember(Name="cash")]
        public double Cash { get; set; }
        [DataMember(Name="marketValue")]
        public double MarketValue { get; set; }
        [DataMember(Name="totalEquity")]
        public double TotalEquity { get; set; }
        [DataMember(Name="buyingPower")]
        public double BuyingPower { get; set; }
        [DataMember(Name="maintenanceExcess")]
        public double MaintenanceExcess { get; set; }
        [DataMember(Name="isRealTime")]
        public bool IsRealTime { get; set; }
    }
}