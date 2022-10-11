using System;
using System.IO;

using UserBankAccounts;
using BankAndBankFactory;
using MySimpleTools;
using TextUI;

namespace The_Bank
{
    class Program
    {
        public static void Main()
        {
            int activeBuildNumber;
            string buildReportFile = "CurrentBuild.txt";
            string bankFile = "Bankdata.txt";
            string activeBankFile = "BankTemporary.txt";

            SimpleTools.FileExistsCheck(buildReportFile);   //Creates and initiliaizes new bank if none is fund
            SimpleTools.FileExistsCheck(bankFile);          //
            SimpleTools.FileExistsCheck(activeBankFile);    //

            activeBuildNumber = SimpleTools.ReadBuildReport(buildReportFile)+1;

            string backupBankFile = @"BankdataBackups\BackupBankData" + activeBuildNumber + ".txt";

            DictionaryBank gringotts = DictionaryBank.Load(bankFile);
            gringotts.Save(backupBankFile);
            
            TextUI.UserTextUI.RunUserTextUI(gringotts,activeBankFile);

            gringotts.Save(bankFile);
            SimpleTools.WriteBuildReport(buildReportFile);
            Console.Read();
        }
    }
}
