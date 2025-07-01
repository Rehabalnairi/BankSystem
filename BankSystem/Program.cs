using System;
using System.Collections.Generic;
using System.IO;
namespace MiniBankSystem
{
    internal class Program
    {
        const double MinBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string RequestsFilePath = "requests.txt";
        const string ReviewsFilePath = "reviews.txt";

        static List<int> AcountNum = new List<int>();
        static List<string> accountNames = new List<string>() ;
        static List<double> balances = new List<double>() { 500.0, 300.0 };

        static Queue<string> RequestAccountCreate = new Queue<string>();
        static Stack<string> reviewsStack = new Stack<string>();
        const string AdminPassword = "Rehab23";
    
        //List<string> approvedNationalIDs = new List<string>();
        static List <string> NationalID = new List<string>(); //store national Id in List 

        static int LastAccountNumber;

        static void Main()
        {
            LoadAccountInformationFromFile();
            LoadReviews();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("Bank System");
                Console.WriteLine("1. User Menu");
                Console.WriteLine("2. Admin Menu");
                Console.WriteLine("3. Create Account"); // Option to create a new account
                Console.WriteLine("0. Exit");
                Console.Write("Select option: ");
                string mainChoice = Console.ReadLine();
                switch (mainChoice)
                {
                    case "1":int userIdxt = UserLogin();
                          if(userIdxt != -1) // Check if the user login was successful
                            UserMenu(); // Call UserMenu method to display user options

                        break;
                    case "2": AdminMenu(); break;
                    case "3": RequestAccountCreation(); break; // Call CreateAccount method to handle account creation
                    case "0": running = false; break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        static void UserMenu()
        {
            bool userMenuRunning = true;
            while (userMenuRunning)
            {
                Console.Clear();
            

                Console.WriteLine("========== User Menu ==========");
                Console.WriteLine("1. View Account Balance");
                Console.WriteLine("2. Deposit Money");
                Console.WriteLine("3. Withdraw Money");
                Console.WriteLine("4. Submit a Review");
               // Console.WriteLine("5. Request Account Creation"); 
                Console.WriteLine("0. login Menu");
                Console.WriteLine("Select an option: ");

                string userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1": ViewBalance(); break;
                    case "2": Deposit(); break;
                    case "3": Withdraw(); break;
                    case "4": SubmitReview(); break;
                  //  case "5": RequestAccountCreation(); break; 
                    case "0": userMenuRunning = false; break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

         static void CreateAccount()
        {

        }
        static void AdminMenu()
        {
            bool adminMenuRunning = true;
            while (adminMenuRunning)
            {
                Console.Clear();
                Console.WriteLine("========== Admin Menu ==========");
                Console.WriteLine("1. View All Accounts");
                Console.WriteLine("2. Process Account Creation Requests");
                Console.WriteLine("3. View Pending Requests");
                Console.WriteLine("4. View Reviews");
                Console.WriteLine("5. Save Account Information");
                Console.WriteLine("6. Save Reviews");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");
                string adminChoice = Console.ReadLine();

                switch (adminChoice)
                {
                    case "1": ViewAllAccounts(); break;
                    case "2": ProcessNextRequest(); break;
                    case "3": ViewPendingRequests(); break;
                    case "4": ViewReviews(); break;
                    case "5": SaveAccountInformationInFile(); break;
                    case "6": SaveReviews(); break;
                    case "0": adminMenuRunning = false; break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }


        static int UserLogin()

        {
            Console.WriteLine("Enter your National ID (10 digits:");
       
            try
            {
               // int accNum = Convert.ToInt32(Console.ReadLine());
                string inputID = Console.ReadLine().Trim(); // Read the National ID input and trim any extra spaces

                int index = NationalID.IndexOf(inputID);
                if (index != -1)
                {
                    Console.WriteLine($"Welcome,{accountNames[index]}!");
                    Console.ReadLine();
                    return index;
                }
                else
                {
                    Console.WriteLine("National ID not found!");
                    Console.ReadLine();
                    return -1;




                }
            }
            catch
            {
                Console.WriteLine("Invaild Input");
                Console.ReadLine();
                return -1;
            }
        }
        static bool AdminLogin()
        {
            Console.WriteLine("Enter admin password:");
            string password = Console.ReadLine();
            if (password == AdminPassword)
            {
                Console.WriteLine("Welcome, Admin!");
                return true;
            }
            else
            {
                Console.WriteLine("Incorrect password.");
                return false;
            }
        }


        //    if (!File.Exists(AccountsFilePath))
        //    {
        //        Console.WriteLine("Accounts file not found.");
        //        return;
        //    }

        //    var allAccounts = File.ReadAllLines(AccountsFilePath);
        //    List<string> nationalIDs = new List<string>();


        //    foreach (var account in allAccounts)
        //    {
        //        var parts = account.Split('|');
        //        if (parts.Length >= 2)
        //        {
        //            nationalIDs.Add(parts[1].Trim()); 
        //        }
        //    }

        static void RequestAccountCreation()
        {
            string name, nid;
            do
            {
                Console.Write("Enter Full Name: ");
                name = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(name)) Console.WriteLine("Name cannot be empty.");
            } while (string.IsNullOrWhiteSpace(name));

            do
            {
                Console.Write("Enter 10-digit National ID: ");
                nid = Console.ReadLine().Trim();
                if (nid.Length != 10 || !nid.All(char.IsDigit))
                    Console.WriteLine("Invalid National ID.");
            } while (nid.Length != 10 || !nid.All(char.IsDigit));

            RequestAccountCreate.Enqueue($"{name}|{nid}");
            Console.WriteLine("Request submitted.");
            Console.ReadLine();
        }

        static void ProcessNextRequest()
        {
            if (RequestAccountCreate.Count == 0)
            {
                Console.WriteLine("No pending requests.");
                return;
            }

            string request = RequestAccountCreate.Dequeue();
            string[] parts = request.Split('|');

            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid request format.");
                return;
            }

            string name = parts[0].Trim();
            string nationalID = parts[1].Trim();// trim any extra spaces
         

            int newAccountNumber = ++LastAccountNumber;
            // Add new account details
            AcountNum.Add(newAccountNumber);
         
            accountNames.Add(name);
       


            balances.Add(MinBalance);

            Console.WriteLine("\nAccount created successfully:");
            Console.WriteLine($"Account Number: {newAccountNumber}");
            Console.WriteLine($"Account Holder: {name}");
          
          
            Console.WriteLine($"Initial Balance: {MinBalance} OMR");
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void ViewPendingRequests()
        {
            Console.WriteLine("\nPending Account Requests:");

            if (RequestAccountCreate.Count == 0)
            {
                Console.WriteLine("No pending requests.");
                return;
            }

            int count = 1;
            foreach (var request in RequestAccountCreate)
            {
                string[] parts = request.Split('|'); // Split the request into name and national ID
                if (parts.Length == 2)
                {
                    string name = parts[0].Trim(); // Trim any extra spaces
                    string nationalID = parts[1].Trim();

                    Console.WriteLine($"{count}. Name: {name}, National ID: {nationalID}");
                    count++;
                }
                else
                {
                    Console.WriteLine("Invalid request format detected.");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static int GetAccountIndex()
        {
            Console.WriteLine("Enter account number:");
            if (int.TryParse(Console.ReadLine(), out int accountNumber))// Try to parse the input into an integer
            {
                // Search for the account number in the AcountNum list and get the index
                int index = AcountNum.IndexOf(accountNumber);
                if (index == -1)
                // If the account is not found, display an error message
                {
                    Console.WriteLine("Account not found.");
                }
                return index;
            }
            else
            {
                Console.WriteLine("Invalid input.");
                return -1;
            }
        }

        static void ViewAllAccounts()
        {
            Console.WriteLine("All Accounts:");
            if (AcountNum.Count == 0)
            {
                Console.WriteLine("No accounts available.");
                return;
            }
            else  
            for (int i = 0; i < AcountNum.Count; i++) // for loop to iterate through all accounts
                
            {
                Console.WriteLine($"Account Number: {AcountNum[i]}, Account Holder: {accountNames[i]}, Balance: {balances[i]} OMR");
            }
                Console.WriteLine("press Enter to Continue");
                Console.ReadLine();
            }

        static void Withdraw()
        {
            int index = GetAccountIndex();// Request the account index by calling GetAccountIndex
            if (index == -1) return; // If no valid account was found, exit the method

            Console.WriteLine("Enter withdrawal amount:");
            if (double.TryParse(Console.ReadLine(), out double amount) && amount > 0)  // Try to parse the user's input as a double and check if the amount is greater than 0
            {
                if (balances[index] - amount >= MinBalance) // Check if the withdrawal amount will not leave the account below the minimum balance
                {
                    balances[index] -= amount;   // If the withdrawal is allowed, subtract the amount from the balance
                    Console.WriteLine("Withdrawal successful.");
                }
                else
                {
                    Console.WriteLine("Insufficient balance after minimum balance restriction.");
                }
            }
            else
            {
                Console.WriteLine("Invalid amount.");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void Deposit()
        {
            int index = GetAccountIndex(); //request acount number by call GetAccountIndex

            if (index == -1) return;// If the account index is -1 (indicating no valid account was found), exit the method

            Console.WriteLine("Enter deposit amount:");
            if (double.TryParse(Console.ReadLine(), out double amount))
            {
                if (amount < 1)// Try to parse the user's input into a double for the deposit amount
                {
                    Console.WriteLine("Minimum deposit amount is 1 OMR!");
                    Console.WriteLine("\nPress Enter to continue...");
                    Console.ReadLine();
                    return;
                }
                // Add the deposit amount to the account's balance
                balances[index] += amount;
                Console.WriteLine($"Deposit successful. New Balance: {balances[index]} OMR");
            }
            else
            {
                Console.WriteLine("Invalid amount.");
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }


        static void ViewBalance()
        {
            int index = GetAccountIndex(); //request acount number by call GetAccountIndex
            if (index == -1) return;

            Console.WriteLine($"Account Number: {AcountNum[index]}");
            Console.WriteLine($"Account Holder: {accountNames[index]}");
            Console.WriteLine($"Balance: {balances[index]} OMR");
            Console.ReadLine();

        }

        static void SaveAccountInformationInFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(AccountsFilePath)) // open file to write
                {
                  
                        for (int i = 0; i < AcountNum.Count; i++)
                    {
                        writer.WriteLine($"{AcountNum[i]}|{accountNames[i]}|{balances[i]} |{NationalID[i]}"); // update to save Natinalid as password 
                    }
                }
                Console.WriteLine("Account information saved successfully.");
            }
            catch
            {
                Console.WriteLine("Error saving account information."); // If an error occurs while saving, inform the user
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void LoadAccountInformationFromFile()
        {
            if (!File.Exists(AccountsFilePath)) return;  // Check if the accounts file exists at the specified path

            try
            {
                // Clear the existing data collections to ensure fresh loading
                AcountNum.Clear();
                accountNames.Clear();
                balances.Clear();
                NationalID.Clear();

                foreach (var line in File.ReadAllLines(AccountsFilePath))// Read all lines from the accounts file
                {
                    var parts = line.Split('|');   // Split each line into parts based on the delimiter '|'
                    if (parts.Length == 4)  // Ensure the line has exactly 3 parts (account number, account name, balance)
                    {
                        int index = AcountNum.Count;
                        AcountNum.Insert(index,int.Parse(parts[0]));
                        accountNames.Insert(index,parts[1]);
                        balances.Insert(index,double.Parse(parts[2]));
                        NationalID.Insert(index,(parts[3]));//save 


                        if (AcountNum[^1] > LastAccountNumber)   // Update the LastAccountNumber to be the highest account number found
                            LastAccountNumber = AcountNum[^1];
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error loading account information.");
            }
        }

        static void SubmitReview()
        {
            Console.WriteLine("Enter your review:");
            string review = Console.ReadLine();
            // Check if the review is not null, empty, or just whitespace
            if (!string.IsNullOrWhiteSpace(review)) 
            {
                reviewsStack.Push(review); // Push the valid review onto the stack
                Console.WriteLine("Review submitted successfully.");
            }
            else
            {
                Console.WriteLine("Review cannot be empty.");
            }
        }

        static void ViewReviews()
        {
            Console.WriteLine("All Reviews:");
            if (reviewsStack.Count == 0)
            {
                Console.WriteLine("No reviews.");
                return;
            }

            // Initialize a counter to display the review number
            int i = 1; 
            foreach (var review in reviewsStack)
            {
                // Display each review with a number prefix
                Console.WriteLine($"{i++}. {review}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void SaveReviews()
        {
            try
            {
                // Open the file at ReviewsFilePath in write mode using a StreamWriter
                using (StreamWriter writer = new StreamWriter(ReviewsFilePath))
                {
                    // Iterate through the reviews stack and write each review to the file
                    foreach (var review in reviewsStack)
                    {
                        writer.WriteLine(review);
                    }
                }
                Console.WriteLine("Reviews saved successfully.");
            }
            catch
            {
                Console.WriteLine("Error saving reviews.");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void LoadReviews()
        {
            // Check if the reviews file exists at the specified path
            if (!File.Exists(ReviewsFilePath)) return;

            try
            {
                foreach (var line in File.ReadAllLines(ReviewsFilePath)) // Read all lines from the reviews file
                {
                    reviewsStack.Push(line);// Push each line (review) from the file onto the stack
                }
            }
            catch
            {
                Console.WriteLine("Error loading reviews.");
            }
        }
    }
}
