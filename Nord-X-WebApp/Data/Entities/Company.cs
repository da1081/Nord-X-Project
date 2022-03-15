using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618
namespace Nord_X_WebApp.Data.Entities
{
    [DisplayName("Virksomhed"), Description("Ingen beskrivelse")]
    public class Company : BaseEntity
    {
        [Required]
        [DisplayName("CVR-Nr."), Description("Ingen beskrivelse")]
        public string? Vat { get; set; }

        [Required]
        [DisplayName("Navn"), Description("Ingen beskrivelse")]
        public string Name { get; set; }

        [DisplayName("Addresse"), Description("Ingen beskrivelse")]
        public string? Address { get; set; }

        [DisplayName("Post Nr."), Description("Ingen beskrivelse")]
        public int? ZipCode { get; set; }

        [DisplayName("By"), Description("Ingen beskrivelse")]
        public string? City { get; set; }

        [Required]
        [DisplayName("Beskrivelse"), Description("Ingen beskrivelse")]
        public string Description { get; set; }


        [DisplayName("Telefon Nr."), Description("Nummer bruges til genneral kontakt")]
        public string? Phone { get; set; }


        [DisplayName("Kontakt Email"), Description("Mail bruges til general kontakt")]
        public string? ContactMail { get; set; }


        [DisplayName("Rapport Email"), Description("Rapport kan sendes til denne email")]
        public string? ReportMail { get; set; }

        public ICollection<Sensor>? Sensors { get; set; }
    }
}
