using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    public class TGA
    {
        public int Id { get; set; }
        public double SampleTemperature { get; set; }
        public double TGAdata { get; set; }

        /*for providing one to many connections with Initial */
        public int? InitialId { get; set; }
        public Initial Initial { get; set; }
    }
}
