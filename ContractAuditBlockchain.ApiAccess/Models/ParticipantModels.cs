using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.ApiAccess.Models
{
    public abstract class BaseModel
    {
        public abstract string _Class { get; }
        public string ID { get; set; }
    }

    public abstract class ParticipantModel : BaseModel
    {
        public string Name { get; set; }
    }
    public class AdminParticipantModel : ParticipantModel
    {
        public override string _Class => "HubAdmin";
    }

    public class ClientParticipantModel : ParticipantModel
    {
        public override string _Class => "HubClient";
    }


    public class BaseModelResponse
    {

        public string GetIDFromAttributedID(string attributedID)
        {
            return attributedID.Substring(attributedID.IndexOf("#") + 1);
        }
    }

    public class ParticipantModelResponse : BaseModelResponse
    {
        //"$class": "...",
        public string participantId { get; set; }
        public string name { get; set; }
    }
}
