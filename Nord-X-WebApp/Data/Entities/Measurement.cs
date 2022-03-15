using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618
namespace Nord_X_WebApp.Data.Entities
{
    [DisplayName("Måling"), Description("Ingen beskrivelse")]
    public class Measurement : BaseEntity
    {
        [DisplayName("Måling"), Description("Målings værdien")]
        public string Measure { get; set; }

        [DisplayName("Tidspunkt"), Description("Første tidspunkt registreret for måling")]
        public DateTime OriginRegTime { get; set; }

        [ForeignKey("Sensor")]
        [DisplayName("Sensor"), Description("Vælg en sensor fra listen")]
        public Guid SensorId { get; set; }
        public Sensor Sensor { get; set; }
    }
}
