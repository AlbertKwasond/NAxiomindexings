using AxiomAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AxiomAdmin.ViewModel
{
    public class UsersAccountsViewModle
    {
        public UsersAccount GetUsersAccount { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string? Password { get; set; }

        public List<SelectListItem>? Roles { get; set; }
    }
}