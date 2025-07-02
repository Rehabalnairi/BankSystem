using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;
namespace MiniBankSystem
{
    internal class Program
    {
        const double MinBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string RequestsFilePath = "requests.txt"; 
        const string ReviewsFilePath = "reviews.txt";
        const string MonthlyReportFilePath = " Statement_Acc12345_2025-07.txt";

        static List<int> AcountNum = new List<int>();
        static List<string> accountNames = new List<string>();
        static List<double> balances = new List<double>() { 500.0, 300.0 };

        static Queue<string> RequestAccountCreate = new Queue<string>();
        //  static Queue<string> RequestAccountCreate = new Queue<string>();
        static Stack<string> reviewsStack = new Stack<string>();
        const string AdminPassword = "Rehab23";

        //List<string> approvedNationalIDs = new List<string>();
        static List<string> NationalID = new List<string>(); //store national Id in List 
        static List<string> phoneNumbers = new List<string>(); //store phone numbers in List
        static List<string> addresses = new List<string>(); //store addresses in List

        static List<bool> hasActiveLoan = new List<bool>();
        static Queue<string> loanRequests = new Queue<string>();

        static int LastAccountNumber;

        static void Main()
        {

            LoadAccountInformationFromFile();
            LoadReviews();

            static void LoadPendingRequestsFromFile()
            {
                if (!File.Exists(RequestsFilePath)) return;

                try
                {
                    RequestAccountCreate.Clear();
                    foreach (var line in File.ReadAllLines(RequestsFilePath))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            RequestAccountCreate.Enqueue(line);
                    }
                }
                catch
                {
                    Console.WriteLine("Error loading pending requests.");
                }
            }


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
                    case "1": int userIdxt = UserLogin();
                        if (userIdxt != -1) // Check if the user login was successful
                            UserMenu(userIdxt); // Call UserMenu method to display user options

                        break;
                    case "2": AdminMenu(); break;
                    case "3": RequestAccountCreati(); break; // Call CreateAccount method to handle account creation
                    case "0":
                        running = false; break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        static void UserMenu(int index)
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
                Console.WriteLine("5. show Monthliy report"); // Option to request account creation

                Console.WriteLine("6. update information"); // Option to view reviews
                Console.WriteLine("7. Request Loan");

                Console.WriteLine("0. login Menu");
                Console.WriteLine("Select an option: ");

                string userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1": ViewBalance(); break;
                    case "2": Deposit(); break;
                    case "3": Withdraw(); break;
                    case "4": SubmitReview(); break;
                    case "5": MonthlyReport(); break;
                    case "6": UpdatePersonalInformation(index); break;
                    case "7": RequestLoan(index); break;


                    case "0":

                        userMenuRunning = false; break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
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
                Console.WriteLine("7. Process Loan Requests");
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
                    case "7": ProcessLoanRequests(); break;

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

        static void RequestAccountCreati()
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
            string nationalID = parts[1].Trim();



            Console.Write("Enter phone number: ");
            string phone = Console.ReadLine().Trim();

            Console.Write("Enter address: ");
            string address = Console.ReadLine().Trim();

            int newIndex = AcountNum.Count;
            int newAccountNumber = ++LastAccountNumber;

            AcountNum.Insert(newIndex, newAccountNumber);
            accountNames.Insert(newIndex, name);
            balances.Insert(newIndex, MinBalance);
            NationalID.Insert(newIndex, nationalID);
            phoneNumbers.Insert(newIndex, phone);
            addresses.Insert(newIndex, address);
            hasActiveLoan.Insert(newIndex, false);

            Console.WriteLine("\nAccount created successfully:");
            Console.WriteLine($"Account Number: {newAccountNumber}");
            Console.WriteLine($"Account Holder: {name}");
            Console.WriteLine($"Initial Balance: {MinBalance} OMR");
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();

            SaveAccountInformationInFile(); // Save after creating account
            Console.WriteLine("Account information saved.");
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
            balances[index] -= amount; // Subtract the withdrawal amount from the account's balance
            LogTransaction("Withdrawal", amount, index); // Log the withdrawal transaction

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

            balances[index] += amount; // Add the deposit amount to the account's balance
            LogTransaction("Deposit", amount, index); // Log the deposit transaction
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
                using (StreamWriter writer = new StreamWriter(AccountsFilePath))
                {
                    for (int i = 0; i < AcountNum.Count; i++)
                    {
                        writer.WriteLine($"{AcountNum[i]}|{accountNames[i]}|{balances[i]}|{NationalID[i]}|{phoneNumbers[i]}|{addresses[i]}|{hasActiveLoan[i]}");

                    }
                }
                Console.WriteLine("Account information saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving account information: " + ex.Message);
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
                phoneNumbers.Clear();
                addresses.Clear();
                hasActiveLoan.Clear();

                if (!File.Exists(AccountsFilePath)) return;


                foreach (var line in File.ReadAllLines(AccountsFilePath))// Read all lines from the accounts file
                {
                    var parts = line.Split('|');   // Split each line into parts based on the delimiter '|'
                    if (parts.Length == 7)  // Ensure the line has exactly 3 parts (account number, account name, balance)
                    {
                        int index = AcountNum.Count;
                        AcountNum.Insert(index, int.Parse(parts[0]));
                        accountNames.Insert(index, parts[1]);
                        balances.Insert(index, double.Parse(parts[2]));
                        NationalID.Insert(index, parts[3]);
                        phoneNumbers.Insert(index, parts[4]);
                        addresses.Insert(index, parts[5]);// save
                        hasActiveLoan.Insert(index, bool.Parse(parts[6]));


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

        static void UpdateuserNUM()
        {

            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine().Trim();

            Console.Write("Enter Address: ");
            string address = Console.ReadLine().Trim();

            int index = AcountNum.Count;
           // phoneNumbers.Insert(newIndex, phone);
            //addresses.Insert(newIndex, address);

        }

        static void LogTransaction(string transactionType, double amount, int accountIndex)
        {
            static void LogTransaction(string transactionType, double amount, int accountIndex)
            {
                string logFileName = $"transactions_{AcountNum[accountIndex]}.txt";
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|{transactionType}|{amount}";

                File.AppendAllText(logFileName, logEntry + Environment.NewLine);
            }

        }
        static void MonthlyReport()
        {
            Console.WriteLine("Enter account number for monthly report:");
            if (int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                int index = AcountNum.IndexOf(accountNumber);
                if (index != -1)
                {
                    string reportContent = $"Monthly Statement for Account Number: {accountNumber}\n" +
                                           $"Account Holder: {accountNames[index]}\n" +
                                           $"Balance: {balances[index]} OMR\n" +
                                           $"Date: {DateTime.Now.ToString("yyyy-MM-dd")}\n";
                    File.WriteAllText(MonthlyReportFilePath, reportContent); // Write the report content to a file
                    Console.WriteLine("Monthly report generated successfully.");
                }
                else
                {
                    Console.WriteLine("Account not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid account number.");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void UpdatePersonalInformation(int index)
        {
            Console.Write("Enter new phone number (or leave blank to keep current): ");
            string newPhone = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(newPhone))
            {
                phoneNumbers[index] = newPhone;
                Console.WriteLine("Phone updated.");
            }

            Console.Write("Enter new address (or leave blank to keep current): ");
            string newAddress = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(newAddress))
            {
                addresses[index] = newAddress;
                Console.WriteLine("Address updated.");
            }

            SaveAccountInformationInFile(); // Save after update
            Console.WriteLine("Changes saved.");
            Console.ReadLine();
        }

    
   static void RequestLoan(int index)
        {
            if (balances[index] < 5000)
            {
                Console.WriteLine("You must have at least 5000 OMR to request a loan.");
                Console.ReadLine();
                return;
            }

            if (hasActiveLoan[index])
            {
                Console.WriteLine("You already have an active loan.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter desired loan amount: ");
            if (!double.TryParse(Console.ReadLine(), out double loanAmount) || loanAmount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter interest rate (e.g., 0.05 for 5%): ");
            if (!double.TryParse(Console.ReadLine(), out double interestRate) || interestRate < 0)
            {
                Console.WriteLine("Invalid interest rate.");
                Console.ReadLine();
                return;
            }

            loanRequests.Enqueue($"{index}|{loanAmount}|{interestRate}");
            Console.WriteLine("Loan request submitted for review.");
            Console.ReadLine();
        }

        static void ProcessLoanRequests()
        {
            while (loanRequests.Count > 0)
            {
                string request = loanRequests.Dequeue();
                string[] parts = request.Split('|');
                int index = int.Parse(parts[0]);
                double amount = double.Parse(parts[1]);
                double rate = double.Parse(parts[2]);

                Console.WriteLine($"\nLoan Request for: {accountNames[index]}");
                Console.WriteLine($"Amount: {amount} OMR, Interest Rate: {rate * 100}%");
                Console.Write("Approve this loan? (y/n): ");
                string choice = Console.ReadLine().ToLower();

                if (choice == "y")
                {
                    balances[index] += amount;
                    hasActiveLoan[index] = true;
                    Console.WriteLine("Loan approved and balance updated.");
                }
                else
                {
                    Console.WriteLine("Loan request rejected.");
                }
            }

            Console.WriteLine("All loan requests processed.");
            Console.ReadLine();
        }
    }


    }
