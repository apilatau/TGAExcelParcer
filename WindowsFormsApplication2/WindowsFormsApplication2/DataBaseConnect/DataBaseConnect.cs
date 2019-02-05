using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;

namespace WindowsFormsApplication2
{
    public class DataBaseConnect
    {
        public static string AttachDBFilename { get; set; }
        public static SqlConnection l_oConnection;

        //структура параметров строки соединения
        public struct ConnectParam
        {
            public string DataSourse;
            public string InitialCatalog;
            public bool  IntegratedSecurity;
            public string AttachDbFileName;
            public string NameConString;

            public ConnectParam(string _pathToDb, string _dsourse= @"(localdb)\MSSQLLocalDB", 
                string _iniCat= @"TGATreatment.mdf", bool _inSecurity=true, string NameCStr= "TGAContex")
            {
                DataSourse = _dsourse;
                InitialCatalog = _iniCat;
                IntegratedSecurity = _inSecurity;
                this.AttachDbFileName = @"" + _pathToDb;
                NameConString = NameCStr;
            }  
        }

        public static void ConnectionStrings(ConnectParam _cp)
        {
            SqlConnectionStringBuilder connect1 =new SqlConnectionStringBuilder();

            connect1.DataSource = _cp.DataSourse; // имя сервера
            connect1.InitialCatalog = _cp.InitialCatalog; // имя базы данных
            connect1.IntegratedSecurity = _cp.IntegratedSecurity; //проверка подлинности через авторизацию Windows

            connect1.AttachDBFilename = _cp.AttachDbFileName; // путь к БД

            string conf_name = _cp.NameConString; // название строки подключения из app.config
                ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings[conf_name];
            cs = new ConnectionStringSettings(conf_name, connect1.ConnectionString, "System.Data.SqlClient");

            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            
            // Получаем доступ к строкам подключения
            ConnectionStringsSection csSection = config.ConnectionStrings;
            
            // заменяем строку подключения
            csSection.ConnectionStrings.Remove(cs.Name);
            csSection.ConnectionStrings.Add(cs);
            
            // сохранение файла конфигурации
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        public static bool IsServerConnected(string _cs= "TGAContex")
        {
            bool res = false;
            try
            {
                l_oConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[_cs].ConnectionString);
                l_oConnection.Open();
                //l_oConnection.Close();
                res = true;
            }
            catch (SqlException ex)
            {
                throw new DataException("Соединение с базой отсутствует" + ex.Message);
            }
            finally
            {
                if (l_oConnection != null)
                    l_oConnection.Close();
            }

            return res;
        }
    }
}
