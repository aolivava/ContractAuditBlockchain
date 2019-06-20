using System.ComponentModel.DataAnnotations;

namespace ContractAuditBlockchain.ClientApp.Models
{
    public class ResetPasswordViewModel
    {
        public string ErrorMessage { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string PasswordResetKey { get; set; }

        public string UserName { get; set; }

        public bool IsReset { get; set; }
    }
}
