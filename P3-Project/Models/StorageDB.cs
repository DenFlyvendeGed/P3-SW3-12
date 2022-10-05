using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
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
        public string GetField(string key, string value, string table, string field)
        {
            

            cmd.CommandText = "SELECT "+ field + " FROM " + table + " Where " + key + " = " + value;
            

            // open database connection.
            conn.Open();

            //Execute the query 
            SqlDataReader sdr = cmd.ExecuteReader();

            ////Retrieve data from table and Display result
            string Result = "";
            while (sdr.Read())
            {
                Result = sdr[field].ToString();
               
                
            }
            //Close the connection
            conn.Close();
            return Result;
        }
        public bool CheckRow(string table, string key, string value)
        {

            cmd.CommandText = "SELECT "+ key +" FROM " + table + " Where " + key + " = " + value;


            // open database connection.
            conn.Open();


            bool Result = true;
            try
            {
                //Execute the query 
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                sdr[key].ToString();
            }
            catch
            {
                Result = false;
            }

            Console.WriteLine(Result);
            //Close the connection
            conn.Close();
            return Result;
        }
        public bool CheckTable(string table)
        {
            cmd.CommandText = "SELECT * FROM " + table;


            // open database connection.
            conn.Open();

            
            bool Result = true;
            try
            {
                //Execute the query 
                cmd.ExecuteReader();
            }
            catch
            {
                Result = false;
            }

            Console.WriteLine(Result);
            //Close the connection
            conn.Close();
            return Result;
        }

        public void AddItem(string table, int id, int amount = 1)
        {
            if(CheckRow(table, "Id", id.ToString())) { 
                int currentAmount =  int.Parse(GetField("id", id.ToString(), table, "stock"));

                //open database connection.
                conn.Open();
                cmd.CommandText = "UPDATE " + table + " Set stock = " + (currentAmount + amount) + " Where id = " + id;

                //Execute the query 
                cmd.ExecuteReader();

                //Close the connection
                conn.Close();
            }
            else
            {
                throw new Exception("Item dosent exist");
            }
            return;
        }

        public void RemoveItem(string table, int id, int amount = 1)
        {
            if (CheckRow(table, "Id", id.ToString()))
            {
                int currentAmount = int.Parse(GetField("id", id.ToString(), table, "stock"));

                //open database connection.
                conn.Open();

                if (amount > currentAmount)
                    throw new Exception("Cant remove stock count to below 0");
                cmd.CommandText = "UPDATE " + table + " Set stock = " + (currentAmount - amount) + " Where id = " + id;

                //Execute the query 
                cmd.ExecuteReader();

                //Close the connection
                conn.Close();
            }
            else
            {
                throw new Exception("Item dosent exist");
            }
            return;
        }
        public void CreateItemTable(string Name)
        {
            //Check if table exist
            if (!CheckTable(Name))
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString.Contains("password")) 
                { 
                    cmd.CommandText = "CREATE TABLE " + Name + "(" +
                    "Id int NOT NULL AUTO_INCREMENT," +
                    "Color varchar(255)," +
                    "Size varchar(255)," +
                    "ModelId Int," +
                    "Reserved Int," +
                    "Sold Int," +
                    "Visible BOOL," +
                    "RestockDate DATE," + //format YYYY-MM-DD
                    "Stock Int)";
                }
                else
                {
                    cmd.CommandText = "CREATE TABLE " + Name + "(" +
                    "Id int NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                    "Color varchar(255)," +
                    "Size varchar(255)," +
                    "ModelId Int," +
                    "Reserved Int," +
                    "Sold Int," +
                    "Visible bit," +
                    "RestockDate DATE," + //format YYYY-MM-DD
                    "Stock Int)";
                }

                // open database connection.
                conn.Open();

                //Execute the query 
                SqlDataReader sdr = cmd.ExecuteReader();

                conn.Close();
            }
            return;

        }
        public void AddItemToTable(string modelId, string size, string color)
        {

        }

        public void CreatePromoCode()
        {

        }

        public void DeleteTable(string Name)
        {
            cmd.CommandText = "DROP TABLE " + Name ;


            // open database connection.
            conn.Open();

            //Execute the query 
            SqlDataReader sdr = cmd.ExecuteReader();

            conn.Close();
            return;

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