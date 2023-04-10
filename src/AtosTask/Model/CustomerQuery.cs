using System.Text.Json.Serialization;

namespace AtosTask.Model
{
    public class CustomerQuery
    {        
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CustomerOrderBy OrderBy { get; set; } = CustomerOrderBy.Id;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderByDirection Direction { get; set; } = Model.OrderByDirection.Ascending;
    }
}
