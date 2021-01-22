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
            return 0;
        }

        public override string ToString()
        {
            return "Name: " + Name + "\nAddress: " + Address + "\nLast Left Country: " + LastLeftCountry;
        }
    }
}
