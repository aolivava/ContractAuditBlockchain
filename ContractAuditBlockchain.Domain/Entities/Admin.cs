using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractAuditBlockchain.Domain
{
    [Table("Admins")]
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(128)]
        public string Id { get; set; }

        // Admin to ApplicationUser is 1 to 1 . Handled in fluent API
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
