using System.Text.Json.Serialization;

namespace PaymentDemo.Manage
{
    public class CategoryViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}