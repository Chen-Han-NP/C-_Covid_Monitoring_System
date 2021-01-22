//============================================================
// Student Number : S10202961, S10204388
// Student Name : Chen Han, Chung Tze Siong
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Globalization;
using Newtonsoft.Json;

/* TO DO 
1) Load Person and Business Location Data
    1. load both given CSV and populate two lists
2) Load SHN Facility Data
    1. call API and populate a list
3) List all Visitors
4) List Person Details
    1. prompt user for name
    2. search for person
    3. list person details including TravelEntry and SafeEntry details
        i. if resident, display TraceTogetherToken details
*/


namespace COVID_19_Monitoring_System
{
    class Program
    {
        static void Main(string[] args)
        {
            //Task 1.1
            displayPersonData();
            displayBusinessLocation();


            //Make a list to store Resident and Visitor
            List<Person> personList = new List<Person>();


            using (StreamReader sr = new StreamReader("Person.csv"))
            {
                string s = sr.ReadLine();

                while ((s = sr.ReadLine()) != null)
                {
                    string[] items = s.Split(',');

                    //Check the type of the Person, either visitor or resident
                    if (items[0] == "resident")
                    {
                        Resident resident = new Resident(items[1], items[2], DateTime.ParseExact(items[3], "dd/MM/yyyy", CultureInfo.InvariantCulture));

                        //Check whether the resident has any TravelEntry
                        if (items[9] != "")
                        {
                            TravelEntry te = new TravelEntry(items[9], items[10], DateTime.ParseExact(items[11], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
                            //Check if theres facility name
                            if (items[14] != "")
                            {

                                List<SHNFacility> fList = getSHNFacilityDetail();
                                foreach (SHNFacility f in fList)
                                {
                                    if (f.FacilityName == items[14])
                                    {
                                        te.AssignSHNFacility(f);
                                    }
                                }
                            }
                        }



                    }
                }
            }

        }

        //Task 2.1
        //Make a methdo that calls the API and search for the detail of the SHN facility using the name
        static List<SHNFacility> getSHNFacilityDetail()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://covidmonitoringapiprg2.azurewebsites.net");
                Task<HttpResponseMessage> responseTask = client.GetAsync("/facility");
                responseTask.Wait();

                HttpResponseMessage result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Task<string> readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    string data = readTask.Result;

                    List<SHNFacility> shnList = JsonConvert.DeserializeObject<List<SHNFacility>>(data);
                    return shnList;

                }
                else
                {
                    return null;
                }
            }


        }

        static void displayPersonData()
        {
            using (StreamReader sr = new StreamReader("Person.csv"))
            {
                string s = sr.ReadLine();
                string[] headers = s.Split(',');

                Console.WriteLine("Person.csv");
                Console.WriteLine("{0, -8} {1, -8} {2, -20} {3, -16} {4, -11} {5, -12} {6, -11} {7, -25} {8, -15} {9, -25} {10, -15} {11, -16} {12, -16} {13, -20}",
                    headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6],
                    headers[7], headers[8], headers[9], headers[10], headers[11], headers[12], headers[13]);

                while ((s = sr.ReadLine()) != null)
                {
                    //Task 1.1
                    string[] items = s.Split(',');
                    Console.WriteLine("{0, -8} {1, -8} {2, -20} {3, -16} {4, -11} {5, -12} {6, -11} {7, -25} {8, -15} {9, -25} {10, -15} {11, -16} {12, -16} {13, -20}",
                        items[0], items[1], items[2], items[3], items[4], items[5], items[6],
                        items[7], items[8], items[9], items[10], items[11], items[12], items[13]);
                }
            }
        }

        static void displayBusinessLocation()
        {
            using (StreamReader sr = new StreamReader("BusinessLocation.csv"))
            {
                string s = sr.ReadLine();
                string[] headers = s.Split(',');

                Console.WriteLine("\nBusinessLocation.csv");
                Console.WriteLine("{0, -25} {1, -14} {2, -20}", headers[0], headers[1], headers[2]);

                while ((s = sr.ReadLine()) != null)
                {
                    //Task 1.1
                    string[] items = s.Split(',');
                    Console.WriteLine("{0, -25} {1, -14} {2, -20}", items[0], items[1], items[2]);
                }
            }
        }
    }
}
