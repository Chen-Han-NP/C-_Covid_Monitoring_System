//============================================================
// Student Number : S10202961, S10204388
// Student Name : Chen Han, Chung Tze Siong
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Text;

//Advanced Feature: Added in another class variable amountDue to keep track of the SHN charges due of a person.
namespace COVID_19_Monitoring_System
{
    abstract class Person
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        private double amountDue;

        public double AmountDue
        {
            get { return amountDue; }
            set { amountDue = value; }
        }



        private List<SafeEntry> safeEntryList;  

        public List<SafeEntry> SafeEntryList
        {
            get { return safeEntryList; }
            set { safeEntryList = value; }
        }

        private List<TravelEntry> travelEntryList;  

        public List<TravelEntry> TravelEntryList
        {
            get { return travelEntryList; }
            set { travelEntryList = value; }
        }

        public Person()
        {

        }

        public Person(string n)
        {
            Name = n;
            SafeEntryList = new List<SafeEntry>();
            TravelEntryList = new List<TravelEntry>();
            
        }

        public void AddTravelEntry(TravelEntry te)
        {
            TravelEntryList.Add(te);
        }

        public void AddSafeEntry(SafeEntry se)
        {
            SafeEntryList.Add(se);
        }

        abstract public double CalculateSHNCharges();

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
