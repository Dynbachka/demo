using System.Data;
using System.Data.SqlClient;

namespace ShoesApp
{
    internal class DatabaseHelper
    {
        private static string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShoesDB;Integrated Security=True";

        public static DataTable GetTable(string query, params SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                new SqlDataAdapter(cmd).Fill(table);
            }
            return table;
        }

        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}