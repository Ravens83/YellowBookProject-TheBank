using System;

namespace MySimpleTools
{
    //Simple tool methods
    public class SimpleTools
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

        public static int ReadBuildReport(string filename)
        {
            int buildNumber = 0;
            System.IO.TextReader textIn = null;
            try
            {
                textIn = new System.IO.StreamReader(filename);
                buildNumber = int.Parse(textIn.ReadLine());
            }
            catch
            {
                throw new Exception("Error in filename input or corrupt file");
            }
            finally
            {
                if (textIn != null)
                {
                    textIn.Close();
                }
            }

            return buildNumber;
        }

        public static bool WriteBuildReport(string filename)
        {
            int buildNumber = ReadBuildReport(filename);

            System.IO.TextWriter textOut = null;
            try
            {
                buildNumber++;
                textOut = new System.IO.StreamWriter(filename);
                textOut.WriteLine(buildNumber);
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
    }


}
