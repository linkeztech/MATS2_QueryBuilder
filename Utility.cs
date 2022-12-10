using Newtonsoft.Json.Linq;

namespace MATS2_QueryBuilder
{
    public class Utility
    {
        //to read Database connection string from a file
        public static JObject? ReadMYSQLDBConfigFile()
        {
            try
            {
                string text = File.ReadAllText(@"Project/Settings/MYSQLDBConnection.json");
                JObject dbConnObject = JObject.Parse(text);
                //Console.WriteLine("Contents of file = {0}", dbConnObject.ToString());
                return dbConnObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to read from file : {0}", ex.Message);
            }
            return null;
        }//end of readfile
    }
}
