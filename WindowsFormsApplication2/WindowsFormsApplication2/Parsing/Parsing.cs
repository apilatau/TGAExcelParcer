using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;

namespace WindowsFormsApplication2
{
    public class Parsing
    {
        private Worksheet objWorkSheetParse;
        private BackgroundWorker backgroundWorker;

        IFormatProvider formatter;

        private string NumberSeparator = "";

        private int iCountTGARecords;

        private List<List<double>> lTGA = new List<List<double>>();

        //регулярное выражение для поиска десятичной дроби с 5 знаками после запятой
        private string rgDecFr = @"[0-9]+([\.,][0-9]{1,5})?";
       

        public Parsing(ref Worksheet _objWorkSheet, ref BackgroundWorker _backgroundWorker)
        {
            if (_objWorkSheet != null)
                objWorkSheetParse = _objWorkSheet;
            else
                throw new ParsingException("присутствует нулевая ссылка _objWorkSheet :" + _objWorkSheet.ToString()); // выбрасывает в вызывающий конструктор метод исключение о том, что 
            // присутствует нулевая ссылка

            if (_backgroundWorker != null)
                backgroundWorker = _backgroundWorker;
            else
                throw new ParsingException("присутствует нулевая ссылка _backgroundWorker :" + _backgroundWorker.ToString());

            iCountTGARecords = CountTGAData;

            if(iCountTGARecords<=1)
                throw new ParsingException("пустой лист экселя :" + _objWorkSheet.Name.ToString());
        }

        public int CountTGAData
        {
            get
            {
                return GetCountTGAData(ref objWorkSheetParse);
            }
        }

        private int GetCountTGAData(ref Worksheet objWorkSheetParse)
        {
            int i = 13;
            string stvalue = "";
            Range range;

            do
            {
                i++;
                range = objWorkSheetParse.get_Range("A" + i.ToString(), "A" + i.ToString());//.Cells[i, j];
                stvalue = range.Text.ToString();

                if (backgroundWorker.CancellationPending == true)
                    break;
            }
            while (stvalue != "");

            return i - 13;
        }

        //обмен с общим классом значением начальной массы
        public double InitialMass
        {
            get
            {                
                return IniMass(ref objWorkSheetParse);
            }            
        }                

        //парсинг из файла начальной массы Initial Mass
        private double IniMass(ref Worksheet objWorkSheet)
        {
            //выделение строки из excel файла
            Range range = objWorkSheet.get_Range("A6", "A6");//.Cells[i, j] - не работает в этом контексте;
            string stvalue = range.Text.ToString();

            Regex regIniMass = new Regex(rgDecFr); // [\.,] - или точка или запятая
            MatchCollection matches = regIniMass.Matches(stvalue);
            if (matches.Count > 0)
            {
                double result;

                foreach (Match match in matches)
                {
                    result = ConvertStringToDouble(match.Value);

                    if (result != 0)
                        return result;
                    else
                        throw new ParsingException("Не нашелся параметр Initial Mass в соответствующей строке");
                }                     
            }
            else
            {
                throw new ParsingException("Числа в InitialMass не найдено");                
            }
            return 0;
        }

        //определяет - что в выделенном числе точка или запятая
        // достаточно одного вызова этой функции, затем искомое значение сепаратора 
        //помещается в переменную NumberSeparator 
        private string IsPointOrComma(string strNumber, ref string NumberSeparator)
        {
            if (strNumber.IndexOf(",") == -1)
                NumberSeparator = ".";
            else
                NumberSeparator = ",";

            return NumberSeparator;
        }

        //конвертирование строки в double число
        private double ConvertStringToDouble(string _number)
        {
            double result;

            //проверка была ли уже вызвана эта функция - если да то запись из NumberSeparator
            if (NumberSeparator == "")
                NumberSeparator = IsPointOrComma(_number, ref NumberSeparator);

            formatter = new NumberFormatInfo { NumberDecimalSeparator = NumberSeparator };            

            if(double.TryParse(_number, NumberStyles.AllowDecimalPoint, formatter, out result))            
              return result;
            else
            {
                throw new ParsingException("Не удалось конвертировать строку"+ _number+"в число");
                //return 0;               
            }
        }

        //функция получения значением UserTGA

        public string UserTGA
        {
            get
            {
                return GetUserTGA(ref objWorkSheetParse);
            }
        }

        private string GetUserTGA(ref Worksheet objWorkSheet)
        {
            //выделение строки из excel файла
            Range range = objWorkSheet.get_Range("A3", "A3");//.Cells[i, j] - не работает в этом контексте;
            string stvalue = range.Text.ToString();

            Regex regex = new Regex(@"User\s+:\s*");

            return GetRegexMatches(stvalue, regex,true,1);
        }

        //функция получения значения Creation Date
        public DateTime CreationDate
        {
            get
            {
                return GetCreationDate(ref objWorkSheetParse);
            }
        }

        private DateTime GetCreationDate(ref Worksheet objWorkSheet)
        {
            //выделение строки из excel файла
            Range range = objWorkSheet.get_Range("A2", "A2");//.Cells[i, j] - не работает в этом контексте;
            string stvalue = range.Text.ToString();
            Regex regex2 = new Regex(@"Creation\s+Date\s+:\s*");

            DateTime dt;
            string stdt = GetRegexMatches(stvalue, regex2,true,1);
            if (DateTime.TryParse(stdt, out dt))
                return dt;
            else
                throw new ParsingException("Не удалось конвертировать Creation Date в дату" + stdt + "в число");
        }

        //находит регулярное выражение
        private string GetRegexMatches(string fullStr, Regex reg, bool _flagToByMatches, int Column)
        {
            int counter = 0;
            MatchCollection matches = reg.Matches(fullStr);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    counter++;
                    if (counter == Column)
                        return GetSubStringByMatches(fullStr, match.Value, _flagToByMatches);
                }    
            }
            return "0";
            //throw new ParsingException("Запрашиваемой строки не найдено");
        }

        //выделяет из всей строки нужное имя либо дату
        //по полной строке и найденому регулярному выражению, которое обрезается
        private string GetSubStringByMatches(string full, string _matchesString, bool _flagToByMatches)
        {
            if (!_flagToByMatches)
                return _matchesString;

           StringBuilder sbMatches = new StringBuilder(_matchesString);
           StringBuilder sbFull = new StringBuilder(full);

           int pUser = full.IndexOf(_matchesString);
           if (pUser != 0)
             sbFull.Remove(0, pUser);

           return sbFull.Remove(0, sbMatches.Length).ToString().TrimEnd();
        }

        //функция получения значения Sample Temperature
        public List<List<double>> TGAData
        {
            get
            {
                return GetTGAData(ref objWorkSheetParse);
            }
        }


        private List<List<double>> GetTGAData(ref Worksheet objWorkSheet)
        {                        
            string stvalue = "";
            int iRow = 13;
            Regex regex3;
            Range range;
            int prState = 1;            
                       
            while ((stvalue != "") || (iRow==13))
            {
                if (backgroundWorker.CancellationPending == true)
                    break;
                iRow++; 
                System.Threading.Thread.Sleep(100);
                prState = ((70 * (iRow - 13)) / (iCountTGARecords)) + 30;                
                backgroundWorker.ReportProgress(prState, "Working TGA Data Parsing.." + prState.ToString() + "%");
                range = objWorkSheet.get_Range("A" + iRow.ToString(), "A" + iRow.ToString());//.Cells[i, j] - не работает в этом контексте;
                stvalue = range.Text.ToString();
                regex3 = new Regex(@"([0-9]+([\.,][0-9]{1,7})?)");

                AddListRow(ref lTGA);
                AddTwoColumnList(ref lTGA, iRow-14, ConvertStringToDouble(GetRegexMatches(stvalue, regex3, false, 4)), ConvertStringToDouble(GetRegexMatches(stvalue, regex3, false, 5)));
            }
            return lTGA;
        }
        
        //добавать строку в лист ( двумерный массив
        private void AddListRow(ref List<List<double>> _lst)
        {
            _lst.Add(new List<double>());
        }

        //добавить две колонки в лист ( двумерный массив) к iой строке со значениями ValueSampleTemperature и ValueTGA
        private void AddTwoColumnList(ref List<List<double>> _lst, int _i, double valueSmp, double valueTGA)
        {
            _lst[_i].Add(valueSmp);
            _lst[_i].Add(valueTGA);
        }

       

       

    }
}
