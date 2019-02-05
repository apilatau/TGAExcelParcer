using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using Microsoft.Office.Interop.Excel;

namespace WindowsFormsApplication2
{
    // структура, передающая необходимые параметры в класс SaveInDb сохранения данных
    public struct SaveParamInDb
    {
        public DateTime startDate;
        public string FileName;
        public DateTime CrDt;
        public string UserTGA;        
        public double iMass;
        public bool inMassOrNot;
    }    

    public partial class Form1 : Form
    {
        public TGAContex db = new TGAContex();
        

        //для возврашения результатов из методов 
        //FillUsersData
        //FillFileNameData
        public IEnumerable<Treatment> tr;
        public IEnumerable<Initial> init;
        Parsing parse;
        public List<List<double>> listTGA = new List<List<double>>();

        public static Worksheet ObjWorkSheet; // переменная листа, которая передается по ссылке в другие классы для доступа к Эекселю
        Microsoft.Office.Interop.Excel.Application ObjExcel;

        public double iMass=0;
        public DateTime crdt;
        string userTGA = "";

        bool flLastRecRB = true; // флаг последней записи в ричбокс
        bool flDropForm = false;
      //  bool flbgwr = false; // флаг сохранения для backgrounda

       // SaveParamInDb svparIndb; //структура для передачи информации в конструктор класса сохранения ресультатов.

        DateTime startDate = DateTime.Now;

        public Form1()
        {
            InitializeComponent();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            

            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork += new DoWorkEventHandler(backgroundWorker2_DoWork);
            backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker2_RunWorkerCompleted);
            backgroundWorker2.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker2_ProgressChanged);

            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.WorkerSupportsCancellation = true;
            backgroundWorker3.DoWork += new DoWorkEventHandler(backgroundWorker3_DoWork);
            backgroundWorker3.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker3_RunWorkerCompleted);
            backgroundWorker3.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker3_ProgressChanged);

            /*     if (FillUsersData(db) != null) // если имеются данные в базе в таблице Обработки Treatment
                 {
                     FillComboUsers(db);

                     if (FillFileNameData(db, FillUsersData(db)) != null)
                         FillComboFileNames(db);
                     else
                         comboBox1.DroppedDown = false;
                 }               
                 else
                     comboBox2.DroppedDown = false; 
                     */
        }

        public void ExcelConnect()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                comboBox1.Items.Add(openFileDialog1.FileName);
                //Создаём приложение.
                ObjExcel = new Microsoft.Office.Interop.Excel.Application();
                //Открываем книгу.                                                                                                                                                        
                Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = ObjExcel.Workbooks.Open(openFileDialog1.FileName, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                //Выбираем таблицу(лист).
                //Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet;
                ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];

                //Очищаем от старого текста окно вывода.
                richTextBox1.Clear();                
            }
        }

   /*    public void FillComboFileNames(TGAContex _db)
        {
            var _filenmtr = FillFileNameData(db, FillUsersData(db));
            foreach (Initial usTr in _filenmtr)
            {
                if (comboBox1.Items.Count > 10)
                {
                    comboBox1.Items.RemoveAt(1);
                    comboBox1.Items.Add(usTr);
                }
            }
            int index = comboBox1.FindString(_filenmtr.First().FileName);
            comboBox1.SelectedIndex = index;
            
            comboBox1.Sorted = true;
           
            return;
        }
*/
       

      /*  public void FillComboUsers(TGAContex _db)
        {
            var _usertr = FillUsersData(db);
            foreach (Treatment usTr in _usertr)
            {
                if (comboBox2.Items.Count > 10)
                {
                    comboBox2.Items.RemoveAt(1);
                    comboBox2.Items.Add(usTr.TreatersName);
                }                                    
            }
            comboBox2.Sorted = true;
            comboBox2.DroppedDown = true;
            return;
        }

            */
      /*  public IEnumerable<Treatment> FillUsersData(TGAContex _db)
        {
            return _db.Treatments.Where(p => p.UserName != null);
        }
        */
        public IEnumerable<Initial> FillFileNameData(TGAContex _db, IEnumerable<Treatment> _tr)
        {
            return _db.Initials.Include(p => p.Treatment == _tr);
        }
                

        private void button1_Click(object sender, EventArgs e)
        {
            ExcelConnect();

            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(openFileDialog1.FileName);

            comboBox1.Sorted = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                if (listTGA.Count != 0)
                  listTGA.Clear();

                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                button3.Enabled = false;
                button2.Enabled = false;

                backgroundWorker1.RunWorkerAsync();
            }                      
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            string res = "Complete!";

            try
            {
                Preatretment(e);

                SaveParamInDb svparIndb = new SaveParamInDb
                {
                    startDate = this.startDate,
                    FileName = openFileDialog1.FileName,
                    CrDt = crdt,
                    UserTGA = userTGA,
                    iMass = this.iMass,
                    inMassOrNot = checkBox2.Checked
                };
                // вызов конструктора класса сохранения данных
                SaveInDb svdb = new SaveInDb(ref db, ref backgroundWorker2, ref svparIndb, ref listTGA);

                //вызов метода сохранения данных
                svdb.SaveDataInDB(ref backgroundWorker2);
            }
            catch (ParsingException exPar)
            {
                res = "Ошибка :" + exPar.Message + "\n" + exPar.InnerException + "\n";
            }
            finally
            {
                e.Result = res;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string res = "Complete!";

            try
            {
                Preatretment(e);
            }
            catch (ParsingException exPar)
            {
                res = "Ошибка :" + exPar.Message + "\n" + exPar.InnerException + "\n";
            }
            finally
            {
                e.Result = res;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
                if ((backgroundWorker1.CancellationPending == true) && (!flDropForm))
                    return;

                toolStripProgressBar1.Value = e.ProgressPercentage;
                toolStripStatusLabel1.Text = e.UserState as String;

                var switchInitBlock = new Dictionary<Func<int, bool>, Action<double, DateTime, string>> // в Action идут параметры, которые надо передать в вызовы в соответствие с условиями
            {
                 { x => ((x == 10)) , (im,cd,u) =>  richTextBox1.AppendText(" InitMass :" + im.ToString() + "\n")  },
                 { x => ((x == 20)) , (im,cd,u) => richTextBox1.AppendText(" Creation Date :" + cd.ToString() + "\n")  },
                 { x => ((x == 30)) , (im,cd,u) =>  richTextBox1.AppendText(" User TGA  :" + u + "\n") }
            };

                // Now to call our conditional switch
                if ((toolStripProgressBar1.Value <= 30) && (toolStripProgressBar1.Value > 2))
                {
                    if (flLastRecRB)
                        switchInitBlock.First(sw => sw.Key(toolStripProgressBar1.Value)).Value(iMass, crdt, userTGA);
                    if (toolStripProgressBar1.Value == 30)
                        flLastRecRB = false;
                }
                System.Windows.Forms.Application.DoEvents();
          //  }
                
        }    

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {          
                if (!flDropForm)
                    FillDataGrid(ref listTGA);

                richTextBox1.AppendText(e.Result.ToString());
                button3.Enabled = true;
                button2.Enabled = true;       
        }

        public void Preatretment(DoWorkEventArgs e)
        {
            string res = "Complete!";
            try
            {
                if(!backgroundWorker1.CancellationPending)
                {
                    backgroundWorker1.ReportProgress(2);
                    parse = new Parsing(ref ObjWorkSheet, ref backgroundWorker1);

                    iMass = parse.InitialMass;
                    backgroundWorker1.ReportProgress(10, "Working initial mass parsing");

                    crdt = parse.CreationDate;
                    backgroundWorker1.ReportProgress(20, "Working creation data parsing");

                    userTGA = parse.UserTGA;
                    backgroundWorker1.ReportProgress(30, "Working User TGA Parsing");

                    listTGA = parse.TGAData;                    
                }
            }
            catch(ParsingException eParse)
            {
                res = "Ошибка :" + eParse.Message + "\n" + eParse.InnerException + "\n";
            }
            finally
            {                
                backgroundWorker1.ReportProgress(100, "Complete!");
                e.Result = res;                
            }
        }

        public void FillDataGrid(ref List<List<double>> _ST)
        {
            for (int i=0; i<_ST.Count; i++)                
              dataGridView1.Rows.Add(new object[] { _ST[i][0], _ST[i][1] });            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // treatment to closing form

            if (backgroundWorker1.IsBusy == false)
            {
                if (ObjExcel!=null)
                    ObjExcel.Quit();
                return;
            }                

            const string message = "Are you sure that you would like to close the form?";
            const string caption = "Form Closing";

            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.No)
            {
                // cancel the closure of the form.
                e.Cancel = true;
            }
            else
            {
                flDropForm = true;
                backgroundWorker1.CancelAsync();
                ObjExcel.Quit();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                backgroundWorker1.CancelAsync();
                button2.Enabled = true;
                button3.Enabled = true;
            }               
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Text = comboBox1.Items[0].ToString();
        }
        private void ComboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {}

        private void button3_Click(object sender, EventArgs e)
        {
            SavindData();
        }

        private void SavindData()
        {           

            if ((backgroundWorker1.IsBusy != true) && (backgroundWorker2.IsBusy != true))
            {
                Task t1 = new Task(() =>
                {
                    if (listTGA.Count <= 1)
                        backgroundWorker1.RunWorkerAsync();
                });

                Task t2 = t1.ContinueWith(bgwkStartAfterRead);

                t1.Start();
                t2.Wait();            
                               
            }
        }

        private void bgwkStartAfterRead(Task t)
        {
            backgroundWorker2.ReportProgress(1, "Start of Work for Saving In DB..");
            backgroundWorker2.RunWorkerAsync();
        }

       
        //методы второго backgroundworker2 для сохранения данных
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            string res = "Saving is Complete!";
            try
            {
                // инициализация структуры сохранения данных
                SaveParamInDb svparIndb = new SaveParamInDb
                {
                    startDate = this.startDate,
                    FileName = openFileDialog1.FileName,
                    CrDt = crdt,
                    UserTGA = userTGA,
                    iMass = this.iMass,
                    inMassOrNot = checkBox2.Checked
                };
                // вызов конструктора класса сохранения данных
                SaveInDb svdb = new SaveInDb(ref db, ref backgroundWorker2, ref svparIndb, ref listTGA);

                //вызов метода сохранения данных
                svdb.SaveDataInDB(ref backgroundWorker2);
            }
            catch(DataException exdb)
            {
                res = "Ошибка :" + exdb.Message + "\n" + exdb.InnerException + "\n";
            }
            finally
            {
                backgroundWorker2.ReportProgress(100, "Complete!");
                e.Result = res;
            }   
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            richTextBox1.AppendText(e.Result.ToString());            
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = e.UserState as String;
        }      
       
    }
}
