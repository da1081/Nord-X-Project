using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618
namespace Nord_X_WebApp.Data.Entities
{
    [DisplayName("Data Type"), Description("Ingen beskrivelse")]
    public class MeasurementType : BaseEntity
    {
        [DisplayName("Navn"), Description("Ingen beskrivelse")]
        public string Name { get; set; }

        [DisplayName("Alternativt navn"), Description("Eventuelt et alternativt navn for typen brugt på tværs af systemer")]
        public string? Translation { get; set; }

        [DisplayName("Beskrivelse"), Description("Ingen beskrivelse")]
        public string? Description { get; set; }

        [EnumDataType(typeof(DataType))]
        [DisplayName("Data Typen"), Description("string (tekst), int (helt tal), double (komma tal)")]
        public DataType Type { get; set; }

        public ICollection<Sensor>? Sensors { get; set; }
    }

    public enum DataType
    {
        String = 1, Integer = 2, Double = 3
    }

    public enum Unit
    {
        Watt = 1, Gram = 2, Liters = 3
    }
}
