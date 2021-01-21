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
            return 0;
        }

        public override string ToString()
        {
            return "Passport No: " + PassportNo + "\nNationality: " + Nationality + "Name: " + Name;
        }


    }
}
