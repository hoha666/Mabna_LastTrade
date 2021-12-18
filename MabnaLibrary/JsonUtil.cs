using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace MabnaLibrary
{
    public static class JsonUtil
    {
        public static async Task<IEnumerable<LastTrade>> GetAsync(DateTime? startDate, CancellationToken ct)
        {
            try
            {
                if (startDate == null)
                    startDate = DateTime.MinValue;
                IEnumerable<LastTrade> result = null;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                DataTable lastTrades = DbContext.getDataSet("LastTrade", ct).Tables["LastTrade"]; // :)
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                var bres = lastTrades.AsEnumerable().Select(row => new LastTrade
                {
                    Id = Convert.ToInt32(row["Id"] is DBNull ? 0 : row["Id"]),
                    InstrumentId = Convert.ToInt32(row["InstrumentId"] is DBNull ? row["InstrumentId"] : 0),
#pragma warning disable CS8601 // Possible null reference assignment.
                    ShortName = Convert.ToString(row["ShortName"] is DBNull ? "" : row["ShortName"]),
#pragma warning restore CS8601 // Possible null reference assignment.
                    DateTimeEn = Convert.ToDateTime(row["DateTimeEn"] is DBNull ? DateTime.Now : row["DateTimeEn"]),
                    open = Convert.ToDecimal(row["open"] is DBNull ? 0m : row["open"]),
                    High = Convert.ToDecimal(row["High"] is DBNull ? 0m : row["High"]),
                    Low = Convert.ToDecimal(row["Low"] is DBNull ? 0m : row["Low"]),
                    Close = Convert.ToDecimal(row["Close"] is DBNull ? 0m : row["Close"]),
                    RealClose = Convert.ToDecimal(row["RealClose"] is DBNull ? 0m : row["RealClose"]),
                }).Where(t => t.DateTimeEn >= startDate);

                var tres = bres.ToList<LastTrade>();

                var jres = JsonConvert.SerializeObject(tres);
                string fileName = @"C:\temp\LastTrades.txt";
                CreateFile("{ \n" + jres + "\n}", fileName);
                return bres;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private static bool CreateFile(string textContext, string fileName)
        {
            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file    
                    Byte[] title = new UTF8Encoding(true).GetBytes(textContext);
                    fs.Write(title, 0, title.Length);
                }
                return true;

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
            return false;
        }
    }



    public class LastTrade
    {
        public int Id { get; set; }
        public int InstrumentId { get; set; }
        public string ShortName { get; set; }
        public DateTime DateTimeEn { get; set; }
        public decimal open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal RealClose { get; set; }
    }
}