using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618
namespace Nord_X_WebApp.Data.Entities
{
    [DisplayName("Data Kilde"), Description("Ingen beskrivelse")]
    public class Sensor : BaseEntity
    {
        [DisplayName("Navn"), Description("Ingen beskrivelse")]
        public string Name { get; set; }

        [DisplayName("Beskrivelse"), Description("Ingen beskrivelse")]
        public string? Description { get; set; }

        [DisplayName("URL Endpoint"), Description("Den fulde addresse (URL) til sensor endpoint inklusiv nødvendige parametere")]
        public string Endpoint { get; set; }

        [DisplayName("Opdatering fejler"), Description("Opdatering fejler")]
        public bool? IsFailing { get; set; } = false;

        [ForeignKey("Company")]
        [DisplayName("Virksomhed"), Description("Vælg en virksomhed fra listen")]
        public Guid CompanyId { get; set; }
        public Company? Company { get; set; }

        [ForeignKey("MeasurementType")]
        [DisplayName("Målingstype"), Description("Vælg en målings type fra listen")]
        public Guid MeasurementTypeId { get; set; }
        public MeasurementType? MeasurementType { get; set; }

        public ICollection<Measurement>? DataPoints { get; set; }
    }
}
