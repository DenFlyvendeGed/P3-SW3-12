using Microsoft.AspNetCore.Mvc.ModelBinding;
using Org.BouncyCastle.Utilities;
using P3_Project.Models.DB;
using System.Security.Policy;

namespace P3_Project.Models
{
    public class Faktura
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public int CompanyCVR { get; set; }
        public string PickUpAddress { get; set; }

        public int PickUpDeadLine { get; set; }

        public int Moms { get; set; }

        static StorageDB db = new StorageDB();


        public Faktura()
        {
            Id = 0;
            CompanyName = "";
            CompanyAddress = "";
            CompanyCVR = 0;
            PickUpAddress = "";
            PickUpDeadLine = 0;
            Moms = 0;
        }

        public void UpdateFaktura()
        {
            if(db.DB.CheckRow("FakturaSettings", "Id", "1"))
            {
                db.DB.DeleteTable("FakturaSettings");
                db.DB.CreateTable("FakturaSettings", new Faktura());
            }

            db.DB.AddRowToTable("FakturaSettings", this);
        }

    }
   
}
