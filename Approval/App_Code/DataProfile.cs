﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Approval
{
    public class DataProfile
    {
        string connect = @"Data Source=10.120.112.215;Initial Catalog=Approval;Persist Security Info=True;User ID=sa;Password=Mtv@2020;MultipleActiveResultSets=True";
        //string connect = @"Data Source = .\SQLEXPRESS;Initial Catalog = Approval; Persist Security Info=True;User ID = sa; Password=towada";
        SqlConnection conn = null;
        SqlDataAdapter da = null;
        SqlCommand cmd = null;
        public DataProfile()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public void Connect()
        {
            conn = new SqlConnection(connect);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }
        public void Disconnect()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }
        public DataTable GetDataTable(string sql)
        {
            try
            {
                Connect();
                da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Disconnect();
                return dt;
            }
            catch
            {
                return null;
            }
        }
        public void ExcuteQuery(string sql)
        {
            Connect();
            cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            Disconnect();
        }
    }
}