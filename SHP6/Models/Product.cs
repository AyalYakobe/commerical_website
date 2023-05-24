using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SHP6.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required(ErrorMessage = "No Title")]
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; }

        [Required(ErrorMessage = "No Short Description")]
        [Column(TypeName = "varchar(100)")]
        [DisplayName("Short Description")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "No description entered")]
        [Column(TypeName = "varchar(2000)")]
        [DisplayName("Long Description")]
        public string LongDescription { get; set; }

        [Required(ErrorMessage = "No Date Entered")]
        [Column(TypeName = "smalldatetime")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "No Price Entered")]
        [Column(TypeName = "decimal(18,2)")]
        public double Price { get; set; }

        public int State { get; set; }

        public string Pic1 { get; set; }
        public string Pic2 { get; set; }
        public string Pic3 { get; set; }

        [NotMapped]
        [DisplayName("Upload Photo")]
        public IFormFile NotMappedPic1 { get; set; }
        [NotMapped]
        [DisplayName("Upload Photo")]
        public IFormFile NotMappedPic2 { get; set; }
        [NotMapped]
        [DisplayName("Upload Photo")]
        public IFormFile NotMappedPic3 { get; set; }
    }
}
