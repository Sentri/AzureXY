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

        [DataType(DataType.MultilineText)]
        public string Instructions { get; set; }

        public string Name { get; set; }
        
        public DateTime Created { get; set; }

        public Drawing()
        {
            Created = DateTime.Now;
        }
    }
}