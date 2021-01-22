//============================================================
// Student Number : S10202961, S10204388
// Student Name : Chen Han, Chung Tze Siong
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Text;

//Done
namespace COVID_19_Monitoring_System
{
    class TravelEntry
    {
        private string lastCountryOfEmbarkation;

        public string LastCoutryOfEmbarkation
        {
            get { return lastCountryOfEmbarkation; }
            set { lastCountryOfEmbarkation = value; }
        }

        private string entryMode;

        public string EntryMode
        {
            get { return entryMode; }
            set { entryMode = value; }
        }

        private DateTime entryDate;

        public DateTime EntryDate
        {
            get { return entryDate; }
            set { entryDate = value; }
        }

        private DateTime shnEndDate;

        public DateTime ShnEndDate
        {
            get { return shnEndDate; }
            set { shnEndDate = value; }
        }

        private SHNFacility shnStay;

        public SHNFacility ShnStay
        {
            get { return shnStay; }
            set { shnStay = value; }
        }

        private bool isPaid;

        public bool IsPaid
        {
            get { return isPaid; }
            set { isPaid = value; }
        }

        //methods
        public TravelEntry() { }

        public TravelEntry(string lcoe, string em, DateTime ed)
        {
            LastCoutryOfEmbarkation = lcoe;
            EntryMode = em;
            EntryDate = ed;
        }

        public void AssignSHNFacility(SHNFacility shn)
        {
            ShnStay = shn;
        }

        public void CalculateSHNDuration()
        {
            //Check Last Country of Embarkation & gives SHN Days
            if (LastCoutryOfEmbarkation == "New Zealand" || LastCoutryOfEmbarkation == "Vietnam")
            {
                ShnEndDate = EntryDate;
                Console.WriteLine("No. of SHN Days: {0}", (ShnEndDate - EntryDate));
            }
            else if(LastCoutryOfEmbarkation == "Macao SAR")
            {
                ShnEndDate = EntryDate.AddDays(7);
                Console.WriteLine("No. of SHN Days: {0}", (ShnEndDate - EntryDate));
            }
            else
            {
                ShnEndDate = EntryDate.AddDays(14);
                Console.WriteLine("No. of SHN Days: {0}", (ShnEndDate - EntryDate));
            }
        }

        public override string ToString()
        {
            return base.ToString() + "Last Country of Embarkation: " + LastCoutryOfEmbarkation + "\tEntry Mode: " + EntryMode + "\tEntry Date: " + EntryDate + "\tSHN End Date: " + ShnStay;
        }

    }
}
