using System;
using System.Linq;
using System.Collections.Generic;
using UserBankAccounts;

namespace BankAndBankFactory
{
    //Everything bank + factories related ------------------------------------------------
    public class DictionaryBank
    {
        Dictionary<string, IAccount> accountDictionary = new Dictionary<string, IAccount>();

        public List<string> ListAccountNames()
        {
            List<string> accountNames = accountDictionary.Keys.ToList();
            return accountNames;
        }
        
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

        public bool DeleteAccount(string name)
        {
            return this.accountDictionary.Remove(name);

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
}
