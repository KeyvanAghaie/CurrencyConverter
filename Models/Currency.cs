using System.Runtime.Serialization;

namespace CurrencyConverter.Models
{
    [DataContract]
    public class Currency
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<CurrencyRate> Rates { get; set; }
    }
}
