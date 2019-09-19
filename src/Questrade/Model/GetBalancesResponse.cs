using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PortfolioSmarts.Questrade.Model {
    
    [DataContract]
    public class GetBalancesResponse
    {
        [DataMember(Name="perCurrencyBalances")]
        public IEnumerable<BalanceDto> Balances { get; set; }
        [DataMember(Name="combinedBalances")]
        public IEnumerable<BalanceDto> BalancesWithOtherCurrencies { get; set; } 
        [DataMember(Name="sodPerCurrencyBalances")]
        public IEnumerable<BalanceDto> StartOfDayBalances { get; set; }
        [DataMember(Name="sodCombinedBalances")]
        public IEnumerable<BalanceDto> StartOfDayBalancesWithOtherCurrencies { get; set; }
    }
}