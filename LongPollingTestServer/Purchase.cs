
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace EFTServer
{
    public class Purchase
    {
        public int ID { get; set; }
        public int MerchantID { get; set; }
        public float Amount { get; set; }
        public string Status { get; set; } = string.Empty;

        public DataBaseConnetionDetails dbConnectionDetails;
        public PurchaseStatusDetails StatusDetails;
        public ServerClientTimerDetails TimerDetails;

        private SqlConnection con;
        private SqlCommand com;
        
        private void connection()
        {
            dbConnectionDetails = JsonConvert.DeserializeObject<DataBaseConnetionDetails>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "appsettings.json"));
            con = new SqlConnection(GetConnectionString());
        }

        private string GetConnectionString()
        {
            
            return "Data Source=" + dbConnectionDetails.DataSource + ";" + "Initial Catalog=" + dbConnectionDetails.InitialCatalogue + ";" + "Integrated Security = " + dbConnectionDetails.IntegratedSecurity + "; ";
   
        }
        public string AddPurchase(Purchase Pr)
        {
            connection();
            com = new SqlCommand("InsertPurchaseData", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@ID", Pr.ID);
            com.Parameters.AddWithValue("@MerchantID", Pr.MerchantID);
            com.Parameters.AddWithValue("@Amount", Pr.Amount);
            com.Parameters.AddWithValue("@Status", Pr.Status);
            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return "Purchase Request Accepted successfully";

            }
            else
            {
                return "Purchase Transaction failed";

            }
        }
        public DataTable GetAllRecordsPurchase()
        {
            DataTable dt = new DataTable();
            connection();
            com = new SqlCommand("GetAllRecordsPurchaseData", con);
            com.CommandType = CommandType.StoredProcedure;
            con.Open();
            using (SqlDataAdapter adapter = new SqlDataAdapter(com))
            {
                adapter.Fill(dt);
            }

            con.Close();
            return dt;
        }

        public DataTable GetPurchase(int ID)
        {
            DataTable dt = new DataTable(); 
            connection();
            com = new SqlCommand("GetPurchaseData", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@ID", ID);
            con.Open();
            using (SqlDataAdapter adapter = new SqlDataAdapter(com))
            {
                adapter.Fill(dt);
            }

            con.Close();
            return dt;
            
        }
       
        public string UpdatePurchaseStatus(int ID, string Status)
        {
            connection();
            com = new SqlCommand("UpdatePurchasestatus", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@ID", ID);
            com.Parameters.AddWithValue("@Status", Status);
            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return "Purchase Request Status Updated successfully";

            }
            else
            {
                return "Purchase Transaction status Update failed";

            }
        }

    }

    public class DataBaseConnetionDetails
    {
        public string DataSource { get; set; }
        public string InitialCatalogue { get; set; }
        public string IntegratedSecurity { get; set; }
    }

    public class PurchaseStatusDetails
    {
        public string RequestReceived { get; set; }
        public string Accepted { get; set; }
        public string SwipeCard { get; set; }
        public string SelectAccount { get; set; }
        public string EnterPIN { get; set; }
        public string Success { get; set; }
    }

    public class ServerClientTimerDetails
    {
        public int ServerTimer { get; set; }
        public int ClientTimer { get; set; }
    }
}