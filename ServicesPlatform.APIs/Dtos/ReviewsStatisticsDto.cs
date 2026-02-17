namespace ServicesPlatform.APIs.Dtos
{
    public class ReviewsStatisticsDto
    {
        public int TotalReviews { get; set; }
        public int OneStar { get; set; }
        public int TwoStar { get; set; }
        public int ThreeStar { get; set; }
        public int FourStar { get; set; }
        public int FiveStar { get; set; }
    }
}
