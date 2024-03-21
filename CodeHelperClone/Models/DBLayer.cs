using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace CodeHelperClone.Models
{
    public class DBLayer
    {
        SqlConnection con;
        public DBLayer()
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        }
        //method to excute a stored procedure that return number of row affected 
        public int ExecuteDML(string procname, SqlParameter[] param
            )
        {
            SqlCommand cmd = new SqlCommand(procname, con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            foreach (SqlParameter p in param)
            {
                if (p.Value != null)
                {
                    cmd.Parameters.Add(p);
                }
            }
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            return result;
        }

        //function to execute a stored procedure thet return a datatable or table as a return

        public DataTable ExecuteSelect(string procname, SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand(procname, con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            foreach (SqlParameter p in param)
            {
                if (p.Value != null)
                {
                    cmd.Parameters.Add(p);
                }
            }
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            return dt;
        }
        //function to execute a stired procedure that return datatable aas a table which as no parameters
        public DataTable ExecuteSelect(string procname)
        {
            SqlCommand cmd = new SqlCommand(procname, con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            return dt;
        }

        // a function is used to execute stored procedure that return a one row and only one column as a responce

        public object ExecuteScalar(string procname, SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand(procname, con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            foreach (SqlParameter p in param)
            {
                if (p.Value != null)
                {
                    cmd.Parameters.Add(p);
                }
            }
            if (con.State == ConnectionState.Closed)
                con.Open();
            object res = cmd.ExecuteScalar();
            con.Close();

            return res;
        }
        // execute a stored procedure select only one row and only one column and does not need any parameters
        public object ExecuteScalar(string procname)
        {
            SqlCommand cmd = new SqlCommand(procname, con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            if (con.State == ConnectionState.Closed)
                con.Open();
            object res = cmd.ExecuteScalar();
            con.Close();

            return res;
        }
    }
}