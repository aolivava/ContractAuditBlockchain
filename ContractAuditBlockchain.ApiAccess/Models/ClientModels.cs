using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.ApiAccess.Models
{
    public class ClientModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class ClientModelResponse
    {
        //"$class": "org.example.basic.HubClient",
        public string participantId { get; set; }
        public string name { get; set; }
    }
}
