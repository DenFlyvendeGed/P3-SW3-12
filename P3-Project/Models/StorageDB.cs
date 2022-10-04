using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace P3_Project.Models
{
    public class StorageDB
    {
        private SqlConnection conn = new SqlConnection();
        private SqlCommand cmd = new SqlCommand();
        public StorageDB()
        {
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

        }
        public string GetField(string key, string value, string table)


        {
            

            cmd.CommandText = "SELECT * FROM " + table + " Where " + key + " = " + value;
            

            // open database connection.
            conn.Open();

            //Execute the query 
            SqlDataReader sdr = cmd.ExecuteReader();

            ////Retrieve data from table and Display result
            string Result = "";
            while (sdr.Read())
            {
                Result = sdr[key].ToString();
                string? test = sdr["ingenting"].ToString();
                
            }
            //Close the connection
            conn.Close();
            return Result;
        }

        public void CreateTable(string Name, string location)
        {

        }
    } 
}
//while (sdr.Read())
//{
//    string itemName = (string)sdr["Name"];
//    string price = sdr["Price"].ToString();

//    list.Add(price);
//    list.Add(itemName);
//    Console.WriteLine(itemName);
//    Console.WriteLine(price);
//}