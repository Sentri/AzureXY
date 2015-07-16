using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureXY.Models
{
    public class DrawingQueue
    {
        public int ID { get; set; }

        public int DrawingID { get; set; }

        public DateTime QueueTime { get; set; }
        
        public int BoardID { get; set; }
        public virtual Board Board { get; set; }
    }
}
