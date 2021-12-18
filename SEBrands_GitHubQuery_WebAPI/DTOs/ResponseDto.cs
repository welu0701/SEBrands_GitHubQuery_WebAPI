namespace SEBrands_GitHubQuery_WebAPI.DTOs
{
    public class ResponseDto
    {
        public bool incomplete_results { get; set; }
        public List<Item>? items { get; set; }
        public class Item
        {
            public string? name { get; set; }
            
        }

    }
}
