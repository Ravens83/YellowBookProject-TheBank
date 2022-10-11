using System;

namespace UserBankAccounts
{
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

            if (SetName(inName) == false)
            {
                errorMessage = errorMessage + "Bad Name: " + inName;
            }

            if (SetBalance(inBalance) == false)
            {
                errorMessage = errorMessage + "Bad Balance: " + inBalance;
            }

            if (errorMessage != "")
            {
                throw new Exception("Account construction failed " + errorMessage);
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
            }
        }

        //methods:
        public bool PayInFunds(decimal amount)
        {
            if (amount > 0)
            {
                this.balance = this.balance + amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool WithdrawFunds(decimal amount)
        {
            decimal safeValue = Math.Abs(amount);
            if (this.balance >= safeValue)
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
            if (trimmedName.Length == 0)
            {
                return "No text in the name";
            }
            return "";
        }

        public bool SetName(string inName)      //WARNING. IF THE NAME IS KEY IN A LIBRARYBANK YOU MUST REMOVE AND ADD EVERY TIME THE NAME IS EDITED
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
                if (textOut != null)
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
        public CustomerAccount(string inName, decimal inBalance) : base(inName, inBalance)
        {
        }

        public CustomerAccount(System.IO.TextReader textIn) : base(textIn)
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

        public override bool WithdrawFunds(decimal amount)
        {
            decimal safeValue = Math.Abs(amount);
            if ((this.balance >= safeValue) && (safeValue <= 10))
            {
                this.balance = this.balance - safeValue;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //End of everything account related ------------------------------------------------
}
