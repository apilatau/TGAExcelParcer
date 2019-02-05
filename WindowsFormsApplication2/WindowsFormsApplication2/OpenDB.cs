using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;


namespace WindowsFormsApplication2
{
    public partial class OpenDB : Form
    {
        public string ConnectionYes = "";
        public string ConnectionNo = "";

        public string initialcatalog = "";

        public OpenDB()
        {
            InitializeComponent();
            this.Text = "Specify the path to DataBase:";
        }

        private void OpenDB_Load(object sender, EventArgs e)
        {
            HideAddControls();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {// срабатывает после изменения пользователем

            if (checkBox1.Checked == true)
            {
                HideAddControls();
                checkBox2.Checked = false;
            }
            else if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
                checkBox2.Checked = true; //если включается второй чекбокс выключением первого                          
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                ShowAddControls();
                checkBox1.Checked = false;
            }
            else if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
                checkBox1.Checked = true;// если включается первый чекбокс выключением второго
        }

        private void HideAddControls()
        {
            comboBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            this.Size = new Size(349, 131);
        }

        private void ShowAddControls()
        {
            this.Size = new Size(349, 203);            
            button1.Enabled = true;
            button2.Enabled = true;
            comboBox1.Enabled = true;
        }

        //установить путь к файлу БД
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DataBaseConnect.AttachDBFilename = openFileDialog1.FileName;
                this.initialcatalog = openFileDialog1.InitialDirectory;
                comboBox1.Items.Add(DataBaseConnect.AttachDBFilename);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Устанавливается новая строка подключения
               DataBaseConnect.ConnectParam Dbcp = new DataBaseConnect.ConnectParam(DataBaseConnect.AttachDBFilename);
               
               try
               {
                   //перебивается стока в конфиге с учетом нового пути
                   DataBaseConnect.ConnectionStrings(Dbcp);

                   if (DataBaseConnect.IsServerConnected())
                   {
                       ConnectionYes = "Connection with DB is build. ";
                       DialogResult resultYes = MessageBox.Show(
                           ConnectionYes + "Do you intend to continue saving data in the DB [yes]?"
                           + " or choose any other DB [no] ",
                           "Result of Connection",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Information,
                           MessageBoxDefaultButton.Button1,
                           MessageBoxOptions.DefaultDesktopOnly);
                       
                       if ((resultYes == DialogResult.Yes) || (resultYes == DialogResult.None))
                       {
                            Form1 frm1 = new Form1();//если пользователь выбирает "yes" , загружаем основную форму
                            frm1.Show();
                            this.Hide();// и закрываем эту
                       }
                           
                   }
                   else
                   {
                       ConnectionNo = "Connection with DB is failed";
                       DialogResult resultNo = MessageBox.Show(
                           ConnectionNo + " Do you want to try to connect once again? [yes] ",
                           "Result of Connection",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Information,
                           MessageBoxDefaultButton.Button1,
                           MessageBoxOptions.DefaultDesktopOnly);
                       if ((resultNo == DialogResult.No) || (resultNo == DialogResult.None))
                           this.Close();
                   }
               }
               catch (Exception ex)
               {
                   if (DataBaseConnect.l_oConnection != null)
                       DataBaseConnect.l_oConnection.Close();
                   
                   this.Close();

                   //throw new DataException("Соединение с базой отсутствует");
               }
               finally
               {}           
          }
    }
}
