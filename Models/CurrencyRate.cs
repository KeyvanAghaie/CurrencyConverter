using System.Runtime.Serialization;

namespace CurrencyConverter.Models
{
    [DataContract]
    public class CurrencyRate
    {
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public decimal Rate { get; set; }
    }
}
