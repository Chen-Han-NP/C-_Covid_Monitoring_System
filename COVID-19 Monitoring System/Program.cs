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

/*Assumption(s)
 * Task 11
 *    1.  Cannot add a visitor if a duplicated name is entered.
 *    2.  Passport number for visitors is unique though the name is different.
 * 
*/

/* TO DO 

5) Assign/Replace TraceTogether Token
    3. create and assign a TraceTogetherToken object if resident has no existing (Collection location????)

11) Duplicated visitor name????? Do we need to check for the same name exist in the list
12) Travel entry record. Can we have more than one travel entry record for a person?

12) Try except for DisplayVisitors list (the Facility name one);

Update UI for choice 1, need to add Resident details and Residents with Travel records
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

            List<SHNFacility> SHNList = GetSHNFacilityDetail();
            LoadPersonData(personList, SHNList);
            LoadBusinessLocation(businessList);

            

            //Task 3 
            List<Visitor> visitorList = new List<Visitor>();
            List<Resident> residentList = new List<Resident>();
            List<String> serialNums = new List<String>();
            

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
                Console.Write("\n========COVID-19 Monitoring System========\n\n<=======For all Visitors/Residents=======>\n[1] Display all Visitors and Residents\n[2] Search & List Person Details\n[3] Assign/Replace TraceTogether Token \n\n<=========For Business Locations=========>\n[4] Display all Business Locations " +
                    "\n[5] Edit Business Location Capacity\n\n<============For Safe Entries============>\n[6] Display all SafeEntry records\n[7] Perform SafeEntry Check-In \n[8] Perform SafeEntry Check-out \n\n<===========For Travel Entries===========>\n[9]  Display all SHN facilities \n[10] Add Visitor" +
                    "\n[11] Create a new Travel Entry Record \n[12] Calculate SHN Charges \n[0]  Exit \n\nOption: ");
                string choice = Console.ReadLine();


                if (choice == "0")
                    break;


                /*-----------------Task 3---------------------*/
                else if (choice == "1")
                {
                    DisplayVisitors(visitorList);
                    Console.WriteLine("\n||===========================================================||");
                    DisplayResidents(residentList);
                }



                //DO EXCEPTION HANDLING
                /*-----------------Task 4---------------------*/
                else if (choice == "2")
                {
                    SearchPerson(personList);
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
                                    Console.WriteLine("{0}'s Token is eligible for replacement! \nThe new expiry date is {1}", r.Name, r.Token.ExpiryDate);
                                }
                                else
                                {

                                    if (currentDate > r.Token.ExpiryDate.AddMonths(1))
                                        Console.WriteLine("{0}'s Token is not eligible for replacement as the token has exceeded the expiry date for more than one month.", r.Name);
                                    else
                                        Console.WriteLine("{0}'s Token is not eligible for replacement as the token has not expired yet.", r.Name);
                                }
                            }
                            else
                            {
                                Console.WriteLine("{0} has no token, assigning new token...", r.Name);
                                String newSerialNo = GetRandomSerialNo(serialNums);
                                serialNums.Add(newSerialNo);

                                Console.Write("Please enter your collection location: ");
                                string newCL = Console.ReadLine();
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
                    int businessNo = -1;
                    DisplayBusinessLocation(businessList);
                    while (true)
                    {
                        try
                        {
                            Console.Write("\nEnter Business No. to edit: ");
                            businessNo = Convert.ToInt32(Console.ReadLine());
                            break;
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine(ex.Message + "\nPlease try again.");
                        }
                    }
                    for (int i = 0; i < businessList.Count; i++)
                    {
                        if (i + 1 == businessNo)
                        {
                            found = true;
                            int newMax = 0;
                            Console.WriteLine("{0} found!", businessList[i].BusinessName);
                            while (true)
                            {
                                try
                                {
                                    Console.Write("\nPlease enter the new Maximum Capcity: ");
                                    newMax = Convert.ToInt32(Console.ReadLine());
                                    break;
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message + "\nPlease try again.");
                                }
                            }
                            businessList[i].MaximumCapacity = newMax;
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
                        Console.WriteLine("{0} found!", p.Name);
                        while (true)
                        {
                            DisplayBusinessLocation(businessList);
                            try
                            {
                                Console.Write("Please select a Business Location No. to check-in: ");
                                int businessNo = Convert.ToInt32(Console.ReadLine());
                                bool duplicatedCheckedIn = false;

                                for (int i = 0; i < businessList.Count; i++)
                                {
                                    if (i + 1 == businessNo)
                                    {
                                        //Check whether the business name is in the safeentry list of this person
                                        foreach (SafeEntry se in p.SafeEntryList)
                                        {
                                            if (se.Location.BusinessName == businessList[i].BusinessName && se.CheckOut == new DateTime())
                                            {
                                                duplicatedCheckedIn = true;
                                                businessFound = true;
                                            }
                                        }
                                        if (duplicatedCheckedIn == true)
                                        {
                                            Console.WriteLine("The business {0} has already checked in but yet to check out!\nPlease check out before you check in again! ", businessList[i].BusinessName);
                                            break;
                                        }
                                        businessFound = true;
                                        if (!businessList[i].isFull())
                                        {
                                            DateTime dt = new DateTime();
                                            SafeEntry se = new SafeEntry(DateTime.Now, dt, businessList[i]);

                                            p.AddSafeEntry(se);
                                            businessList[i].VisitorsNow += 1;
                                            Console.WriteLine("{0} has checked in to the {1}.", p.Name, businessList[i].BusinessName);
                                            break;
                                        }
                                        else
                                            Console.WriteLine("{0} is at Max Capacity.", businessList[i].BusinessName);
                                    }
                                }
                                if (!businessFound)
                                    Console.WriteLine("The Business No. does not exist in the list. Please try again.");
                                else
                                    break;
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
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
                                Console.WriteLine("{0, -25} {1, -25} {2, -25}", "Check-In", "Check-Out", "Business Location");
                                if (p.SafeEntryList[i].CheckOut == new DateTime())
                                    Console.WriteLine("{0, -25} {1, -25} {2, -20}", p.SafeEntryList[i].CheckIn, "N/A", p.SafeEntryList[i].Location.BusinessName);

                                else
                                    Console.WriteLine("{0, -25} {1, -25} {2, -25}", p.SafeEntryList[i].CheckIn, p.SafeEntryList[i].CheckOut, p.SafeEntryList[i].Location.BusinessName);

                                Console.WriteLine();

                            }
                        }

                        if (!isThereCheckedOut)
                            Console.WriteLine("Sorry, no SafeEntry record that has not been checked out yet!");
                        else
                        {
                            int result = 0;
                            while (true)
                            {
                                try
                                {
                                    Console.Write("\nPlease enter which record to check-out: ");
                                    result = Convert.ToInt32(Console.ReadLine());
                                    break;
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message + "\nPlease try again.");
                                }
                            }
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
                    DisplayVisitors(visitorList);
                    
                    //Check whether the name is duplicated in the visitorlist

                    while (true)
                    {
                        Console.Write("\nEnter new visitor name: ");
                        string name = Console.ReadLine();
                        bool isNameDuplicated = false;
                        bool isPassportDuplicated = false;

                        foreach (Visitor v in visitorList)
                        {
                            if (v.Name == name)
                            {
                                isNameDuplicated = true;
                                Console.WriteLine("{0} has already existed in the list. Please try again!", v.Name);
                            }
                        }
                        if (isNameDuplicated)
                            break;


                        Console.Write("Enter passport No.: ");
                        string passport = Console.ReadLine();
                        foreach (Visitor v in visitorList)
                        {
                            if (v.PassportNo == passport)
                            {
                                isPassportDuplicated = true;
                                Console.WriteLine("The passport number is duplicated! Please try again!");
                            }
                        }
                        if (isPassportDuplicated)
                            break;

                        Console.Write("Enter nationality: ");
                        string nationality = Console.ReadLine();

                        visitorList.Add(new Visitor(passport, nationality, name));
                        personList.Add(new Visitor(passport, nationality, name));
                        Console.WriteLine("Visitor {0} is added to the list successfully!", name);
                        break;

                    }

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
                        bool isEligibleForTE = true;
                        //Now check whether the person is eligible for a new Travel Entry
                            
                        if (p.TravelEntryList.Count > 0)
                        {
                            foreach (TravelEntry te in p.TravelEntryList)
                            {
                                if (te.ShnEndDate > DateTime.Now)
                                {
                                    isEligibleForTE = false;
                                    Console.WriteLine("{0} stay has not ended yet!", p.Name);
                                    break;
                                }
                                
                                if (!te.IsPaid)
                                {
                                    isEligibleForTE = false;
                                    Console.WriteLine("{0} has not paid for the previous fees!", p.Name);
                                    break;
                                }
                            }
                        }
                        if (isEligibleForTE)
                        {
                            List<String> modeList = new List<String> { "Air", "Sea", "Land" };
                            List<String> countriesList = new List<String> { "Vietnam", "New Zealand", "Macao SAR" };

                            
                            Console.Write("Enter your last country of embarkation: ");
                            string lastCountryTravelled = Console.ReadLine();
                            Console.Write("Enter your entry mode(Air/Sea/Land): ");
                            string entryMode = Console.ReadLine();
                            while (!modeList.Contains(entryMode))
                            {
                                Console.WriteLine("Please enter a valid entry mode!");
                                Console.Write("Enter your entry mode(Air/Sea/Land): ");
                                entryMode = Console.ReadLine();
                                if (modeList.Contains(entryMode))
                                    break;
                            }


                            if (!countriesList.Contains(lastCountryTravelled))
                            {

                                DisplaySHNFacilities(SHNList);
                                try
                                {

                                    Console.Write("Please select a SHNFacility No. from above: ");
                                    int fIndex = Convert.ToInt32(Console.ReadLine());
                                    bool shnFound = false;
                                    
                                    foreach (SHNFacility f in SHNList)
                                    {
                                        if (SHNList[fIndex-1].FacilityName == f.FacilityName)
                                        {
                                            if (f.IsAvailable())
                                            {
                                                shnFound = true; 
                                                TravelEntry newTravelEntry = new TravelEntry(lastCountryTravelled, entryMode, DateTime.Now);
                                                newTravelEntry.CalculateSHNDuration();
                                                p.AddTravelEntry(newTravelEntry);
                                                newTravelEntry.AssignSHNFacility(f);
                                                f.FacilityVacancy -= 1;
                                                Console.WriteLine("{0} is available! {1} has checked in.", f.FacilityName, p.Name);
                                            }
                                        }
                                    }
                                    if (!shnFound)
                                        Console.WriteLine("There is no vacant slots in facility {0}!", SHNList[fIndex-1].FacilityName);

                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine("Please enter a valid number from above!");
                                }

                            }

                            else
                            {
                                TravelEntry newTravelEntry = new TravelEntry(lastCountryTravelled, entryMode, DateTime.Now);
                                newTravelEntry.CalculateSHNDuration();
                                p.AddTravelEntry(newTravelEntry);
                                Console.WriteLine("A new travel entry has successfully added!");
                            }
                            
                        }
                        else
                        {
                            Console.WriteLine("Not eligible to add Travel Entry");
                        }
                    }
                }


                //Task 12
                else if (choice == "12")
                {
                    Console.Write("Enter person name: ");
                    string name = Console.ReadLine();
                    int personIndex = FindPerson(name, personList);
                    if (personIndex == -1)
                        Console.WriteLine("Invalid input or Person name is not found!");
                    else
                    {
                        Person p = personList[personIndex];
                        //Display the Person's travel entry info
                        if (p.TravelEntryList.Count > 0)
                        {
                            if (p is Visitor visitor)
                            {
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
                                //For resident who has a travelEntry
                                foreach (TravelEntry te in resident.TravelEntryList)
                                {
                                    Console.WriteLine(te.ToString());
                                    if (te.ShnStay != null)
                                        Console.WriteLine("Facility name: " + te.ShnStay.FacilityName);
                                }
                            }
                        }
                        else
                            Console.WriteLine("You do not have anything to pay.");
                        Console.WriteLine();
                        

                        double amountToPay = p.CalculateSHNCharges();
                        if (amountToPay > 0)
                        {
                            Console.WriteLine("Total amount to pay: ${0}.", amountToPay.ToString("#0.00"));
                            Console.Write("Make payment now? (Y/N): ");
                            string payNow = Console.ReadLine();
                            if (payNow == "Y")
                            {
                                foreach (TravelEntry te in p.TravelEntryList)
                                {
                                    te.IsPaid = true;
                                }
                                Console.WriteLine("Payment successful.");
                            }
                            else if (payNow == "N")
                            {
                                Console.WriteLine("Payment not made.");
                            }
                        }
                        if (amountToPay == 0)
                        {
                            Console.WriteLine("You do not have to pay anything at the moment!");
                        }
                    }

                }

                else
                {
                    Console.WriteLine("Please enter a valid choice number!");
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
                    foreach (SHNFacility f in shnList)
                    {
                        f.FacilityVacancy = f.FacilityCapacity;
                    }
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
            Console.WriteLine("{0, -5}{1, -20}{2, -20}{3, -20}{4, -20}{5, -20}{6, -20}", "No.", "Facility Name", "Facility Capacity", "Facility Vacancy", "Dist from air C.P.", "Dist from sea C.P.", "Dist from land C.P. (km)");
            for (int i = 0; i < fList.Count; i++)
            {
                Console.WriteLine("{0, -5}{1, -20}{2, -20}{3, -20}{4, -20}{5, -20}{6, -20}", i+1 , fList[i].FacilityName, fList[i].FacilityCapacity, fList[i].FacilityVacancy, fList[i].DistFromAirCheckpoint, fList[i].DistFromSeaCheckpoint, fList[i].DistFromLandCheckpoint);
            }
        }

        //This method takes in person name and the personList, return either the index of the Person in the list OR a -1 value means that its not found.
        static int FindPerson(string n, List<Person> pList)
        {
            for (int i = 0; i < pList.Count; i++)
            {
                if (pList[i].Name == n)
                {
                    return i;
                }
            }
            return -1;
        }

        

        static void LoadPersonData(List<Person> pList, List<SHNFacility> fList)
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
            Console.WriteLine("\n<-----Business Locations----->");
            Console.WriteLine("{0, -4} {1, -25} {2, -14} {3, -20} {4, -20}", "No.", "Business Name", "Branch Code", "No. of Visitors Now", "Maximum Capacity");

            for (int i = 0; i < bList.Count; i++)
            {
                Console.WriteLine("{0, -4} {1, -25} {2, -14} {3, -20} {4, -20}", i+1, bList[i].BusinessName, bList[i].BranchCode, bList[i].VisitorsNow, bList[i].MaximumCapacity);
            }
        }
            


        static void DisplayVisitors(List<Visitor> vList)
        {
            Console.WriteLine("\n<----------Visitors List----------->");

            Console.WriteLine("{0, -15} {1, -20} {2, -15}", "Name", "Passport No", "Nationality");
            foreach (Visitor v in vList)
            {
                 Console.WriteLine("{0, -15} {1, -20} {2, -15}", v.Name, v.PassportNo, v.Nationality);
            }
            
            Console.WriteLine("\n<Visitors with TravelEntries>");
            foreach (Visitor v in vList)
            {
                if (v.TravelEntryList.Count > 0)
                {
                    Console.WriteLine("\nTravelEntry for {0}", v.Name);
                    Console.WriteLine("{0,-17} {1, -10} {2, -22} {3, -22} {4, -14} {5, -15}", "TE Last Country", "TE Mode", "TravelEntry Date", "TravelSHN EndDate", "Travells Paid", "Facility Name");
                    foreach (TravelEntry te in v.TravelEntryList)
                    {
                        if (te.ShnStay == null)
                            Console.WriteLine("{0,-17} {1, -10} {2, -22} {3, -22} {4, -14} {5, -15}", te.LastCoutryOfEmbarkation, te.EntryMode, te.EntryDate, te.ShnEndDate, te.IsPaid, "Nil");

                        else
                            Console.WriteLine("{0,-17} {1, -10} {2, -22} {3, -22} {4, -14} {5, -15}", te.LastCoutryOfEmbarkation, te.EntryMode, te.EntryDate, te.ShnEndDate, te.IsPaid, te.ShnStay.FacilityName);
                    }
                }
            }
        }

        static void DisplayResidents(List<Resident> rList)
        {
            Console.WriteLine("\n<----------Residents List----------->");

            Console.WriteLine("{0, -15} {1, -20} {2, -20}", "Name", "Address", "Last Left Country");
            foreach (Resident r in rList)
            {
                Console.WriteLine("{0, -15} {1, -20} {2, -20}", r.Name, r.Address, r.LastLeftCountry.ToString("dd MMMM, yyyy"));
            }

            Console.WriteLine("\n<Residents with TravelEntries>");
            foreach (Resident r in rList)
            {
                if (r.TravelEntryList.Count > 0)
                {
                    Console.WriteLine("\nTravelEntry for {0}", r.Name);
                    Console.WriteLine("{0,-17} {1, -10} {2, -22} {3, -22} {4, -14} {5, -15}", "TE Last Country", "TE Mode", "TravelEntry Date", "TravelSHN EndDate", "Travells Paid", "Facility Name");
                    foreach (TravelEntry te in r.TravelEntryList)
                    {
                        if (te.ShnStay == null)
                            Console.WriteLine("{0,-17} {1, -10} {2, -22} {3, -22} {4, -14} {5, -15}", te.LastCoutryOfEmbarkation, te.EntryMode, te.EntryDate, te.ShnEndDate, te.IsPaid, "Nil");

                        else
                            Console.WriteLine("{0,-17} {1, -10} {2, -22} {3, -22} {4, -14} {5, -15}", te.LastCoutryOfEmbarkation, te.EntryMode, te.EntryDate, te.ShnEndDate, te.IsPaid, te.ShnStay.FacilityName);
                        //USE TRY CATCH FOR THIS

                    }
                }
            }

        }

        static void SearchPerson(List<Person> pList)
        {
            Console.Write("\nEnter person name: ");
            string name = Console.ReadLine();
            //Get the PersonIndex from the method
            int personIndex = FindPerson(name, pList);
            if (personIndex == -1)
                Console.WriteLine("Invalid input or the Person name is not found!");
            else
            {
                Person p = pList[personIndex];
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

            Console.WriteLine("\n<-----SafeEntry Records for everyone----->");
            foreach (Person p in pList)
            {
                if (p.SafeEntryList.Count > 0)
                {
                    isEmpty = false;
                    
                    Console.WriteLine("For person: " + p.Name + ": ");
                    Console.WriteLine("{0, -25} {1, -25} {2, -20}", "Check-In", "Check-Out", "Business Location");
                    foreach(SafeEntry se in p.SafeEntryList)
                    {
                        if (se.CheckOut == new DateTime())
                            Console.WriteLine("{0, -25} {1, -25} {2, -20}", se.CheckIn, "N/A", se.Location.BusinessName);
                        
                        else
                            Console.WriteLine("{0, -25} {1, -25} {2, -20}", se.CheckIn, se.CheckOut, se.Location.BusinessName);
                        
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
