using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace WindowsFormsApplication2
{
    public class TGAContex : DbContext,IDisposable
    {
        private bool disposed = false;//true -ресурсы неуправляемые ; false - управляемые

        public TGAContex() : base("TGAContex")
        { }

        public DbSet<TGA> TGAs { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Initial> Initials { get; set; }

        //реализация интерфейса IDisposable - удаления ресурсов
        
        public void DisposeTGA()
        {
            Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Освобождаем управляемые ресурсы проверим на всякий случай не открыто ли sql-соединение 
                    if (DataBaseConnect.l_oConnection != null)
                        DataBaseConnect.l_oConnection.Close();
                }
                // освобождаем неуправляемые объекты
                disposed = true;
            }
        }        

        // Деструктор
        ~TGAContex()
        {
            Dispose(false);
        }

    }
}
