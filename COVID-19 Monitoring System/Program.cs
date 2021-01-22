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


    }
}
