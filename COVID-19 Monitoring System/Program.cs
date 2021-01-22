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
            List<Person> personList = new List<Person>();
            List<BusinessLocation> businessList = new List<BusinessLocation>();

            LoadPersonData(personList);
            LoadBusinessLocation(businessList);

            //Task 3 
            List<Visitor> visitorList = new List<Visitor>();
            List<Resident> residentList = new List<Resident>();
            foreach (Person p in personList)
            {
                if (p is Visitor)
                {
                    Visitor v = (Visitor)p;
                    visitorList.Add(v);
                }
                else if (p is Resident)
                {
                    Resident r = (Resident)p;
                    residentList.Add(r);
                }
            }


            
            while (true)
            {

                Console.Write("\n[1] List person detail. \n[0] Exit \nChoice: ");
                string choice = Console.ReadLine();
                if (choice == "0")
                {
                    break;
                }

                else if (choice == "1")
                {
                    //DO EXCEPTION HANDLING
                    //Task 4.1
                    DisplayVisitors(visitorList);
                    DisplayResidents(residentList);
                    Console.Write("\nEnter person name: ");
                    string name = Console.ReadLine();

                    //Task 4.2
                    foreach (Person p in personList)
                    {
                        if (p.Name == name)
                        {
                            if (p is Visitor)
                            {
                                Console.WriteLine("Type: Visitor");
                                Visitor v = (Visitor)p;
                                Console.WriteLine(v.ToString());
                            }

                            else if (p is Resident)
                            {
                                Console.WriteLine("Type: Resident");
                                Resident r = (Resident)p;
                                Console.WriteLine(r.ToString());
                                if (r.Token != null)
                                {
                                    Console.WriteLine(r.Token.ToString());
                                }

                                //For resident who has a travelEntry
                                foreach (TravelEntry te in r.TravelEntryList)
                                {
                                    Console.WriteLine(te.ToString());
                                    if (te.ShnStay != null)
                                    {
                                        Console.WriteLine("Facility name: " + te.ShnStay.FacilityName);
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
        static List<SHNFacility> GetSHNFacilityDetail()
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

        static void LoadPersonData(List<Person> pList)
        {
            using (StreamReader sr = new StreamReader("Person.csv"))
            {
                string s = sr.ReadLine();
                string[] headers = s.Split(',');

              /*  Console.WriteLine("Person.csv");
                Console.WriteLine("{0, -8} {1, -8} {2, -20} {3, -16} {4, -11} {5, -12} {6, -11} {7, -25} {8, -15} {9, -25} {10, -15} {11, -16} {12, -16} {13, -20}",
                    headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6],
                    headers[7], headers[8], headers[9], headers[10], headers[11], headers[12], headers[13]);*/

                while ((s = sr.ReadLine()) != null)
                {
                    //Task 1.1
                    string[] items = s.Split(',');
               /*     Console.WriteLine("{0, -8} {1, -8} {2, -20} {3, -16} {4, -11} {5, -12} {6, -11} {7, -25} {8, -15} {9, -25} {10, -15} {11, -16} {12, -16} {13, -20}",
                        items[0], items[1], items[2], items[3], items[4], items[5], items[6],
                        items[7], items[8], items[9], items[10], items[11], items[12], items[13]);
*/
                    //Check the type of the Person, either visitor or resident
                    if (items[0] == "visitor")
                    {
                        Visitor visitor = new Visitor(items[4], items[5], items[1]);
                        pList.Add(visitor);

                    }


                    else if (items[0] == "resident")
                    {
                        Resident resident = new Resident(items[1], items[2], DateTime.ParseExact(items[3], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                        //Check whether the resident has any TraceTogetherToken
                        if (items[6] != "")
                        {
                            TraceTogetherToken ttt = new TraceTogetherToken(items[6], items[7], Convert.ToDateTime(items[8]));
                            resident.Token = ttt;

                        }


                        //Check whether the resident has any TravelEntry
                        if (items[9] != "")
                        {
                            TravelEntry te = new TravelEntry(items[9], items[10], DateTime.ParseExact(items[11], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
                            te.ShnEndDate = DateTime.ParseExact(items[12], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                            te.IsPaid = Convert.ToBoolean(items[13]);

                            //Check if theres facility name
                            if (items[14] != "")
                            {

                                List<SHNFacility> fList = GetSHNFacilityDetail();
                                foreach (SHNFacility f in fList)
                                {
                                    if (f.FacilityName == items[14])
                                    {
                                        te.AssignSHNFacility(f);
                                    }
                                }
                            }
                            resident.TravelEntryList.Add(te);
                        }
                        pList.Add(resident);

                    }

                }
            }
        }

        static void LoadBusinessLocation(List<BusinessLocation> bList)
        {
            using (StreamReader sr = new StreamReader("BusinessLocation.csv"))
            {
                string s = sr.ReadLine();

                while ((s = sr.ReadLine()) != null)
                {
                    string[] items = s.Split(',');
                    bList.Add(new BusinessLocation(items[0], items[1], Convert.ToInt32(items[2])));
                }
            }

        }


        static void DisplayVisitors(List<Visitor> vList)
        {
            Console.WriteLine("\n\nVisitors");
            Console.WriteLine("{0, -15} {1, -20} {2, -15}", "Name", "Passport No", "Nationality" );
            foreach (Visitor v in vList)
            {
                Console.WriteLine("{0, -15} {1, -20} {2, -15}", v.Name, v.PassportNo, v.Nationality);
            }
        }

        static void DisplayResidents(List<Resident> rList)
        {
            Console.WriteLine("\n\nResidents");
            Console.WriteLine("Name");
            foreach (Resident r in rList)
            {
                Console.WriteLine(r.Name);
            }

        }



    }
}
