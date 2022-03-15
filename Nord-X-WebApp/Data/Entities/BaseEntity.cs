using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nord_X_WebApp.Data.Entities
{
    public abstract class BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }

        [Timestamp]
        public virtual byte[]? RowVersion { get; set; }

        [DisplayName("Tilføjet"), Description("Ingen beskrivelse")]
        public virtual DateTime AddedDate { get; set; }

        [DisplayName("Sidst Opdateret"), Description("Ingen beskrivelse")]
        public virtual DateTime ModifiedDate { get; set; }

        [DisplayName("Aktiv"), Description("Er dette objekt aktivt?")]
        public virtual bool? IsActive { get; set; } = false;
    }
}
