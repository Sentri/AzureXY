using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzureXY.Models
{
    public class Board
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string AccessToken { get; set; }

        public string ApplicationUserID { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public virtual List<Drawing> Drawings { get; set; }

    }
}