using Microsoft.AspNetCore.Hosting;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptsRunner
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("settings.json");

            var config = builder.Build();
            var settings = config.GetSection("Settings").Get<Settings>();

            RunScripts(settings);
        }


        static private void RunScripts(Settings settings)
        {
            string folderName = "Db_Intelisale";
            var fileLines = System.IO.Directory.GetFiles(folderName);

            string folderNameADM = "Db_Intelisale_Adm";
            var fileLinesADM = System.IO.Directory.GetFiles(folderNameADM);

            var sLConnections = settings.Connections.Where(x => x.Enabled).Select(x => x.ConnectionString);
            var sLAdmConnections = settings.Connections.Where(x => x.Enabled).Select(x => x.AdmConnectionString);

            foreach (var item in sLConnections)
            {
                var conn = BuildConnectionString(item);
                SqlConnection sqlConnection = new SqlConnection(conn);

                sqlConnection.Open();

                string[] splits = conn.Split(';');
                string[] anothersplit = splits[1].Split('=');
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Preapare to execute Country: {0}", anothersplit[1]);

                foreach (var file in fileLines.OrderBy(x =>x))
                {
                    string script = File.ReadAllText(@file);

                    try
                    {
                        new SqlCommand(script, sqlConnection).ExecuteNonQuery();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Executed: {0}", file);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR!!! {0} ---- {1}", file, ex.Message);
                    }
                }

                sqlConnection.Close();

            }


            foreach (var item in sLAdmConnections)
            {
                var conn = BuildConnectionString(item);
                SqlConnection sqlConnection = new SqlConnection(conn);

                sqlConnection.Open();

                string[] splits = conn.Split(';');
                string[] anothersplit = splits[1].Split('=');
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Preapare to execute Country: {0}", anothersplit[1]);

                foreach (var file in fileLinesADM.OrderBy(x => x))
                {
                    string script = File.ReadAllText(@file);

                    try
                    {
                        new SqlCommand(script, sqlConnection).ExecuteNonQuery();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Executed: {0}", file);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR!!! {0} ---- {1}", file, ex.Message);
                    }
                }

                sqlConnection.Close();

            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("\nPress any key to exit...");
            Console.ReadKey(true);
        }
        
        static private string BuildConnectionString(ConnectionString connectionString)
        {
            var conn = $"Data Source = {connectionString.Host}; Initial Catalog = {connectionString.DbName}; Integrated Security = False; Persist Security Info = False; User ID = {connectionString.User}; Password = {connectionString.Password}";

            return conn;
        }
    }
}
