using System;
using System.IO;
using System.Collections.Generic;

namespace The_Bank
{
    class Program
    {
        public static void Main()
        {
            ErrorCollector.Start();
            DictionaryBank gringotts = new DictionaryBank();

            CustomerAccount newAcc = new CustomerAccount("Potter", 100000);

            if (gringotts.StoreAccount(newAcc))
                Console.WriteLine("{0} succesfully stored", newAcc.GetName());

            BabyAccount newBaby = new BabyAccount("Ron", 20, "Arthur");

            if (gringotts.StoreAccount(newBaby))
                Console.WriteLine("Baby {0} succesfully stored", newBaby.GetName());

            gringotts.Save("Bankdata.txt");

            DictionaryBank cloneBank = DictionaryBank.Load("Bankdata.txt");

            IAccount storedAccount = cloneBank.FindAccount("Potter");
            if (storedAccount != null)
                Console.WriteLine(storedAccount.GetName() + " has been cloned");

            storedAccount = cloneBank.FindAccount("Ron");
            if (storedAccount != null)
                Console.WriteLine(storedAccount.GetName() + " has been cloned");

            storedAccount = cloneBank.FindAccount("Potter");
            if (storedAccount != null)
                if (!storedAccount.SetName(" Fotter"))
                    Console.WriteLine("Setname Fotter has failed");

            storedAccount = cloneBank.FindAccount("Ron");
            if (storedAccount != null)
                if (!storedAccount.SetName("  "))
                    Console.WriteLine("Setname to blank string has failed");

            storedAccount = cloneBank.FindAccount("Ron");
            if (storedAccount != null)
                AccountEditTextUI.DoEdit(storedAccount);

                cloneBank.Save("CloneBankdata.txt");

            ErrorCollector.FinalizeAndEnd();
            Console.Read();
        }
    }

    //Everything bank + factories related ------------------------------------------------
    class DictionaryBank
    {
        Dictionary<string, IAccount> accountDictionary = new Dictionary<string, IAccount>();

        public IAccount FindAccount(string name)
        {
            if (this.accountDictionary.ContainsKey(name))
                return this.accountDictionary[name];
            else
                return null;
        }

        public bool StoreAccount(IAccount account)
        {
            if (this.accountDictionary.ContainsKey(account.GetName()))
                return false;

            this.accountDictionary.Add(account.GetName(), account);
            return true;
        }

        public void Save(string filename)
        {
            System.IO.TextWriter textOut = new System.IO.StreamWriter(filename);

            textOut.WriteLine(this.accountDictionary.Count); //First line in the file will be the number of accounts
            foreach (IAccount account in this.accountDictionary.Values)
            {
                textOut.WriteLine(account.GetType().Name);
                account.Save(textOut);
            }

            textOut.Close();
        }

        public static DictionaryBank Load(string filename)
        {
            System.IO.TextReader textIn = new System.IO.StreamReader(filename);

            DictionaryBank result = new DictionaryBank();
            int count = int.Parse(textIn.ReadLine()); //First line in the file is the number of accounts

            for (int i = 0; i < count; i++)
            {
                string className = textIn.ReadLine();
                IAccount account = AccountFactory.MakeAccount(className, textIn);
                result.accountDictionary.Add(account.GetName(), account);
            }

            textIn.Close();
            return result;
        }

    }

    class AccountFactory //returns account classes constructed specific to their class. MUST BE UPDATE WHEN NEW CUSTOMER ACCOUNT CLASSES ARE ADDED
    {
        public static IAccount MakeAccount(string name, System.IO.TextReader textIn)
        {
            switch (name)
            {
                case "CustomerAccount":
                    return new CustomerAccount(textIn);
                case "BabyAccount":
                    return new BabyAccount(textIn);
                default:
                    return null;

            }
        }
    }
    //End of everything bank + factories related ------------------------------------------------

    //Everything account editing and textIU related ------------------------------------------------
    public class AccountEditTextUI
    {
        //fields:
        private IAccount account;

        //constructors:
        public AccountEditTextUI(IAccount inAccount)
        {
            this.account = inAccount;
        }

        //methods:
        static public void DoEdit (IAccount inAccount)
        {
            AccountEditTextUI edit = new AccountEditTextUI(inAccount);
            
            string command;
            do
            {
                Console.WriteLine("Editing account for {0}", inAccount.GetName());
                Console.WriteLine("     To edit name enter the command: name");
                Console.WriteLine("     To read balance enter the command: balance");
                Console.WriteLine("     To pay in funds enter the command: pay");
                Console.WriteLine("     To draw out funds enter the command: draw");
                Console.WriteLine("     To exit program enter the command: exit");
                Console.Write("Enter command : ");
                command = Console.ReadLine();
                command = command.Trim();
                command = command.ToLower();
                switch (command)
                {
                    case "name":
                        edit.TextUIEditName();
                        break;
                    case "balance":
                        edit.TextUIReadBalance();
                        break;
                    case "pay":
                        edit.TextUIPayInFunds();
                        break;
                    case "draw":
                        edit.TextUIWithdrawFunds();
                        break;
                }

            } while (command != "exit");
        }

        public void TextUIEditName()
        {
            string newName;
            Console.WriteLine("Name Edit");
            while (true)
            {
                Console.Write("Enter new name: ");
                newName = Console.ReadLine();
                string reply;
                reply = Account.ValidateName(newName);

                if (reply.Length == 0)
                {
                    break;
                }
                else
                    Console.WriteLine("Invalid name : " + reply);
            }
            this.account.SetName(newName);
        }

        public void TextUIPayInFunds()
        {
            string newValue;
            Console.WriteLine("Pay in");
            while (true)
            {
                Console.Write("Enter value to pay in: ");
                newValue = Console.ReadLine();
                string reply;
                reply = SimpleTools.ValidateDecimalString(newValue);

                if (reply.Length == 0)
                {
                    break;
                }
                else
                    Console.WriteLine("Invalid input : " + reply);
            }

            decimal newValueD = decimal.Parse(newValue.Trim());

            this.account.PayInFunds(newValueD);
        }

        public void TextUIWithdrawFunds()
        {
            string newValue;
            Console.WriteLine("Withdraw");
            while (true)
            {
                Console.Write("Enter value to withdraw: ");
                newValue = Console.ReadLine();
                string reply;
                reply = SimpleTools.ValidateDecimalString(newValue);

                if (reply.Length == 0)
                {
                    break;
                }
                else
                    Console.WriteLine("Invalid input : " + reply);
            }

            decimal newValueD = decimal.Parse(newValue.Trim());

            if (!this.account.WithdrawFunds(newValueD))
                Console.WriteLine("ERROR: Not enough funds");
        }

        public void TextUIReadBalance()
        {
            Console.WriteLine("Current Balance : {0}", this.account.GetBalance());
        }
    }


    //End of everything account editing and textIU related -----------------------------------------


    //Everything account related ------------------------------------------------
    //Interfaces:
    public interface IAccount
    {
        bool PayInFunds(decimal amount);
        bool WithdrawFunds(decimal amount);
        decimal GetBalance();
        string GetName();
        bool SetName(string inName);

        bool Save(string filename);
        void Save(System.IO.TextWriter textout);
        string ToString();
    }

    public abstract class Account : IAccount
    {
        //fields:
        protected string name;
        protected decimal balance;

        //constructors:
        public Account(string inName, decimal inBalance)
        {
            string errorMessage = "";

            if(SetName(inName) == false)
            {
                errorMessage = errorMessage + "Bad Name: " + inName;
            }

            if(SetBalance(inBalance) == false)
            {
                errorMessage = errorMessage + "Bad Balance: " + inBalance;
            }

            if (errorMessage != "")
            {
                throw new Exception("Account construction failed " + errorMessage);
                //ErrorCollector.AddAndPrint("Account construction failed " + errorMessage);
            }
        }

        public Account(System.IO.TextReader textIn)
        {
            string errorMessage = "";

            string tmpName = textIn.ReadLine();
            if (SetName(tmpName) == false)
            {
                errorMessage = errorMessage + "Bad Name: " + tmpName;
            }

            string balanceText = textIn.ReadLine();
            decimal tmpBalance = decimal.Parse(balanceText);
            if (SetBalance(tmpBalance) == false)
            {
                errorMessage = errorMessage + "Bad Balance: " + tmpBalance;
            }

            if (errorMessage != "")
            {
                throw new Exception("Account construction failed " + errorMessage);
                //ErrorCollector.AddAndPrint("Account construction failed " + errorMessage);
            }
        }

        //methods:
        public bool PayInFunds(decimal amount)
        {
            if(amount>0)
            {
                this.balance = this.balance + amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool WithdrawFunds(decimal amount)
        {
            decimal safeValue = Math.Abs(amount);
            if (this.balance > safeValue)
            {
                this.balance = this.balance - safeValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        public decimal GetBalance()
        {
            return this.balance;
        }

        public string GetName()
        {
            return this.name;
        }

        public static string ValidateName(string testName)
        {
            if (testName == null)
            {
                return "Name parameter null";
            }
            string trimmedName = testName.Trim();
            if(trimmedName.Length == 0)
            {
                return "No text in the name";
            }
            return "";
        }

        public bool SetName(string inName)
        {
            string reply;
            reply = ValidateName(inName);
            if (reply.Length > 0)
            {
                return false;
            }

            this.name = inName.Trim();
            return true;

        }

        public virtual void Save(System.IO.TextWriter textOut)
        {
            textOut.WriteLine(this.name);
            textOut.WriteLine(this.balance);
        }

        public bool Save(string filename)
        {
            System.IO.TextWriter textOut = null;
            try
            {
                textOut = new System.IO.StreamWriter(filename);
                Save(textOut);
            }
            catch
            {
                return false;
            }
            finally
            {
                if(textOut != null)
                {
                    textOut.Close();
                }

            }
            return true;
        }
        public override string ToString()
        {
            return "Name: " + this.name + "Balance: " + this.balance;
        }


        //Setters for private use:


        private bool SetBalance(decimal inBalance)          //To be improved?
        {
            this.balance = inBalance;
            return true;
        }
    }

    public sealed class CustomerAccount : Account
    {
        //constructors
        public CustomerAccount(string inName, decimal inBalance):base(inName, inBalance)
        {
        }

        public CustomerAccount(System.IO.TextReader textIn):base(textIn)
        {
        }
    }

    public sealed class BabyAccount : Account
    {
        string parentName;
        //constructors
        public BabyAccount(string inName, decimal inBalance, string inParentName) : base(inName, inBalance)
        {
            string errorMessage = "";

            if (SetParentName(inParentName) == false)
            {
                errorMessage = errorMessage + "Bad Parent Name: " + inParentName;
            }

            if (errorMessage != "")
            {
                throw new Exception("Account construction failed " + errorMessage);
                //ErrorCollector.AddAndPrint("Account construction failed " + errorMessage);
            }

        }

        public BabyAccount(System.IO.TextReader textIn) : base(textIn)
        {
            string errorMessage = "";

            string tmpParentName = textIn.ReadLine();
            if (SetParentName(tmpParentName) == false)
            {
                errorMessage = errorMessage + "Bad Parent Name: " + tmpParentName;
            }

            if (errorMessage != "")
            {
                throw new Exception("Account construction failed " + errorMessage);
                //ErrorCollector.AddAndPrint("Account construction failed " + errorMessage);
            }
        }

        //methods

        public override void Save(System.IO.TextWriter textOut)
        {
            base.Save(textOut);
            textOut.WriteLine(this.parentName);
        }
        public bool SetParentName(string inParentName)
        {
            string reply;
            reply = ValidateName(inParentName);
            if (reply.Length > 0)
            {
                return false;
            }

            this.parentName = inParentName.Trim();
            return true;
        }
    }

    //End of everything account related ------------------------------------------------

    //Simple tool methods
    class SimpleTools
    {
        public static string ValidateDecimalString(string inValue)
        {
            if (inValue == null)
            {
                return "Value input null";
            }
            string trimmedInValue = inValue.Trim();
            if (trimmedInValue.Length == 0)
            {
                return "Blank input";
            }
            if (!decimal.TryParse(trimmedInValue, out decimal tmp))
            {
                return "Not a value decimal input";
            }

            return "";
        }
    }



















        /*
         * The Error collection class.
         * To be improved:
         * Lookup table with error codes instead of random custom text in the program
         * Custom filename input and rewritten to match that functionality. This will allow multiple levels/types of errors
       */
    public static class ErrorCollector         //Inteded to be created at the start of a program. Collects and prints costumized errors to be assigned througout
    {                                   //the design of a program. Like object construction with parameters unacceptible to the programmer.
        //fields:
        private static string filename;
        private static int errCount;

        //constructors:
        public static void Start()
        {
            filename = "CriticalErrorsReport.txt";      //TO BE IMPROVED to have custom names - at the moment all instances would gangrape the same file
            errCount = 0;
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                sw.WriteLine("All critical errors reported during runtime are recorded here");
                sw.Close();
            }
            
        }

        //I want to at some point write a general constructor that takes a filename, checks validity and creates that file
        /*public CriticalErrors(string inFilename)
        {
            inFilename = inFilename.Trim();
            if (!string.IsNullOrEmpty(inFilename))
            {
                filename = inFilename; //+ ".txt" here? to not let people give crazy file types
            }
            else
            {
                filename = "CriticalErrorsReport.txt";
            }
        }*/

        //methods:
        public static void AddAndPrint(string errorMessage)    //To be called at every Error throwing in the program.
        {
            errCount++;
            using (StreamWriter sw = new StreamWriter(filename, true))
            {
                sw.WriteLine("Error number {0}:", errCount);
                sw.WriteLine(errorMessage);
                sw.Close();
            }

        }

        public static void FinalizeAndEnd()                    //method to be called at the end of a program run
        {
            using (StreamWriter sw = new StreamWriter(filename, true))
            {
                sw.WriteLine("=======================================================");

                if (errCount > 0)
                {
                    sw.WriteLine("The total number of critical errors reported: {0}",errCount);
                }
                else
                {
                    sw.WriteLine("No critical errors reported!");
                }

            sw.Close();
            }

            if (errCount > 0)
            {
                throw new Exception("WARNING: Critical Errors Reported. Check Error Report.");
            }
        }
        //public override string ToString()
        //{
        //    return "filename: " + filename + "errCount: " + errCount;
        //}

    }
}
