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
        const double USDToOMR = 0.3845; // Exchange rate from USD to OMR
        const double EURToOMR = 0.4167; // Exchange rate from EUR to OMR
        const double MinBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string RequestsFilePath = "requests.txt";
        const string ReviewsFilePath = "reviews.txt";
        const string MonthlyReportFilePath = " Statement_Acc12345_2025-07.txt";
        const string FeedbackFilePath = "feedback.txt";
        const string backupFilePath = " Backup_2025-07-01_0930.txt"; // Path for backup file
        const string TransactionFilePath = "transaction_log.txt";
        const string AppointmentFilePath = "appointments.txt";


        static List<int> AcountNum = new List<int>();
        static List<string> accountNames = new List<string>();
        static List<double> balances = new List<double>() { 500.0, 300.0 };

        static Queue<string> RequestAccountCreate = new Queue<string>();
        //  static Queue<string> RequestAccountCreate = new Queue<string>();
        static Stack<string> reviewsStack = new Stack<string>();
        const string AdminID = "admin1";
        const string AdminPassword = "Rehab23";

        //List<string> approvedNationalIDs = new List<string>();
        static List<string> NationalID = new List<string>(); //store national Id in List 
        static List<string> phoneNumbers = new List<string>(); //store phone numbers in List
        static List<string> addresses = new List<string>(); //store addresses in List

        static List<bool> hasActiveLoan = new List<bool>();
        static Queue<string> loanRequests = new Queue<string>();
        static List<int> feedbackRatings = new List<int>();
        static Queue<string> appointments = new Queue<string>();
        static List<int> usersWithAppointments = new List<int>(); // To track user indexes
        static List<int> failedLoginAttempts = new List<int>();
        static List<bool> isAccountLocked = new List<bool>();



        static int LastAccountNumber;

        static void Main()
        {
           // LogTransaction();
            LoadAccountInformationFromFile();
            LoadReviews();
            LoadFeedbackFromFile();

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
                    case "1":
                        int userIdxt = UserLogin();
                        if (userIdxt != -1) // Check if the user login was successful
                            UserMenu(userIdxt); // Call UserMenu method to display user options

                        break;
                    case "2":
                        if (AdminLogin())
                            AdminMenu();
                        break;

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
                Console.WriteLine("8. View Last N Transactions");
                Console.WriteLine("9. View Transactions After Date");
                Console.WriteLine("10. Book Appointment"); 

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
                    case "8": ViewLastNTransactions(index); break;
                    case "9": ViewTransactionsAfterDate(index); break;
                    case "10": BookAppointment(index); break; // Call BookAppointment method to book an appointment



                    case "0":

                        userMenuRunning = false; break;
                    default:
                        Console.Write("Do you want to create a backup before exiting? (y/n): ");
                        if (Console.ReadLine().Trim().ToLower() == "y")
                            BackupAllData();
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
                Console.WriteLine("8. View Feedback Ratings");
                Console.WriteLine("9. Backup All Data"); // Add backup option
                Console.WriteLine("10. Print Transactions of a Use");
                Console.WriteLine("11. View Appointments"); // Add option to view appointments
                Console.WriteLine("12. Unlock Locked Accounts");
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
                    case "8": ViewFeedback(); break;  // Add to AdminMenu()
                    case "9": BackupAllData(); break; // Add backup option
                    case "10": PrintTransactionsOfUser(); break;
                    case "11": ViewAppointments(); break; // Add option to view appointments
                    case "12": UnlockAccounts(); break; // Add option to unlock accounts


                    case "0": adminMenuRunning = false; break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }


        static bool AdminLogin()
        {
            int attempts = 0;
            while (attempts < 3)
            {
                Console.Write("Enter Admin ID: ");
                string enteredID = Console.ReadLine().Trim();

                Console.Write("Enter Admin Password: ");
                string enteredPassword = Console.ReadLine().Trim();

                if (enteredID == AdminID && enteredPassword == AdminPassword)
                {
                    Console.WriteLine("Admin login successful.");
                    Console.ReadLine();
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid Admin ID or Password.\n");
                    attempts++;
                }
            }

            Console.WriteLine("Too many failed attempts. Returning to main menu...");
            Console.ReadLine();
            return false;
        }


        static int UserLogin()
        {
            Console.WriteLine("Enter your National ID (10 digits):");
            string inputID = Console.ReadLine().Trim();

            int index = NationalID.IndexOf(inputID);
            if (index == -1)
            {
                Console.WriteLine("National ID not found!");
                Console.ReadLine();
                return -1;
            }

            if (isAccountLocked[index])
            {
                Console.WriteLine("Your account is locked. Please contact admin.");
                Console.ReadLine();
                return -1;
            }

            Console.Write("Enter password (your National ID): ");
            string enteredPassword = Console.ReadLine().Trim();

            if (enteredPassword == NationalID[index])
            {
                failedLoginAttempts[index] = 0; // reset counter
                Console.WriteLine($"Welcome, {accountNames[index]}!");
                Console.ReadLine();
                return index;
            }
            else
            {
                failedLoginAttempts[index]++;
                if (failedLoginAttempts[index] >= 3)
                {
                    isAccountLocked[index] = true;
                    Console.WriteLine("Account locked due to 3 failed attempts.");
                }
                else
                {
                    Console.WriteLine($"Wrong password. Attempts left: {3 - failedLoginAttempts[index]}");
                }
                Console.ReadLine();
                return -1;
            }
        }


        //static bool AdminLogin()
        //{
        //    Console.WriteLine("Enter admin password:");
        //    string password = Console.ReadLine();
        //    if (password == AdminPassword)
        //    {
        //        Console.WriteLine("Welcome, Admin!");
        //        return true;
        //    }
        //    else
        //    {
        //        Console.WriteLine("Incorrect password.");
        //        return false;
        //    }
        //}


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
            failedLoginAttempts.Insert(newIndex, 0);
            isAccountLocked.Insert(newIndex, false);

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
         //   LogTransaction($"Deposit ({currencyType})", originalAmount, convertedAmount, index);
            CollectFeedback();
        }

        static void Deposit()
        {
            int index = GetAccountIndex();
            if (index == -1) return;

            Console.WriteLine("Select currency:");
            Console.WriteLine("1. OMR (Local)");
            Console.WriteLine("2. USD");
            Console.WriteLine("3. EUR");
            Console.Write("Your choice: ");
            string currencyChoice = Console.ReadLine();

            Console.Write("Enter deposit amount: ");
            if (!double.TryParse(Console.ReadLine(), out double originalAmount) || originalAmount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                Console.ReadLine();
                return;
            }

            double convertedAmount = 0;
            string currencyType = "";

            switch (currencyChoice)
            {
                case "1":
                    convertedAmount = originalAmount;
                    currencyType = "OMR";
                    break;
                case "2":
                    convertedAmount = originalAmount * USDToOMR;
                    currencyType = "USD";
                    break;
                case "3":
                    convertedAmount = originalAmount * EURToOMR;
                    currencyType = "EUR";
                    break;
                default:
                    Console.WriteLine("Invalid currency selection.");
                    Console.ReadLine();
                    return;
            }

            balances[index] += convertedAmount;

            Console.WriteLine($"Deposit successful. {originalAmount} {currencyType} = {convertedAmount} OMR");
            Console.WriteLine($"New Balance: {balances[index]} OMR");
            Console.ReadLine();

            // Log the full transaction with currency
        LogTransaction($"Deposit ({currencyType})", originalAmount, convertedAmount, index);
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
                        writer.WriteLine($"{AcountNum[i]}|{accountNames[i]}|{balances[i]}|{NationalID[i]}|{phoneNumbers[i]}|{addresses[i]}|{hasActiveLoan[i]}|{failedLoginAttempts[i]}|{isAccountLocked[i]}");

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
            if (!File.Exists(AccountsFilePath)) return;

            try
            {
                AcountNum.Clear();
                accountNames.Clear();
                balances.Clear();
                NationalID.Clear();
                phoneNumbers.Clear();
                addresses.Clear();
                hasActiveLoan.Clear();
                failedLoginAttempts.Clear();
                isAccountLocked.Clear();

                foreach (var line in File.ReadAllLines(AccountsFilePath))
                {
                    var parts = line.Split('|');
                    if (parts.Length == 9)
                    {
                        AcountNum.Add(int.Parse(parts[0]));
                        accountNames.Add(parts[1]);
                        balances.Add(double.Parse(parts[2]));
                        NationalID.Add(parts[3].Trim());
                        phoneNumbers.Add(parts[4]);
                        addresses.Add(parts[5]);
                        hasActiveLoan.Add(bool.Parse(parts[6]));
                        failedLoginAttempts.Add(int.Parse(parts[7]));
                        isAccountLocked.Add(bool.Parse(parts[8]));

                        if (AcountNum[^1] > LastAccountNumber)
                            LastAccountNumber = AcountNum[^1];
                    }
                    else
                    {
                        Console.WriteLine("Skipped invalid line in accounts file: " + line);
                    }
                }

    
                for (int i = 0; i < AcountNum.Count; i++)
                {
                    Console.WriteLine($"[{i}] Account: {AcountNum[i]}, Name: {accountNames[i]}, NID: {NationalID[i]}, Locked: {isAccountLocked[i]}, Attempts: {failedLoginAttempts[i]}");
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



        static void LogTransaction(string transactionType, double originalAmount, double convertedAmount, int accountIndex)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int accountNumber = AcountNum[accountIndex];

            string logEntry = $"{timestamp} | {transactionType} | Original: {originalAmount} | Converted: {convertedAmount} OMR | Account: {accountNumber} | New Balance: {balances[accountIndex]}";

            // Log to the central file (optional)
            File.AppendAllText("transaction_log.txt", logEntry + Environment.NewLine);

            // ✅ Log to user-specific file
            string userLogFile = $"transactions_{accountNumber}.txt";
            File.AppendAllText(userLogFile, logEntry + Environment.NewLine);

            Console.WriteLine(logEntry);
        }



        static void MonthlyReport()
        {
            Console.WriteLine("Enter your account number:");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Invalid account number.");
                Console.ReadLine();
                return;
            }

            int index = AcountNum.IndexOf(accountNumber);
            if (index == -1)
            {
                Console.WriteLine("Account not found.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter month (1–12): ");
            if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
            {
                Console.WriteLine("Invalid month.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter year (e.g., 2025): ");
            if (!int.TryParse(Console.ReadLine(), out int year) || year < 2000 || year > DateTime.Now.Year + 5)
            {
                Console.WriteLine("Invalid year.");
                Console.ReadLine();
                return;
            }

            string filePath = $"transactions_{accountNumber}.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No transaction history found.");
                Console.ReadLine();
                return;
            }

            var lines = File.ReadAllLines(filePath);
            List<string> filteredTransactions = new List<string>();

            foreach (var line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 1 && DateTime.TryParse(parts[0], out DateTime date))
                {
                    if (date.Month == month && date.Year == year)
                        filteredTransactions.Add(line);
                }
            }

            if (filteredTransactions.Count == 0)
            {
                Console.WriteLine($"No transactions found for {year}-{month:D2}.");
                Console.ReadLine();
                return;
            }

            string statementFile = $"Statement_Acc{accountNumber}_{year}-{month:D2}.txt";
            using (StreamWriter writer = new StreamWriter(statementFile))
            {
                writer.WriteLine($"Monthly Statement for Account #{accountNumber}");
                writer.WriteLine($"Account Holder: {accountNames[index]}");
                writer.WriteLine($"Month: {year}-{month:D2}");
                writer.WriteLine("=====================================");
                foreach (var entry in filteredTransactions)
                    writer.WriteLine(entry);
                writer.WriteLine("=====================================");
                writer.WriteLine($"Balance: {balances[index]:F2} OMR");
            }

            Console.WriteLine($"Statement saved to file: {statementFile}");
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


        static void ViewLastNTransactions(int accountIndex)
        {
            string filePath = $"transactions_{AcountNum[accountIndex]}.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No transaction history found.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter number of recent transactions to view: ");
            if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
            {
                Console.WriteLine("Invalid number.");
                Console.ReadLine();
                return;
            }

            var allLines = File.ReadAllLines(filePath);
            var lastN = allLines.Reverse().Take(n);

            Console.WriteLine($"\nLast {n} transactions:");
            foreach (var line in lastN.Reverse())
                Console.WriteLine(line);

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void ViewTransactionsAfterDate(int accountIndex)
        {
            string filePath = $"transactions_{AcountNum[accountIndex]}.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No transaction history found.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateFilter))
            {
                Console.WriteLine("Invalid date format.");
                Console.ReadLine();
                return;
            }

            var lines = File.ReadAllLines(filePath);
            Console.WriteLine($"\nTransactions after {dateFilter:yyyy-MM-dd}:");

            foreach (var line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 3 && DateTime.TryParse(parts[0], out DateTime timestamp))
                {
                    if (timestamp > dateFilter)
                        Console.WriteLine(line);
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void CollectFeedback()
        {
            Console.Write("Please rate our service (1–5): ");
            if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 5)
            {
                feedbackRatings.Add(rating);
                try
                {
                    File.AppendAllText(FeedbackFilePath, rating + Environment.NewLine);
                    Console.WriteLine("Thank you for your feedback!");
                }
                catch
                {
                    Console.WriteLine("Error saving feedback.");
                }
            }
            else
            {
                Console.WriteLine("Invalid rating. Feedback not recorded.");
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        static void LoadFeedbackFromFile()
        {
            if (!File.Exists(FeedbackFilePath)) return;

            feedbackRatings.Clear();
            try
            {
                foreach (var line in File.ReadAllLines(FeedbackFilePath))
                {
                    if (int.TryParse(line.Trim(), out int rating) && rating >= 1 && rating <= 5)
                    {
                        feedbackRatings.Add(rating);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error loading feedback ratings.");
            }
        }

        static void ViewFeedback()
        {
            Console.WriteLine(" this is a feedback Ratings");
            if (feedbackRatings.Count == 0)

            {
                Console.WriteLine("No feedback ratings available.");
                return;
            }
            else
            {
                double averageRating = feedbackRatings.Average(); // Calculate the average rating
                Console.WriteLine($"Total Ratings: {feedbackRatings.Count}");
                Console.WriteLine($"Average Feedback Score: {averageRating:F2} / 5");

                Console.ReadLine();

            }

        }

        static void BackupAllData()
        {


            try
            {
                using (StreamWriter writer = new StreamWriter(backupFilePath))
                {
                    writer.WriteLine("==== ACCOUNT DATA ====");
                    for (int i = 0; i < AcountNum.Count; i++)
                    {
                        writer.WriteLine($"{AcountNum[i]}|{accountNames[i]}|{balances[i]}|{NationalID[i]}|{phoneNumbers[i]}|{addresses[i]}|{hasActiveLoan[i]}");
                    }

                    writer.WriteLine("\n==== REVIEWS ====");
                    foreach (var review in reviewsStack)
                        writer.WriteLine(review);

                    writer.WriteLine("\n==== FEEDBACK RATINGS ====");
                    foreach (var rating in feedbackRatings)
                        writer.WriteLine(rating);

                    writer.WriteLine("\n==== TRANSACTIONS ====");
                    if (File.Exists("transaction_log.txt"))
                        writer.Write(File.ReadAllText("transaction_log.txt"));

                    writer.WriteLine("\n==== PENDING ACCOUNT REQUESTS ====");
                    foreach (var req in RequestAccountCreate)
                        writer.WriteLine(req);

                    writer.WriteLine("\n==== LOAN REQUESTS ====");
                    foreach (var loan in loanRequests)
                        writer.WriteLine(loan);
                }
                Console.WriteLine("Backup saved to file: " + backupFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Backup failed: " + ex.Message);
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void PrintTransactionsOfUser()
        {
            Console.Write("Enter Account Number: ");
            if (!int.TryParse(Console.ReadLine(), out int accNum))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            if (!File.Exists(TransactionFilePath))
            {
                Console.WriteLine("Transaction log file not found.");
                return;
            }

            var lines = File.ReadAllLines(TransactionFilePath);
            bool found = false;

            Console.WriteLine($"\nTransactions for Account #{accNum}:");
            Console.WriteLine("Date & Time\t\tType\tAmount\tBalance");

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 5 && parts[1] == accNum.ToString())
                {
                    Console.WriteLine($"{parts[0],-20} {parts[2],-8} {parts[3],-6} {parts[4]}");
                    found = true;
                }
            }

            if (!found)
                Console.WriteLine("No transactions found for this account.");

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }


        static void BookAppointment(int index)
        {
            if (usersWithAppointments.Contains(index))
            {
                Console.WriteLine("You already have an appointment booked.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Choose appointment type:");
            Console.WriteLine("1. Loan Discussion");
            Console.WriteLine("2. Account Consultation");
            string choice = Console.ReadLine();

            string service = choice switch
            {
                "1" => "Loan Discussion",
                "2" => "Account Consultation",
                _ => "General Inquiry"
            };

            Console.Write("Enter preferred appointment date and time (e.g., 2025-07-04 14:00): ");
            string dateTimeInput = Console.ReadLine();

            if (!DateTime.TryParse(dateTimeInput, out DateTime appointmentTime))
            {
                Console.WriteLine("Invalid date and time format.");
                Console.ReadLine();
                return;
            }

            string appointment = $"{accountNames[index]}|{AcountNum[index]}|{service}|{appointmentTime}";
            appointments.Enqueue(appointment);
            usersWithAppointments.Add(index);

            Console.WriteLine("Appointment booked successfully.");
            Console.ReadLine();
        }

        static void ViewAppointments()
        {
            Console.WriteLine("\nUpcoming Appointments:");
            if (appointments.Count == 0)
            {
                Console.WriteLine("No upcoming appointments.");
            }
            else
            {
                foreach (var appointment in appointments)
                {
                    var parts = appointment.Split('|');
                    Console.WriteLine($"User: {parts[0]} (#{parts[1]}), Service: {parts[2]}, Date & Time: {parts[3]}");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void SaveAppointmentsToFile()
        {
            File.WriteAllLines(AppointmentFilePath, appointments);
        }

        static void LoadAppointmentsFromFile()
        {
            if (!File.Exists(AppointmentFilePath)) return;
            foreach (var line in File.ReadAllLines(AppointmentFilePath))
                appointments.Enqueue(line);
        }

        static void UnlockAccounts()
        {
            Console.Clear();
            Console.WriteLine("Locked Accounts:");

            bool found = false;
            for (int i = 0; i < AcountNum.Count; i++)
            {
                if (isAccountLocked[i])
                {
                    found = true;
                    Console.WriteLine($"{i + 1}. {accountNames[i]} - Account Number: {AcountNum[i]}");
                }
            }

            if (!found)
            {
                Console.WriteLine("No locked accounts.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter account number to unlock: ");
            if (int.TryParse(Console.ReadLine(), out int accNum))
            {
                int index = AcountNum.IndexOf(accNum);
                if (index != -1 && isAccountLocked[index])
                {
                    isAccountLocked[index] = false;
                    failedLoginAttempts[index] = 0;
                    Console.WriteLine("Account unlocked.");
                }
                else
                {
                    Console.WriteLine("Invalid or not locked account.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
            Console.ReadLine();
        }


    }
}
