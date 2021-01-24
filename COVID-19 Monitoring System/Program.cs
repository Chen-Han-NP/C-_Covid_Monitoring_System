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

5) Assign/Replace TraceTogether Token
    3. create and assign a TraceTogetherToken object if resident has no existing (Collection location????)
//Duplicated name????? Do we need to check for the same name exist in the list

12) Try except for DisplayVisitors list (the Facility name one);
13) 
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
            List<String> serialNums = new List<String>();
            List<SHNFacility> SHNList = GetSHNFacilityDetail();

            foreach (Person p in personList)
            {
                if (p is Visitor v)
                    visitorList.Add(v);
                else if (p is Resident r)
                {
                    residentList.Add(r);
                    if (r.Token != null)
                        serialNums.Add(r.Token.SerialNo);
                }
            }


            while (true)
            {
                Console.Write("\n========COVID-19 Monitoring System========\n[1] Display all Visitors and Residents\n[2] List Person Details\n[3] Assign/Replace TraceTogether Token \n[4] Display all Business Locations " +
                    "\n[5] Edit Business Location Capacity\n[6] Display all SafeEntry records\n[7] Perform SafeEntry Check-In \n[8] Perform SafeEntry Check-out \n[9] Display all SHN facilities \n[10] Add Visitor" +
                    "\n[11] Create a new Travel Entry Record \n[0] Exit \nChoice: ");
                string choice = Console.ReadLine();


                if (choice == "0")
                    break;


                /*-----------------Task 3---------------------*/
                else if (choice == "1")
                {
                    DisplayVisitors(visitorList);
                    DisplayResidents(residentList);
                }



                //DO EXCEPTION HANDLING
                /*-----------------Task 4---------------------*/
                else if (choice == "2")
                {
                    Console.Write("\nEnter person name: ");
                    string name = Console.ReadLine();
                    //Get the PersonIndex from the method
                    int personIndex = FindPerson(name, personList);
                    if (personIndex == -1)
                        Console.WriteLine("Invalid input or the Person name is not found!");
                    else
                    {
                        Person p = personList[personIndex];
                        if (p is Visitor visitor)
                        {
                            Console.WriteLine("Type: Visitor");
                            Console.WriteLine(visitor.ToString());

                            //For visitor who has a travelEntry
                            foreach (TravelEntry te in visitor.TravelEntryList)
                            {
                                Console.WriteLine(te.ToString());
                                if (te.ShnStay != null)
                                    Console.WriteLine("Facility name: " + te.ShnStay.FacilityName);
                            }
                        }
                        else if (p is Resident resident)
                        {
                            Console.WriteLine("Type: Resident");
                            Console.WriteLine(resident.ToString());
                            if (resident.Token != null)
                                Console.WriteLine(resident.Token.ToString());

                            //For resident who has a travelEntry
                            foreach (TravelEntry te in resident.TravelEntryList)
                            {
                                Console.WriteLine(te.ToString());
                                if (te.ShnStay != null)
                                    Console.WriteLine("Facility name: " + te.ShnStay.FacilityName);
                            }
                        }
                    }
                }



                /*-----------------Task 5---------------------*/
                else if (choice == "3")
                {
                    Console.Write("\nEnter resident name: ");
                    string name = Console.ReadLine();
                    foreach (Resident r in residentList)
                    {
                        if (r.Name == name)
                        {
                            DateTime currentDate = DateTime.Now;
                            if (r.Token != null)
                            {
                                if (r.Token.IsEligibleForReplacement() == true)
                                {
                                    r.Token.ReplaceToken(r.Token.SerialNo, r.Token.CollectionLocation);
                                    Console.WriteLine("{0} is eligible for replacement! \nThe new expiry date is {1}", r.Name, r.Token.ExpiryDate);
                                }
                                else
                                {

                                    if (currentDate > r.Token.ExpiryDate.AddMonths(1))
                                        Console.WriteLine("{0} is not eligible for replacement as the token has exceeded the expiry date for more than one month.", r.Name);
                                    else
                                        Console.WriteLine("{0} is not eligible for replacement as the token has not expired yet.", r.Name);
                                }
                            }
                            else
                            {
                                Console.WriteLine("{0} has no token, assigning new token...", r.Name);
                                String newSerialNo = GetRandomSerialNo(serialNums);
                                serialNums.Add(newSerialNo);

//!!!!!!!!!!!!!!!//ASK CHER
                                string newCL = "(idk)";
                                DateTime newExpiryDate = currentDate.AddMonths(6);
                                TraceTogetherToken token = new TraceTogetherToken(newSerialNo, newCL, newExpiryDate);

                                r.Token = token;


                            }
                        }
                    }
                }



                /*-----------------Task 6---------------------*/
                else if (choice == "4")
                    DisplayBusinessLocation(businessList);




                /*-----------------Task 7---------------------*/
                else if (choice == "5")
                {
                    bool found = false;
                    DisplayBusinessLocation(businessList);
                    Console.Write("\nEnter Business Name to search: ");
                    string businessName = Console.ReadLine();
                    foreach (BusinessLocation bl in businessList)
                    {
                        if (bl.BusinessName == businessName)
                        {
                            //DO EXCEPTION HANDLING
                            found = true;
                            Console.Write("{0} found!\nPlease enter the new Maximum Capcity: ", bl.BusinessName);
                            int newMax = Convert.ToInt32(Console.ReadLine());
                            bl.MaximumCapacity = newMax;
                            DisplayBusinessLocation(businessList);
                            break;
                        }
                    }
                    if (!found)
                        Console.WriteLine("Invalid input or the Business name is not found!");
                }



                else if (choice == "6")
                {
                    DisplaySafeEntryRecords(personList);
                }



                /*-----------------Task 8---------------------*/
                else if (choice == "7")
                {
                    Console.Write("Enter person name: ");
                    string name = Console.ReadLine();
                    int personIndex = FindPerson(name, personList);
                    
                    if (personIndex == -1)
                        Console.WriteLine("Invalid input or the Person name is not found!");
                    else
                    {
                        Person p = personList[personIndex];
                        bool businessFound = false;
                        while (true)
                        {
                            Console.WriteLine("{0} found!", p.Name);
                            DisplayBusinessLocation(businessList);
                            Console.Write("Please select a Business Location to check-in: ");
                            string bName = Console.ReadLine();
                            foreach (BusinessLocation b in businessList)
                            {
                                if (b.BusinessName == bName)
                                {
                                    businessFound = true;
                                    if (!b.isFull())
                                    {
                                        DateTime dt = new DateTime();
                                        SafeEntry se = new SafeEntry(DateTime.Now, dt, b);

                                        p.AddSafeEntry(se);
                                        b.VisitorsNow += 1;
                                        Console.WriteLine("{0} has checked in to the {1}.", p.Name, b.BusinessName);
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("{0} is at Max Capacity.", bName);
                                    }
                                }
                            }
                            if (!businessFound)
                                Console.WriteLine("'{0}' not found. Please try again.", bName);
                            else
                                break;
                        }
                    }
                }


                /*-----------------Task 9---------------------*/
                else if (choice == "8")
                {
                    Console.Write("Enter person name: ");
                    string name = Console.ReadLine();
                    int personIndex = FindPerson(name, personList);
                    if (personIndex == -1)
                        Console.WriteLine("Invalid input or the Person name is not found!");
                    else
                    {
                        bool isThereCheckedOut = false;
                        Person p = personList[personIndex];
                        List<Int32> recordNumList = new List<Int32>();

                        //Check whether this person has any SafeEntry that has not been checked out yet.
                        Console.WriteLine("\n<---SafeEntry Records for {0}--->", p.Name);

                        for (int i = 0; i < p.SafeEntryList.Count; i++)
                        {
                            DateTime dt = new DateTime();
                            if (p.SafeEntryList[i].CheckOut == dt)
                            {
                                isThereCheckedOut = true;
                                recordNumList.Add(i + 1);
                                Console.WriteLine("Record No: {0}", i + 1);
                                Console.WriteLine("{0, -20} {1, -20} {2, -20}", "Check-In", "Check-Out", "Business Location");
                                Console.WriteLine("{0, -20} {1, -20} {2, -20}", p.SafeEntryList[i].CheckIn, p.SafeEntryList[i].CheckOut, p.SafeEntryList[i].Location.BusinessName);
                                Console.WriteLine();

                            }
                        }

                        if (!isThereCheckedOut)
                            Console.WriteLine("Sorry, no SafeEntry record that has not been checked out yet!");
                        else
                        {
                            Console.Write("\nPlease enter which record to check-out: ");
                            //EXCEPTION HANDLING
                            int result = Convert.ToInt32(Console.ReadLine());
                            //Check whether the number has exceeded the size of the SafeEntryList
                            if (result <= p.SafeEntryList.Count)
                            {
                                //Now check whether the user has entered the correct Record No given in the list above.
                                if (recordNumList.Contains(result))
                                {
                                    p.SafeEntryList[result - 1].PerformCheckOut();
                                    Console.WriteLine("{0} has checked out from the {1}.", p.Name, p.SafeEntryList[result - 1].Location.BusinessName);
                                }
                                else
                                    Console.WriteLine("Please do not enter Record No that has already been checked out!");

                            }
                            else
                                Console.WriteLine("No such record found! ");
                        }
                    }

                }


                /*-----------------Task 10---------------------*/
                else if (choice == "9")
                    DisplaySHNFacilities(SHNList);
                


                /*-----------------Task 11---------------------*/
                else if (choice == "10")
                {
//Duplicated name????? Do we need to check for the same name exist in the list
                    Console.Write("Name: ");
                    string name = Console.ReadLine();
                    Console.Write("Passport No.: ");
                    string passport = Console.ReadLine();
                    Console.Write("Nationality: ");
                    string nationality = Console.ReadLine();

                    visitorList.Add(new Visitor(passport, nationality, name));
                    personList.Add(new Visitor(passport, nationality, name));

                }


                /*-----------------Task 12---------------------*/
                else if (choice == "11")
                {
                    Console.Write("Enter person name: ");
                    string name = Console.ReadLine();
                    int personIndex = FindPerson(name, personList);
                    if (personIndex == -1)
                        Console.WriteLine("Invalid input or Person name is not found!");
                    else
                    {
                        Person p = personList[personIndex];
                        Console.Write("Enter your last country of embarkation: ");
                        string lastCountryTravelled = Console.ReadLine();
                        Console.Write("Enter your entry mode(Air/Sea/Land): ");
                        string entryMode = Console.ReadLine();
                        TravelEntry newTravelEntry = new TravelEntry(lastCountryTravelled, entryMode, DateTime.Now);
                        newTravelEntry.CalculateSHNDuration();

                        List<String> countriesList = new List<String> { "Vietnam", "New Zealand", "Macao SAR" };
                        if (!countriesList.Contains(newTravelEntry.LastCoutryOfEmbarkation))
                        {

                            DisplaySHNFacilities(SHNList);
                            Console.Write("Please select a SHNFacility from above: ");
                            string fName = Console.ReadLine();
                            bool shnFound = false;
                            foreach (SHNFacility f in SHNList)
                            {
                                if (fName == f.FacilityName)
                                {
                                    if (f.IsAvailable())
                                    {
                                        newTravelEntry.AssignSHNFacility(f);
                                        f.FacilityVacancy -= 1;
                                    }
                                    else
                                    {
                                        Console.WriteLine("There is no vacant slots in facility {0}", f.FacilityName);
                                    }
                                }
                            }
                            if (!shnFound)
                                Console.WriteLine("Facility name not found!");
                        }

                        p.AddTravelEntry(newTravelEntry);
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

        //Display the SHN Facilities
        static void DisplaySHNFacilities(List<SHNFacility> fList)
        {
            Console.WriteLine("\nSHN Facilities");
            Console.WriteLine("{0, -20}{1, -20}{2, -20}{3, -25}{4, -25}{5, -27}", "Facility Name", "Facility Capacity", "Facility Vacancy", "Dist from air checkpoint", "Dist from sea checkpoint", "Dist from land checkpoint");
            foreach(SHNFacility f in fList)
            {
                Console.WriteLine("{0, -20}{1, -20}{2, -20}{3, -25}{4, -25}{5, -27}", f.FacilityName, f.FacilityCapacity, f.FacilityVacancy, f.DistFromAirCheckpoint, f.DistFromSeaCheckpoint, f.DistFromLandCheckpoint);
            }
        }

        //This method takes in person name and the personList, return either the index of the Person in the list OR a -1 value means that its not found.
        static int FindPerson(string n, List<Person> pList)
        {
            bool personFound = false;
            for (int i = 0; i < pList.Count; i++)
            {
                if (pList[i].Name == n)
                {
                    personFound = true;
                    return i;
                }
            }
            if (!personFound)
            {
                return -1;
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
                        //Check whether the visitor has any TravelEntry
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
                                    //If the Visitor has a facility name 
                                    if (f.FacilityName == items[14])
                                    {
                                        f.FacilityVacancy -= 1;
                                        te.AssignSHNFacility(f);
                                    }
                                }
                            }
                            visitor.AddTravelEntry(te);
                        }
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
                                        f.FacilityVacancy -= 1;
                                        te.AssignSHNFacility(f);
                                    }
                                }
                            }
                            resident.AddTravelEntry(te);
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

        static void DisplayBusinessLocation(List<BusinessLocation> bList)
        {
            Console.WriteLine("\nBusinessLocations");
            Console.WriteLine("{0, -25} {1, -14} {2, -20}", "Business Name", "Branch Code", "Maximum Capacity");

            foreach (BusinessLocation bl in bList)
            {
                Console.WriteLine("{0, -25} {1, -14} {2, -20}", bl.BusinessName, bl.BranchCode, bl.MaximumCapacity);
            }
        }
            

            


        static void DisplayVisitors(List<Visitor> vList)
        {
            Console.WriteLine("\n\nVisitors without TravelEntry");
            Console.WriteLine("{0, -15} {1, -20} {2, -15}", "Name", "Passport No", "Nationality");


            foreach (Visitor v in vList)
            {
                 Console.WriteLine("{0, -15} {1, -20} {2, -15}", v.Name, v.PassportNo, v.Nationality);
                   
                
            }
            
            Console.WriteLine("\nVisitors with TravelEntries");
            foreach (Visitor v in vList)
            {
                if (v.TravelEntryList.Count > 0)
                {
                    Console.WriteLine("\nTravelEntry for {0}", v.Name);
                    Console.WriteLine("{0,-17} {1, -10} {2, -22} {3, -22} {4, -14} {5, -15}", "TE Last Country", "TE Mode", "TravelEntry Date", "TravelSHN EndDate", "Travells Paid", "Facility Name");
                    foreach (TravelEntry te in v.TravelEntryList)
                    {
                        //USE TRY CATCH FOR THIS
                        Console.WriteLine("{0,-17} {1, -10} {2, -22} {3, -22} {4, -14} {5, -15}", te.LastCoutryOfEmbarkation, te.EntryMode, te.EntryDate, te.ShnEndDate, te.IsPaid, te.ShnStay.FacilityName);
                    }
                }
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

        static String GetRandomSerialNo(List<String> sList)
        {
            while (true)
            {
                Random random = new Random();
                int randomNum = random.Next(10000, 99999);
                String output = "T" + randomNum;
                if (!sList.Contains(output))
                {
                    return output;
                }

            }

        }

        static void DisplaySafeEntryRecords(List<Person> pList)
        {
            bool isEmpty = true;

            Console.WriteLine("\n<---SafeEntry Records for everyone--->");
            foreach (Person p in pList)
            {
                if (p.SafeEntryList.Count > 0)
                {
                    isEmpty = false;
                    Console.WriteLine("For person: " + p.Name + ": ");
                    Console.WriteLine("{0, -20} {1, -20} {2, -20}", "Check-In", "Check-Out", "Business Location");
                    foreach(SafeEntry se in p.SafeEntryList)
                    {
                        Console.WriteLine("{0, -20} {1, -20} {2, -20}", se.CheckIn, se.CheckOut, se.Location.BusinessName);

                    }
                }
            }

            if (isEmpty)
            {
                Console.WriteLine("No records found!");
            }

        }
            

    }
}
