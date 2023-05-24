using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SHP6.Models
{
    public class ApplicationUser
    {
        [Key]
        [DisplayName("Owner's ID")]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        [DisplayName("First Name")]
        [Required(ErrorMessage = "No first name entered")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar(50)")]
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "No last name entered")]
        public string LastName { get; set; }

        [ Column(TypeName = "smalldatetime")]
        [Required(ErrorMessage = "No date entered")]
        [DisplayName("Birthdate")]
        public DateTime BirthDate { get; set; }

        [DisplayName("Username")]
        [Column(TypeName = "varchar(50)")]
        [Required(ErrorMessage = "No username entered")]
        public string UserName { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Required(ErrorMessage = "No email entered")]
        public string Email { get; set; }

        [MinLength(8, ErrorMessage = "Min. of 8 characters")]
        [Column(TypeName = "varchar(50)")]
        [Required(ErrorMessage = "No password entered")]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DisplayName("Confirm Password")]
        public string PasswordConfirmed { get; set; }
    }
}
