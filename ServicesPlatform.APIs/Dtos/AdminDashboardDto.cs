using ServicesPlatform.Core.Entities;

namespace ServicesPlatform.APIs.Dtos
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalTechnicians { get; set; }
        public int TotalsActiveTechnicians { get; set; }
        public int OrderCounter { get; set; }
        public IReadOnlyList<OrderToReturnDto> LastOrders { get; set; }
        public IReadOnlyList<TechnicianUserDto> TopTechnicians { get; set; }
    }
}
