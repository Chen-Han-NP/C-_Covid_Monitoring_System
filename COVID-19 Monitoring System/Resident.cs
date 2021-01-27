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
    class Resident: Person
    {
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private DateTime lastLeftCountry;

        public DateTime LastLeftCountry
        {
            get { return lastLeftCountry; }
            set { lastLeftCountry = value; }
        }

        private TraceTogetherToken token;

        public TraceTogetherToken Token
        {
            get { return token; }
            set { token = value; }
        }

        public Resident(string n, string a, DateTime dt) : base(n)
        {
            Address = a;
            LastLeftCountry = dt;
        }

        public override double CalculateSHNCharges()
        {
            double totalCost = 0;
            foreach (TravelEntry te in TravelEntryList)
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
                        if (te.LastCoutryOfEmbarkation == "New Zealand" || te.LastCoutryOfEmbarkation == "Vietnam")
                        {
                            totalCost = 200 * 1.07;
                        }
                        else if (te.LastCoutryOfEmbarkation == "Macao SAR")
                        {
                            totalCost = (200 + 20) * 1.07;
                        }
                        else
                        {
                            totalCost = (200 + 20 + 1000) * 1.07;
                        }
                    }
                }
            }
            return totalCost;

        }

        public override string ToString()
        {
            return "Name: " + Name + "\nAddress: " + Address + "\nLast Left Country: " + LastLeftCountry.ToString("dd MMMM, yyyy");
        }
    }
}


