//============================================================
// Student Number : S10202961, S10204388
// Student Name : Chen Han, Chung Tze Siong
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Text;

//CalculateTravelCost()
namespace COVID_19_Monitoring_System
{
    class SHNFacility
    {

        //attributes
        private string facilityName;

        public string FacilityName
        {
            get { return facilityName; }
            set { facilityName = value; }
        }

        private int facilityCapacity;

        public int FacilityCapacity
        {
            get { return facilityCapacity; }
            set { facilityCapacity = value; }
        }

        private int facilityVacancy;

        public int FacilityVacancy
        {
            get { return facilityVacancy; }
            set { facilityVacancy = value; }
        }

        private double distFromAirCheckpoint;

        public double DistFromAirCheckpoint
        {
            get { return distFromAirCheckpoint; }
            set { distFromAirCheckpoint = value; }
        }

        private double distFromSeaCheckpoint;

        public double DistFromSeaCheckpoint
        {
            get { return distFromSeaCheckpoint; }
            set { distFromSeaCheckpoint = value; }
        }

        private double distFromLandCheckpoint;

        public double DistFromLandCheckpoint
        {
            get { return distFromLandCheckpoint; }
            set { distFromLandCheckpoint = value; }
        }

        //methods
        public SHNFacility() { }

        public SHNFacility(string fn, int fc, double ac, double sc, double lc)
        {
            FacilityName = fn;
            FacilityCapacity = fc;
            DistFromAirCheckpoint = ac;
            DistFromSeaCheckpoint = sc;
            DistFromLandCheckpoint = lc;
        }

        //Add TravelEntry into CalculaeTravelCost
        public double CalculateTravelCost(string em, DateTime ed)
        {
            TravelEntry te = new TravelEntry();
            te.EntryMode = em;
            te.EntryDate = ed;
            return 0;
        }

        public bool IsAvailable()
        {
            if(FacilityVacancy > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return base.ToString() + "Facility Name: " + FacilityName + "\tFacility Capacity: " + FacilityCapacity + "\tFacility Vacancy: " + FacilityVacancy + 
                "\tDistance from Air Checkpoint: " + DistFromAirCheckpoint + "\tDistance from Sea Checkpoint: " + DistFromSeaCheckpoint + "\tDistance from Land Checkpoint: " + DistFromLandCheckpoint;
        }

    }
}
