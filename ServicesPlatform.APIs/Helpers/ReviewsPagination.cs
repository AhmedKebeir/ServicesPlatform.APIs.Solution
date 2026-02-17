namespace ServicesPlatform.APIs.Helpers
{
    public class ReviewsPagination<T>
    {
        public ReviewsPagination(int pageIndex, int pageSize, IReadOnlyList<T> data, int count,ReviewsStatistics reviewsStatistics)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
            ReviewsStatistics = reviewsStatistics; 
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; }
        public ReviewsStatistics ReviewsStatistics { get; set; }
    }
}
