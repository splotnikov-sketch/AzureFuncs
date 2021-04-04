using System;
using Newtonsoft.Json;

namespace AzureFunctions.Commands.Cosmos.Entities
{
    public class NumberEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public long Number { get; set; }
        public bool IsPrime { get; set; }
        public string CalculationTime { get; set; }
        public DateTime CreatedDate { get; set; }
     
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
