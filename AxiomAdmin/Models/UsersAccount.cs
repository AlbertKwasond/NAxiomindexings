using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AxiomAdmin.Models
{
    public class UsersAccount
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please enter your first name")]
        [Display(Name = "First Name")]
        [StringLength(150)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your Last name")]
        [Display(Name = "Last Name")]
        [StringLength(150)]
        public string LastName { get; set; }


        [DisplayName("Date Of Birth")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [StringLength(10)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter a mobile number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter an address")]
        [StringLength(255, ErrorMessage = "Address should be less than 255 characters")]
        public string Address { get; set; }

        [StringLength(50)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Please select a role")]
        [StringLength(50)]
        public string? Roles { get; set; }
    }
}