using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    public class Initial
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime FileCreationDate { get; set; }
        public string UserTGA { get; set; } // who is log in TG analyzer and composed excel file
        public double InitialMass { get; set; }
        public bool InPercent { get; set; } // TGA masslost measured in percents or in absolute mass

        /* for providing one per many connections with Treatment*/
        public int? TreatmentId { get; set; }
        public Treatment Treatment { get; set; }

        /* one to many with TGA */
        public ICollection<TGA> TGAs { get; set; }

        public Initial()
        {
            TGAs = new List<TGA>();
        }
    }
}
