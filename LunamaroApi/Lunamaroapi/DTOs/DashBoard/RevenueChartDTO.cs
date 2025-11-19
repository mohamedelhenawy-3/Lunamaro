namespace Lunamaroapi.DTOs.DashBoard
{
    public class RevenueChartDTO
    {
        public List<string> Labels { get; set; }   // Days or Months
        public List<decimal> Values { get; set; }
    }
}
