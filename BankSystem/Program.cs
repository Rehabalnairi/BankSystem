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
        static List<string> accountNames = new List<string>();
        static List<double> balances = new List<double>();

        static Queue<string> RequestAccountCreate = new Queue<string>();
        static Stack<string> reviewsStack = new Stack<string>();

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
                Console.WriteLine("0. Exit");
                Console.Write("Select option: ");
                string mainChoice = Console.ReadLine();
                switch (mainChoice)
                {
                    case "1": UserMenu(); break;
                    case "2": AdminMenu(); break;
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
                Console.WriteLine("5. Request Account Creation"); 
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("Select an option: ");

                string userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1": ViewBalance(); break;
                    case "2": Deposit(); break;
                    case "3": Withdraw(); break;
                    case "4": SubmitReview(); break;
                    case "5": RequestAccountCreation(); break; 
                    case "0": userMenuRunning = false; break;
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

        static void RequestAccountCreation()
        {
            string name;
            string nationalID;

            do
            {
                Console.WriteLine("Enter your full name:");
                name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Full name cannot be empty. Please try again.");
                }

            } while (string.IsNullOrWhiteSpace(name));

            do
            {
                Console.WriteLine("Enter your National ID (10 digits):");
                nationalID = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nationalID) || nationalID.Length != 10 || !nationalID.All(char.IsDigit))
                {
                    Console.WriteLine("National ID must be exactly 10 digits. Please try again.");
                }

            } while (string.IsNullOrWhiteSpace(nationalID) || nationalID.Length != 10 || !nationalID.All(char.IsDigit));

            string request = $"{name}|{nationalID}";
            RequestAccountCreate.Enqueue(request);

            Console.WriteLine(" Account creation request submitted successfully!");
            Console.ReadKey();
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

            string name = parts[0];
            string nationalID = parts[1];

            int newAccountNumber = ++LastAccountNumber;
            AcountNum.Add(newAccountNumber);
            accountNames.Add(name);
            balances.Add(MinBalance);

            Console.WriteLine("Account created successfully:");
            Console.WriteLine($"Account Number: {newAccountNumber}");
            Console.WriteLine($"Account Holder: {name}");
            Console.WriteLine($"Initial Balance: {MinBalance} OMR");
        }

        static void ViewPendingRequests()
        {
            Console.WriteLine("Pending Account Requests:");
            if (RequestAccountCreate.Count == 0)
            {
                Console.WriteLine("No pending requests.");
                return;
            }

            foreach (var request in RequestAccountCreate)
            {
                string[] parts = request.Split('|');
                Console.WriteLine($"Name: {parts[0]}, National ID: {parts[1]}");
            }
        }

        static int GetAccountIndex()
        {
            Console.WriteLine("Enter account number:");
            if (int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                int index = AcountNum.IndexOf(accountNumber);
                if (index == -1)
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
            for (int i = 0; i < AcountNum.Count; i++)
            {
                Console.WriteLine($"Account Number: {AcountNum[i]}, Account Holder: {accountNames[i]}, Balance: {balances[i]} OMR");
            }
        }

        static void Withdraw()
        {
            int index = GetAccountIndex();
            if (index == -1) return;

            Console.WriteLine("Enter withdrawal amount:");
            if (double.TryParse(Console.ReadLine(), out double amount) && amount > 0)
            {
                if (balances[index] - amount >= MinBalance)
                {
                    balances[index] -= amount;
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
        }

        static void Deposit()
        {
            int index = GetAccountIndex();
            if (index == -1) return;

            Console.WriteLine("Enter deposit amount:");
            if (double.TryParse(Console.ReadLine(), out double amount) && amount > 0)
            {
                balances[index] += amount;
                Console.WriteLine("Deposit successful.");
            }
            else
            {
                Console.WriteLine("Invalid amount.");
            }
        }

        static void ViewBalance()
        {
            int index = GetAccountIndex();
            if (index == -1) return;

            Console.WriteLine($"Account Number: {AcountNum[index]}");
            Console.WriteLine($"Account Holder: {accountNames[index]}");
            Console.WriteLine($"Balance: {balances[index]} OMR");
        }

        static void SaveAccountInformationInFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(AccountsFilePath))
                {
                    for (int i = 0; i < AcountNum.Count; i++)
                    {
                        writer.WriteLine($"{AcountNum[i]}|{accountNames[i]}|{balances[i]}");
                    }
                }
                Console.WriteLine("Account information saved successfully.");
            }
            catch
            {
                Console.WriteLine("Error saving account information.");
            }
        }

        static void LoadAccountInformationFromFile()
        {
            if (!File.Exists(AccountsFilePath)) return;

            try
            {
                AcountNum.Clear();
                accountNames.Clear();
                balances.Clear();

                foreach (var line in File.ReadAllLines(AccountsFilePath))
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        AcountNum.Add(int.Parse(parts[0]));
                        accountNames.Add(parts[1]);
                        balances.Add(double.Parse(parts[2]));

                        if (AcountNum[^1] > LastAccountNumber)
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
            if (!string.IsNullOrWhiteSpace(review))
            {
                reviewsStack.Push(review);
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

            int i = 1;
            foreach (var review in reviewsStack)
            {
                Console.WriteLine($"{i++}. {review}");
            }
        }

        static void SaveReviews()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(ReviewsFilePath))
                {
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
        }

        static void LoadReviews()
        {
            if (!File.Exists(ReviewsFilePath)) return;

            try
            {
                foreach (var line in File.ReadAllLines(ReviewsFilePath))
                {
                    reviewsStack.Push(line);
                }
            }
            catch
            {
                Console.WriteLine("Error loading reviews.");
            }
        }
    }
}
