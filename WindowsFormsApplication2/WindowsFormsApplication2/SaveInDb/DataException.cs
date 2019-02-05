using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    public class DataException:Exception
    {
        static private string errmess;
        static public string ErrMes {
            get
            {
                return errmess;
            }
            private set
            {
                errmess = value;
            }
                }
        public DataException(string message) : base(message)
        {
            errmess = message;
        }
    }
}
