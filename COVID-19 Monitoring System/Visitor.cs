//============================================================
// Student Number : S10202961, S10204388
// Student Name : Chen Han, Chung Tze Siong
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Text;

//CalculateSHNCharges() method
namespace COVID_19_Monitoring_System
{
    class Visitor: Person
    {
        private string passportNo;

        public string PassportNo
        {
            get { return passportNo; }
            set { passportNo = value; }
        }

        private string nationality;

        public string Nationality
        {
            get { return nationality; }
            set { nationality = value; }
        }

        public Visitor(string p, string na, string n):base(n)
        {
            PassportNo = p;
            Nationality = na;
            
        }

        public override double CalculateSHNCharges()
        {
            double totalCost = 0;
            foreach(TravelEntry te in TravelEntryList)
            {
                if (!te.IsPaid)
                {
                    if (te.ShnEndDate > DateTime.Now)
                    {
                        Console.WriteLine("Your stay has not ended yet!");
                        return 0;
                    }
                    else
                    {
                        List<String> countriesList = new List<String> { "Vietnam", "New Zealand", "Macao SAR" };
                        if (countriesList.Contains(te.LastCoutryOfEmbarkation))
                        {
                            totalCost = (200 + 80) * 1.07;
                        }
                        else
                        {
                            double travelCost = te.ShnStay.CalculateTravelCost(te.EntryMode, te.EntryDate);
                            totalCost = (travelCost + 2000 + 200) * 1.07;
                        }
                    }
                }

            }
            return totalCost;
        }

        public override string ToString()
        {
            return "Name: " + Name + "\nPassport No: " + PassportNo + "\nNationality: " + Nationality;
        }


    }
}
