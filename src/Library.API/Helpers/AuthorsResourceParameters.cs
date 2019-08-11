namespace Library.API.Helpers
{
    public class AuthorsResourceParameters : ResourceParameters
    {
        private string _genre;
        private string _search;

        public string OrderBy { get; set; } = "Name";

        public string Genre
        {
            get
            {
                if (string.IsNullOrEmpty(_genre))
                {
                    return null;
                }

                return _genre.Trim().ToLowerInvariant();
            }
            set
            {
                _genre = value;
            }
        }

        public string Search
        {
            get
            {
                if (string.IsNullOrEmpty(_search))
                {
                    return null;
                }

                return _search.Trim().ToLowerInvariant();
            }
            set
            {
                _search = value;
            }
        }
    }
}
