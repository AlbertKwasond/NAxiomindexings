using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DMS.Models
{
    public class Author
    {
        // Unique identifier for the author
        public Guid Id { get; set; } = Guid.NewGuid();

        // First name of the author
        [Required(ErrorMessage = "Please enter your first name")]
        [StringLength(255)]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        // Middle name of the author
        [StringLength(200)]
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        // Last name of the author
        [Required(ErrorMessage = "Please enter your last name ")]
        [StringLength(255)]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        // Full name of the author (concatenation of first name, middle name, and last name)
        public string FullName
        {
            get { return String.Format("{0} {1} {2}", this.FirstName, this.MiddleName, this.LastName); }
        }

        // Gender of the author
        [Required(ErrorMessage = "Please select your gender")]
        [StringLength(25)]
        public string Gender { get; set; }

        // Date of birth of the author
        [Required(ErrorMessage = "Please enter your date of birth ")]
        [DisplayName("Date Of Birth")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        // Email address of the author
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        // Contact number of the author
        [Required(ErrorMessage = "Please enter the contact number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Contact { get; set; }

        // Address of the author
        [StringLength(200)]
        public string Address { get; set; }

        // Country of origin of the author
        [Required(ErrorMessage = "Please enter the country of origin")]
        [StringLength(100)]
        [DisplayName("Country Of Origin")]
        public string CountryOfOrigin { get; set; }

        // Registration status of the author
        [StringLength(50)]
        [DisplayName("Registration Status")]
        public string RegistrationStatus { get; set; }

        // Date when the author registered
        public DateTime DateOfRegistration { get; set; } = DateTime.Now;

        // Navigation property to the journals written by the author
        public ICollection<Journals> Journals { get; set; }
    }
}