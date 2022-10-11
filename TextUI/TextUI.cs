using System;
using System.Collections.Generic;

using UserBankAccounts;
using BankAndBankFactory;
using MySimpleTools;

namespace TextUI
{
    //Start of user interface program------------------------------------
    public class UserTextUI
    {
        //fields:
        private DictionaryBank bank;

        //constructors:
        public UserTextUI(DictionaryBank inBank)
        {
            this.bank = inBank;
        }

        //methods:
        static public void RunUserTextUI(DictionaryBank inBank, string activateDataFile)
        {
            UserTextUI currentUI = new UserTextUI(inBank);

            string command;
            do
            {
                Console.WriteLine("\nWelcome to Gringotts!");
                Console.WriteLine("     To add an account enter the command: add");
                Console.WriteLine("     To edit an account enter the command: edit");
                Console.WriteLine("     To delete an account enter the command: delete");
                Console.WriteLine("     To list all accounts enter the command: list");
                Console.WriteLine("     To exit program enter the command: exit");
                Console.Write("Enter command : ");
                command = Console.ReadLine();
                command = command.Trim();
                command = command.ToLower();
                switch (command)
                {
                    case "add":
                        currentUI.AddNewAccount(activateDataFile);
                        break;
                    case "edit":
                        currentUI.EditCaller(activateDataFile);
                        break;
                    case "delete":
                        currentUI.DeleteAccount(activateDataFile);
                        break;
                    case "list":
                        currentUI.ListAccounts();
                        break;
                }
            } while (command != "exit");
        }

        // START OF ACCOUNT ADD FUNCTIONS
        // NEED TO ADD FUNCTIONALITY FOR MULTIPLE ACCOUNT TYPES. CURRENTLY ALL THESE ADDING FUNCTIONS MUST BE UPDATED IF NEW TYPES ARE ADDED
        public void AddNewAccount(string activateDataFile)                                 
        {
            bool creationCompleted = false;
            string command;
            do
            {
                Console.WriteLine("\nNew Account creation program");
                Console.WriteLine("     To create a normal account type the command: normal");
                Console.WriteLine("     To create a baby account type the command: baby");
                Console.WriteLine("     To cancel creation write the command: cancel");
                Console.Write("Enter command: ");
                command = Console.ReadLine();
                command = command.Trim();
                command = command.ToLower();
                if (command == "normal")
                {
                    AddNewNormalAccount();
                    creationCompleted = true;
                }
                else if (command == "baby")
                {
                    AddNewBabyAccount();
                    creationCompleted = true;
                }

            } while (command != "cancel" && !creationCompleted);

            this.bank.Save(activateDataFile);
        }

        public void AddNewNormalAccount()
        {
            string newName = TextUITools.TextUINewNameTest("Enter new name: ");
            string command = newName.Trim();
            command = command.ToLower();
            if (command != "cancel")
            {
                string reply;
                string newAccountBalanceString;
                do
                {
                    Console.WriteLine("     To cancel creation write the command: cancel");
                    Console.Write("Enter account balance: ");
                    newAccountBalanceString = Console.ReadLine();

                    command = newAccountBalanceString.Trim();
                    command = command.ToLower();

                    reply = MySimpleTools.SimpleTools.ValidateDecimalString(newAccountBalanceString);
                } while (reply != "" & command != "cancel");

                if (command != "cancel")
                {
                    decimal newAccountBalanceDecimal = decimal.Parse(newAccountBalanceString.Trim());
                    CustomerAccount newAcc = new CustomerAccount(newName, newAccountBalanceDecimal);
                    if (this.bank.StoreAccount(newAcc))
                        Console.WriteLine("{0} succesfully stored", newAcc.GetName());
                }
            }
        }

        public void AddNewBabyAccount()
        {
            string parentName;
            Console.WriteLine("     To cancel creation write the command: cancel");
            string newName = TextUITools.TextUINewNameTest("Enter new name: ");
            string command = newName.Trim();
            command = command.ToLower();

            if (command != "cancel")
            {
                Console.WriteLine("     To cancel creation write the command: cancel");
                parentName = TextUITools.TextUINewNameTest("Enter the parents name: ");
                command = parentName.Trim();
                command = command.ToLower();

                if (command != "cancel")
                {
                    string reply;
                    string newAccountBalanceString;
                    do
                    {
                        Console.WriteLine("     To cancel creation write the command: cancel");
                        Console.Write("Enter account balance: ");
                        newAccountBalanceString = Console.ReadLine();

                        command = newAccountBalanceString.Trim();
                        command = command.ToLower();

                        reply = MySimpleTools.SimpleTools.ValidateDecimalString(newAccountBalanceString);
                        if (reply != "")
                        {
                            Console.WriteLine(reply);
                        }
                    } while (reply != "" & command != "cancel");


                    if (command != "cancel")
                    {
                        decimal newAccountBalanceDecimal = decimal.Parse(newAccountBalanceString.Trim());
                        BabyAccount newBaby = new BabyAccount(newName, newAccountBalanceDecimal, parentName);
                        if (this.bank.StoreAccount(newBaby))
                            Console.WriteLine("{0} succesfully stored", newBaby.GetName());
                    }
                }
            }
        }
        // END OF ACCOUNT ADD FUNCTIONS (THESE MUST BE UPDATED FOR NEW ACCOUNT TYPES)


        public void EditCaller(string activateDataFile)
        {
            IAccount storedAccount;

            bool editCompleted = false;

            string command;
            do
            {
                Console.WriteLine("\nAccount editing program");
                Console.WriteLine("     To cancel write the command: cancel");
                Console.Write("Enter account name: ");
                command = Console.ReadLine();
                command = command.Trim();
                if (command != "cancel")
                {
                    storedAccount = bank.FindAccount(command);
                    if (storedAccount != null)
                    {
                        AccountEditTextUI.DoEdit(storedAccount,this.bank);
                        editCompleted = true;
                    }
                    else
                        Console.WriteLine("Invalid Account Name");
                }

            } while (command != "cancel" && !editCompleted);

            this.bank.Save(activateDataFile);
        }

        public void DeleteAccount(string activateDataFile)
        {
            IAccount storedAccount;

            bool editCompleted = false;

            string command;
            do
            {
                Console.WriteLine("\nYou are about to close and delete and account from the bank system");
                Console.WriteLine("     To cancel write the command: cancel");
                Console.Write("Enter account name: ");
                command = Console.ReadLine();
                command = command.Trim();
                if (command.ToLower() != "cancel")
                {
                    storedAccount = bank.FindAccount(command);
                    if (storedAccount != null)
                    {
                        string accountName;
                        do
                        {
                            accountName = storedAccount.GetName();
                            Console.WriteLine("WARNING: You are about to delete the account for: {0}", accountName);
                            Console.WriteLine("Are you sure you want to continue?");
                            Console.WriteLine("     To delete the account type: Yes");
                            Console.WriteLine("     To cancel the process type: cancel");
                            Console.Write("Enter command: ");
                            command = Console.ReadLine();
                            command = command.Trim();
                        } while (command.ToLower() != "cancel" && command != "Yes" && command != "No");

                        if(command == "Yes")
                        {
                            bool success = this.bank.DeleteAccount(accountName);
                            if(!success)
                                throw new Exception ("Unkown error in the Account removal process");
                            else
                                Console.WriteLine("The account for {0} has been successfully removed from the bank", accountName);
                        }

                        editCompleted = true;
                    }
                    else
                        Console.WriteLine("Invalid Account Name");
                }

            } while (command.ToLower() != "cancel" && !editCompleted);

            this.bank.Save(activateDataFile);
        }

        public void ListAccounts()
        {
            Console.WriteLine("Here are the account names for all accounts in the bank");
            List<string> accountNames = this.bank.ListAccountNames();
            accountNames.ForEach(Console.WriteLine);
        }
    }

    
    //Start of user account edit methods----------------------------------------------
    public class AccountEditTextUI
    {
        //fields:
        private IAccount account;
        private DictionaryBank bank;

        //constructors:
        public AccountEditTextUI(IAccount inAccount, DictionaryBank inBank)
        {
            this.account = inAccount;
            this.bank = inBank;
        }

        //methods:
        static public void DoEdit(IAccount inAccount, DictionaryBank inBank)
        {
            AccountEditTextUI edit = new AccountEditTextUI(inAccount, inBank);

            string command;
            do
            {
                Console.WriteLine("\nEditing account for {0}", inAccount.GetName());
                Console.WriteLine("     To edit name enter the command: name");
                Console.WriteLine("     To read balance enter the command: balance");
                Console.WriteLine("     To pay in funds enter the command: pay");
                Console.WriteLine("     To draw out funds enter the command: draw");
                Console.WriteLine("     To exit enter the command: exit");
                Console.Write("Enter command : ");
                command = Console.ReadLine();
                command = command.Trim();
                command = command.ToLower();
                switch (command)
                {
                    case "name":
                        edit.AccountEditTextUIEditName();
                        break;
                    case "balance":
                        edit.AccountEditTextUIReadBalance();
                        break;
                    case "pay":
                        edit.AccountEditTextUIPayInFunds();
                        break;
                    case "draw":
                        edit.AccountEditTextUIWithdrawFunds();
                        break;
                }

            } while (command != "exit");
        }

        public void AccountEditTextUIEditName()
        {
            Console.WriteLine("Name Edit");
            Console.WriteLine("     To cancel enter the command: cancel");

            string oldName = this.account.GetName();

            string newName = TextUITools.TextUINewNameTest("Enter new name: ");

            if (newName.ToLower() != "cancel")
            {
                this.account.SetName(newName);

                this.bank.StoreAccount(this.account);
                this.bank.DeleteAccount(oldName);
                Console.WriteLine("Account has changed name to: {0}",newName);
            }
            else
            {
                Console.WriteLine("Name edit was cancelled.");
            }
        }

        public void AccountEditTextUIPayInFunds()
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

        public void AccountEditTextUIWithdrawFunds()
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
            else
                Console.WriteLine("{0} withdrawn",Math.Abs(newValueD));
        }

        public void AccountEditTextUIReadBalance()
        {
            Console.WriteLine("Current Balance : {0}", this.account.GetBalance());
        }
    }

    //End of user account edit methods----------------------------------------------
    
    //Start of various methods to be used within the TextUI-------------------------
    public static class TextUITools
    {
        public static string TextUINewNameTest(string prompt)                                   
        {
            string newName;
            while (true)
            {
                Console.Write(prompt);
                newName = Console.ReadLine();
                string reply;
                reply = UserBankAccounts.Account.ValidateName(newName);

                if (reply.Length == 0)
                {
                    break;
                }
                else
                    Console.WriteLine("Invalid name : " + reply);
            }

            return newName;
        }
    }
    //End of various methods to be used within the TextUI-------------------------
}
