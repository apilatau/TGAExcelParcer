using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Loading : Form
    {
        private DateTime StartTime = DateTime.Now;

        private string result = "";

        public Loading()
        {
            InitializeComponent();

            backgroundWorker1.WorkerReportsProgress = true;
            
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);

            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;


            if (backgroundWorker1.IsBusy != true)
                backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100)            
                toolStripProgressBar1.Value = e.ProgressPercentage;            
            else
                toolStripProgressBar1.Value = 1;
                        
            //toolStripStatusLabel1.Text = e.UserState as String;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           
            timer1.Stop();

            if (e.Result.ToString() != "")
            {
                OpenDB opdbForm = new OpenDB();
                opdbForm.Show();
            }
            else
            {
                Form1 fr1 = new Form1();
                fr1.Show();
            }                

            this.Hide();

        }

        private void ConnectSetting()
        {
            try
            {
                if (DataBaseConnect.IsServerConnected())
                {
                    if (backgroundWorker1.IsBusy == true)
                        backgroundWorker1.CancelAsync();                    
                }
                    
            }
            catch (DataException exdb)
            {
                throw new DataException("Соединение с базой отсутствует");                
            }
            finally
            {
                
            }
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           
            DateTime StartTime = DateTime.Now;// еще раз перебивается время при старте

            try
            {
                Parallel.Invoke(
                    () =>
                    {
                        try
                        {
                            ConnectSetting();
                        }
                        catch (DataException exdb)
                        {
                            this.result = "Соединение с базой отсутствует";
                        }
                        finally
                        {
                            e.Result = this.result;
                        }
                    },
                     timer1.Start
                    );
            }
            catch(DataException exdb)
            {
                e.Result = this.result;
            }
            finally
            {}                
        }

        private int Progressposition()
        {
            DateTime curtime = DateTime.Now;
            TimeSpan result = curtime - StartTime;

            return Convert.ToInt32(Math.Round(result.TotalSeconds));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker1.CancellationPending)
               backgroundWorker1.ReportProgress(Progressposition(), "Setting Connection to DB" );
            else
                backgroundWorker1.ReportProgress(100, "Setting Connection to DB");

        }
    }
}
