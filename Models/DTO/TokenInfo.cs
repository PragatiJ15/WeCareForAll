namespace WeCareForAll.Models.DTO
{
    public class TokenInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string RefereshToken { get;set; }
        public DateTime RefereshTokenExpiry { get; set; }
    }
}
