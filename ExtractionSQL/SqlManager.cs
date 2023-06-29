using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using CsvHelper;

namespace ExtractionSQL
{
    public class SqlManager : IDisposable
    {
        SqlConnection connection;

        public SqlManager(string serverName, string database, string username, string pwd)
        {
            string connectionString = null;
            if (!(username.Equals("") || pwd.Equals("")))
            {
                Console.WriteLine("connection par user et pwd");
                connectionString = "Data Source=" + serverName + ";Initial Catalog=" + database + ";User ID=" + username + ";Password=" + pwd;
            }
            else
            {
                Console.WriteLine("connexion par authentification windows");
                connectionString = "Data Source=" + serverName + ";Initial Catalog=" + database + ";Integrated Security=SSPI";
            }
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                Console.WriteLine("La connexion à la base de données a été établie avec succès.");
            }
            catch (Exception e)
            {
                Console.WriteLine("La connexion à la base de données a échoué : " + e.Message);
                Dispose();
            }
        }
        public void CreateDirectory(string outputPath)
        {
            string directoryPath = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                    Console.WriteLine("Répertoire crée :" + directoryPath);
                }catch(Exception e)
                {
                    Console.WriteLine(e);

                }       
            }
        }

        public bool TransformSqlToCsv(string query, string outputPath)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //créer le répertoire si il existe pas
                        CreateDirectory(outputPath);
                        using (var writer = new StreamWriter(outputPath))
                        {
                            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                            {
                                //entete de colonnes
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    csv.WriteField(reader.GetName(i));
                                }
                                csv.NextRecord();

                                //écrire les données
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        csv.WriteField(reader[i]);
                                    }
                                    csv.NextRecord();
                                }
                            }
                        }
                    }
                    return true;
                }
            }catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public DataTable ExecuteQueryForDatatable(string query)
        {
            DataTable dataTable = new DataTable();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            return dataTable;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
