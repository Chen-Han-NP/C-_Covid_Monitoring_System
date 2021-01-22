//============================================================
// Student Number : S10202961, S10204388
// Student Name : Chen Han, Chung Tze Siong
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Text;

//IsEligibleForReplacement()
namespace COVID_19_Monitoring_System
{
    class TraceTogetherToken
    {

        //attributes
        private string serialNo;

        public string SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }

        private string collectionLocation;

        public string CollectionLocation
        {
            get { return collectionLocation; }
            set { collectionLocation = value; }
        }

        private DateTime expiryDate;

        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }

        //methods
        public TraceTogetherToken() { }

        public TraceTogetherToken(string sn, string cl, DateTime xd)
        {
            SerialNo = sn;
            CollectionLocation = cl;
            ExpiryDate = xd;
        }

        public bool IsEligibleForReplacement()
        {
            DateTime currentDate = DateTime.Now;
            if ((ExpiryDate < currentDate) && (currentDate < ExpiryDate.AddMonths(1)))
                return true;
            else
                return false;
        }

        public void ReplaceToken(string sn, string cl)
        {
            SerialNo = sn;
            CollectionLocation = cl;
        }

        public override string ToString()
        {
            return "Serial No.: " + SerialNo + "\nCollection Location: " + CollectionLocation + "\nExpiry Date: " + ExpiryDate;
        }

    }
}
