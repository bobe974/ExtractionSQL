using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractionSQL
{
    class Program
    {
        public static string inifilePath = Path.Combine(Directory.GetCurrentDirectory(), "config.ini");
        public static SqlManager sqlManager = null;
        static string query = "SELECT fdl.AR_Ref AS Ref_Article, AR_Design AS Designation, SUM(Dl_Qte) AS Besoin_cumule FROM F_DOCLIGNE fdl " +
                   "INNER JOIN F_ARTICLE fa ON fdl.AR_Ref = fa.AR_Ref " +
                   "WHERE DO_Type = 24 " +
                   "GROUP BY fdl.AR_Ref, AR_Design";
        

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(inifilePath);

            if (File.Exists(inifilePath))
            {
                string[] lines = File.ReadAllLines(inifilePath);
            }
            else //création d'un modele du fichier INI
            {
                Console.WriteLine("Le fichier INI n'existe pas, création du modele...");
     

                IniData ini = new IniData();


                ini.Sections.AddSection("DatabaseSqlServer");
                ini["DatabaseSqlServer"].AddKey("ServerName", "xxxx");
                ini["DatabaseSqlServer"].AddKey("DbName", "xxxx");
                ini["DatabaseSqlServer"].AddKey("User", "");
                ini["DatabaseSqlServer"].AddKey("Password", "");

                ini.Sections.AddSection("FilePath");
                ini["FilePath"].AddKey("CsvFilePath", "xxxx");

                //création du fichier
                FileIniDataParser fileParser = new FileIniDataParser();
                fileParser.WriteFile(inifilePath, ini);
            }

            //Création d'un objet parser pour lire le fichier INI
            var parser = new FileIniDataParser();
            //vérifie si le fichier INI existe puis lecture du fichier
            if (!File.Exists(inifilePath))
            {
                throw new ArgumentException("le fichier INI n'existe pas");
            }

            IniData data = parser.ReadFile(inifilePath);
            Console.WriteLine("lecture du fichier ini...");

            string servername = data["DatabaseSqlServer"]["ServerName"];
            string DbName = data["DatabaseSqlServer"]["DbName"];
            string user = data["DatabaseSqlServer"]["User"];
            string pwd = data["DatabaseSqlServer"]["Password"];

            string outputPath = data["FilePath"]["CsvFilePath"];

            //partie console
            try
            {
                sqlManager = new SqlManager(servername, DbName, user, pwd);
               
                //if(sqlManager.TransformSqlToCsv(query, outputPath))
                //{
                //    Console.WriteLine();
                //    Console.WriteLine("Données exportées!");
                //    Console.WriteLine();
                //    Console.WriteLine("--------------------------------------------------------------------------------------");
                //    Console.WriteLine("Chemin du fichier -> " + outputPath);
                //    Console.WriteLine("--------------------------------------------------------------------------------------");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);              
            }

            //Console.WriteLine("Appuyez sur la touche ENTRER fermer le programme");
            //Console.ReadLine();

            /*********************************************************************************************************/
            //partie IHM
            //Window ihm = new Window(sqlManager, query); //      Window ihm = new Window(sqlManager.ExecuteQueryForDatatable(query), sqlManager);
            //ihm.ShowDialog();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window(sqlManager, query));


            Console.ReadLine();
        }
    }
}
