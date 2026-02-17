namespace ServicesPlatform.APIs.Dtos
{
    public class TechnicianDashboard
    {
        public int TotalActiveOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalNewOrders { get; set; }
        public int CountOfPoints { get; set; }
        public double AverageRating { get; set; }
        public IReadOnlyList<OrderToReturnDto> NewOrders { get; set; }
        public IReadOnlyList<OrderToReturnDto> ActiveOrders { get; set; }
    }
}
