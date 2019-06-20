using ContractAuditBlockchain.ApiAccess.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContractAuditBlockchain.BusinessLogic.Models
{

    public class AdminParticipantListViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class UserLoginDataViewModel
    {
        public string ID { get; set; }

        [UIHint("Checkbox")]
        public bool Active { get; set; }
        [Required]
        [Display(Name = "Roles")]
        public ICollection<string> Roles { get; set; }

        [Required]
        [MaxLength(255)]
        public string Forename { get; set; }
        [Required]
        [MaxLength(255)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool CanEdit { get; set; }
    }

    public class AdminParticipantCreateEditViewModel
    {
        public string ID
        {
            get
            {
                return Login?.ID;
            }
        }

        public string Name
        {
            get
            {
                return Login == null ? string.Empty : Login.Forename + " " + Login.Surname;
            }
        }

        [UIHint("UserLoginData")]
        public UserLoginDataViewModel Login { get; set; }
    }

    public class DetailsForAdminViewModel
    {
        public AdminParticipantCreateEditViewModel AdminParticipant { get; set; }
        public IEnumerable<RentContractDataViewModel> ContractList { get; set; }
    }

    public class ClientParticipantListViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class ClientParticipantCreateEditViewModel
    {
        public string ID
        {
            get
            {
                return Login?.ID;
            }
        }

        public string Name
        {
            get
            {
                return Login == null ? string.Empty : Login.Forename + " " + Login.Surname;
            }
        }

        [UIHint("UserLoginData")]
        public UserLoginDataViewModel Login { get; set; }
    }

    public class DetailsForClientViewModel
    {
        public ClientParticipantCreateEditViewModel ClientParticipant { get; set; }
        public IEnumerable<RentContractDataViewModel> ContractList { get; set; }
    }
}
