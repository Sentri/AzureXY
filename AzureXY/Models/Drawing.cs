using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzureXY.Models
{
    public class Drawing
    {
        public int ID { get; set; }

        public string Instructions { get; set; }

        public bool Queued { get; set; }

        public DateTime Created { get; set; }

        public int BoardID { get; set; }
        public virtual Board Board { get; set; }
        
        public Drawing()
        {
            Created = DateTime.Now;
        }
    }
}