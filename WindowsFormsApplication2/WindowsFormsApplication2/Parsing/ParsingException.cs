using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    public class ParsingException: Exception // класс для выбрасывания своих исключений в классе Parsing
    {
        public ParsingException(string message) : base(message)
        { }
    }
}
