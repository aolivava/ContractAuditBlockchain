using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractAuditBlockchain.Domain
{
    [Table("Contracts")]
    public class Contract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(128)]
        public string Id { get; set; }

        public string ProviderId { get; set; }
        public string ClientId { get; set; }

        [ForeignKey(nameof(ProviderId))]
        public virtual Admin Provider { get; set; }
        [ForeignKey(nameof(ClientId))]
        public virtual Client Client { get; set; }
    }
}
