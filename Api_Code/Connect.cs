//using System;
//using Oracle.ManagedDataAccess.Client;

//class OraConnect
//{
//    OracleConnection con;
//    public void Connect()
//    {
//        con = new OracleConnection();
//        con.ConnectionString = "User Id=jrocha;Password=jrocha;Data Source=LCHDB2";
//        con.Open();
//        Console.WriteLine("Connected to Oracle" + con.ServerVersion);
//        con.Close();
//        con.Dispose();
//    }
//}