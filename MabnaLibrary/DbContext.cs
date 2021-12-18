using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabnaLibrary
{
    public static class DbContext
    {
        private static string ConnectionString = "Server=DESKTOP-3DMUPRF\\SQLEXPRESS;Database=TestDb1;User Id=sa;Password=Admin123.;";

        public static bool CallProcedure(string ProcedureName)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {

                SqlCommand cmd = new SqlCommand(ProcedureName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                return false;
            }
        }

        public static async Task CreateProcedure()
        {
            string queryString = @"                             
                                CREATE OR ALTER PROCEDURE LastTradeProc
	                                -- Add the parameters for the stored procedure here
	
                                AS
                                BEGIN	                               
	                                SET NOCOUNT ON;                                   
                                    IF OBJECT_ID(N'dbo.LastTrade', N'U') IS NOT NULL  
                                    DROP TABLE [dbo].LastTrade;  

	                                CREATE TABLE LastTrade (
	                                Id int not null,
	                                InstrumentId int,
	                                ShortName nvarchar(max),
	                                DateTimeEn datetime,
	                                [open] decimal(18, 4),
	                                [High] decimal(18, 4),
	                                [Low] decimal(18, 4),
	                                [Close] decimal(18, 4),
	                                RealClose decimal(18, 4)
                                );

                                with asd as (
	                                select	
		                                InstrumentID as iid,		
                                        max(DateTimeEn) over (partition by InstrumentID) max_my_date
                                from   Trade
                                )
                                , a as (select distinct * from Trade t inner join asd on asd.iid = t.InstrumentID where t.DateTimeEn = asd.max_my_date )
                                insert into LastTrade
                                select  a.ID as Id,
		                                a.InstrumentID as InstrumentId,
		                                i.ShortName,
		                                a.DateTimeEn,
		                                a.OpenPrice as [open],
		                                a.HighPrice as [High],
		                                a.LowPrice as [Low],
		                                a.ClosePrice as [Close],
		                                a.RealClosePrice as RealClose
		                                from a inner join Instrument i
	                                on i.ID = a.InstrumentID
                                END";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
        }

        public static DataSet getDataSet(string tableName , CancellationToken cts)
        {
            string queryString = "select * from dbo." + tableName; // Im in harry :) :)
            SqlConnection con = new SqlConnection(ConnectionString);
            cts.ThrowIfCancellationRequested();
            SqlDataAdapter adapter = new SqlDataAdapter(queryString, con);
            DataSet result = new DataSet();
            adapter.Fill(result, tableName);
            return result;
        }
    }
}
