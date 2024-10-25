using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataEntities;


public record ProductRanking 
{ 
    public required int ProductId { get; set; }
    public int Score { get; set; }
};
