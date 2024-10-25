using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataEntities;


public class ProductRanking
{
    [JsonPropertyName("product")]
    public Product product { get; set; }
    [JsonPropertyName("rank")]
    public int rank { get; set; }
}

[JsonSerializable(typeof(List<ProductRanking>))]
public sealed partial class ProductRankingSerializerContext : JsonSerializerContext
{
}
