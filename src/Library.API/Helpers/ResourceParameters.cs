namespace Library.API.Helpers
{
    public class ResourceParameters
    {
        private int _pageSize = 10;
        const int maxPageSize = 20;

        public int PageNumber { get; set; } = 1;
        public string OrderBy { get; set; }
        public string Fields { get; set; }

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
