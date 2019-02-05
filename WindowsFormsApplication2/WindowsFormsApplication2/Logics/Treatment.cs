using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    public class Treatment
    {
        public int Id { get; set; }
        public DateTime TreatmentDate { get; set; }
        //public DateTime TreatmentDate { get; set; }
        public string TreatersName { get; set; }       

        /* обеспечение связи один ко многим */
        public ICollection<Initial> Initials { get; set; } 
        public Treatment()
        {
            Initials = new List<Initial>();
        }

    }
}
