using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NuGet.Packaging;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Net;
using System.Reflection;
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


            cmd.CommandText = "SELECT "+ field + " FROM " + table + " Where " + key + " = '" + value + "'";
            

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

        ///int currentAmount = int.Parse(GetField(selectorKey, selectorValue, table, field));

        public void increaseItemStock(string table, string id, int amount = 1)
        {
            try
            {
                int currentAmount = int.Parse(GetField("id", id, table, "stock"));


                if ( amount > 0)
                {
                    UpdateField(table, "id", id, "stock", (amount + currentAmount).ToString());
                }
                else
                {
                    throw new Exception("Cannot Add less than 1 item to stock");
                }
            }
            catch
            {
                throw new Exception("Failed to add item!");
            }
            
        }

        public void DecreaseItemStock(string table, string id, int amount = 1)
        {
            try
            {
                int currentAmount = int.Parse(GetField("id", id, table, "stock"));

                if (amount + currentAmount >= 0)
                {
                    UpdateField(table, "id", id, "stock", (currentAmount - amount).ToString());
                }
                else
                {
                    throw new Exception("Cannot reduce stock level below 0");
                }
            }
            catch
            {
                throw new Exception("Failed to remove item stock!");
            }

        }

        public string getFieldType(string table, string column)
        {
            cmd.CommandText = "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table + "' AND  COLUMN_NAME ='" + column +"'";

            // open database connection.
            conn.Open();

            //Execute the query 
            SqlDataReader sdr = cmd.ExecuteReader();

            ////Retrieve data from table and Display result
            sdr.Read();
            string dataType = (string)sdr[0];

            //Close the connection
            conn.Close();
            return dataType;


        }
        public void UpdateField(string table, string selectorKey, string selectorValue, string field, string fieldValue)
        {

            if (CheckRow(table, selectorKey, selectorValue))
            {

                //switch ()
                //{

                //}

                //open database connection.
                conn.Open();

                cmd.CommandText = "UPDATE " + table + " Set " + field + "  = " + fieldValue + " Where " + selectorKey + "= " + selectorValue;

                //Execute the query 
                cmd.ExecuteReader();

                //Close the connection
                conn.Close();


            }
            else
            {
                throw new Exception("Row with given field dosent exist");
            }
            return;
        }

        //public void CreateItemTable(Item item)
        //{
        //    //Check if table exist
        //    if (!CheckTable(item.modelName))
        //    {
        //        if (System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString.Contains("password")) 
        //        { 
        //            cmd.CommandText = "CREATE TABLE " + Name + "(" +
        //            "Id int NOT NULL AUTO_INCREMENT," +
        //            "Color varchar(255)," +
        //            "Size varchar(255)," +
        //            "ModelId Int," +
        //            "Reserved Int," +
        //            "Sold Int," +
        //            "Visible BOOL," +
        //            "RestockDate DATE," + //format YYYY-MM-DD
        //            "Stock Int)";
        //        }
        //        else
        //        {
        //            cmd.CommandText = "CREATE TABLE " + Name + "(" +
        //            "Id int NOT NULL IDENTITY(1,1) PRIMARY KEY," +
        //            "Color varchar(255)," +
        //            "Size varchar(255)," +
        //            "ModelId Int," +
        //            "Reserved Int," +
        //            "Sold Int," +
        //            "Visible bit," +
        //            "RestockDate DATE," + //format YYYY-MM-DD
        //            "Stock Int)";
        //        }

        //        // open database connection.
        //        conn.Open();

        //        //Execute the query 
        //        SqlDataReader sdr = cmd.ExecuteReader();

        //        conn.Close();
        //    }
        //    return;

        //}

        public List<string> getAllElementsField(string tableName, string key)
        {
            List<string> list = new List<string>();

            cmd.CommandText = "SELECT * FROM " + tableName;


            // open database connection.
            conn.Open();

            //Execute the query 
            SqlDataReader sdr = cmd.ExecuteReader();

            ////Retrieve data from table and Display result
            
            while (sdr.Read())
            {
                list.Add(sdr[key].ToString());


            }
            //Close the connection
            conn.Close();

            return list;
        }

        public List<IDictionary<string, object>> getAllElements(string tableName, object objectClass)
        {

            cmd.CommandText = "SELECT * FROM " + tableName;


            FieldInfo[] myField = objectClass.GetType().GetFields();

            // open database connection.
            conn.Open();



            List<IDictionary<string, object>> objects = new List<IDictionary<string, object>>();

            //Execute the query 
            SqlDataReader sdr = cmd.ExecuteReader();

            ////Retrieve data from table and Display result
            while (sdr.Read())
            {
                Dictionary<string, object> instance = new Dictionary<string, object>();
                foreach (FieldInfo field in myField)
                {
                    instance.Add(field.Name,sdr[field.Name].ToString());
                }
                objects.Add(instance);
            }
            //Close the connection
            conn.Close();

            return objects;
        }
        public void AddRowToTable(string table, List<(string,string)> keyValueSet)
        {

            //Id genation might need to be updated, whic affect values!!


            if (CheckTable(table))
            {
                cmd.CommandText = "INSERT INTO " + table + " (";
                string columns = "";
                string values = "";
                foreach( (string,string) set in keyValueSet)
                {
                    columns += set.Item1 + ", ";
                    values += "'" + set.Item2 + "', ";
                }
                columns += "itemTable";
                values += "'item" + keyValueSet[0].Item2 + "'";

                //INSERT INTO ItemModels(id, modelName, description, itemTable) VALUES(2435, 'Test shirt', 'Its a nice shirt', 'item2435')
                cmd.CommandText += columns + ") VALUES (" + values + ")";

                // open database connection.
                conn.Open();

                //Execute the query 
                 cmd.ExecuteReader();

                //Close the connection
                conn.Close();
            }
            else
            {
                throw new Exception("Table dosent exist");
            }
        }

        public void RemoveRow(string table, string key, string value)
        {



            if (CheckTable(table))
            {
                cmd.CommandText = "DELETE FROM " + table + " WHERE " + key + " = '" + value + "'";

                // open database connection.
                conn.Open();

                //Execute the query 
                cmd.ExecuteReader();

                //Close the connection
                conn.Close();
            }
            else
            {
                throw new Exception("Table dosent exist");
            }
        }


        public void CreateTable(string name, List<string> columns)
        {
            if (CheckTable(name))
            {
                //throw new Exception("Table already exist");
            }
            else { 
                cmd.CommandText = "CREATE TABLE " + name + "(";

                foreach(string column in columns)
                {
                    cmd.CommandText += column + ", ";
                }
                cmd.CommandText += ")";
                // open database connection.
                conn.Open();

                //Execute the query 
                SqlDataReader sdr = cmd.ExecuteReader();

                conn.Close();
            }
        }

        public void CreatePromoCode()
        {
            if (CheckTable("PromoCode"))
            {
                //add code here
            }
            else
            {
                List<string> columns = new List<string>();
                columns.Add(("test varchar(10)"));
                CreateTable("PromoCode", columns);
            }
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