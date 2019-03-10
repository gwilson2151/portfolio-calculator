namespace PortfolioSmarts.Questrade.Model
{
    public class AccountDto
    {
        public string Type { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsBilling { get; set; }
        public string ClientAccountType { get; set; }
    }
}