using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.Common;


namespace WindowsFormsApplication2
{
    public class SaveInDb
    {
        private TGAContex locDb;

        BackgroundWorker bgrwork;

        SaveParamInDb svPar;

        private List<List<double>> slTGA = new List<List<double>>();
        public SaveInDb(ref TGAContex _db, ref BackgroundWorker _bgrwork, ref SaveParamInDb _svPar, ref List<List<double>> _slTGA)
        {
            locDb = _db;
            bgrwork = _bgrwork;
            svPar = _svPar;
            slTGA = _slTGA;            
        }

        public void SaveDataInDB(ref BackgroundWorker _bgrwork)
        {
            try
            {
                Saving(ref _bgrwork);
            }
            catch(Exception edb)
            {
                
                throw new DataException("Соединение с базой отсутствует");
            }
            finally
            { }
           
        }
        private void Saving( ref BackgroundWorker _bgrwork)
        {
            if (IsUnique())
            {
                Treatment trs = TreatmentPaste();
                locDb.Treatments.Add(trs);

                //db.SaveChanges();
                Task taskSVinBDTretment = new Task(() => locDb.SaveChanges());
                taskSVinBDTretment.Start();
                taskSVinBDTretment.Wait();

                _bgrwork.ReportProgress(30, "Saved In DB..(Treatment)");

                Initial inis = InitialPaste(ref trs);
                locDb.Initials.AddRange(new List<Initial> { inis });

                Task taskSVinBDinis = new Task(() => locDb.SaveChanges());
                taskSVinBDinis.Start();
                taskSVinBDinis.Wait();

                _bgrwork.ReportProgress(60, "Saved In DB..(Initial)");
                //db.SaveChanges();

                TGA tgas;

                for (int i = 0; i < slTGA.Count; i++)
                {
                    tgas = TGAPaste(ref inis, slTGA[i][0], slTGA[i][1]);
                    locDb.TGAs.AddRange(new List<TGA> { tgas });
                }

                Task taskSVtgas = new Task(() => locDb.SaveChanges());
                taskSVtgas.Start();
                taskSVtgas.Wait();
                _bgrwork.ReportProgress(100, "Complete! Save In DB..");
            }
        }
        
        private bool IsUnique()
        {
            var fileNames = from ini in locDb.Initials
                            join tr in locDb.Treatments on ini.TreatmentId equals tr.Id
                            where (ini.FileName == svPar.FileName) && (tr.TreatmentDate == svPar.startDate)
                            select new { flNam = ini.FileName };
            if (fileNames.Count() > 0)
                return false;
            else
                return true; // возвращает true если ничего не нашел
        }

        private Treatment TreatmentPaste()
        {
            return new Treatment
            {
                TreatmentDate = svPar.startDate,
                TreatersName = System.Security.Principal.WindowsIdentity.GetCurrent().Name
            };
        }

        private Initial InitialPaste(ref Treatment _tr)
        {
            return new Initial
            {
                FileName = svPar.FileName,
                FileCreationDate = svPar.CrDt,
                UserTGA = svPar.UserTGA,
                InitialMass = svPar.iMass,
                InPercent = svPar.inMassOrNot,
                Treatment = _tr
            };
        }

        private TGA TGAPaste(ref Initial _ini, double smpTemp, double tgaDate)
        {
            return new TGA
            {
                SampleTemperature = smpTemp,
                TGAdata = tgaDate,
                Initial = _ini
            };
        }
    }
}
