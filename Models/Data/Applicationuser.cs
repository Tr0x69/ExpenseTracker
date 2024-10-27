using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models.Data
{
    public class Applicationuser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal? Budget {  get; set; }
        [NotMapped]
        public IList<string> RoleNames { get; set; } = null!;
    }
}
