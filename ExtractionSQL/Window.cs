using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractionSQL
{
    public partial class Window : Form
    {
        SqlManager sqlManager;
        string query;

        public Window(SqlManager sqlManager, string query)
        {
            Console.WriteLine("lancement de l'IHM...");
            InitializeComponent();
            this.sqlManager = sqlManager;
            this.query = query;
        }

        private void Window_Load_1(object sender, EventArgs e)
        {
            dataGridView1.DataSource = sqlManager.ExecuteQueryForDatatable(query);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //ancrage  du grid
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            //angrage du bouton 
            button1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            //ancrage du lien
            linkLabel1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom ;
            //ancrage datePicker
            dateTimePicker1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            //label
            label1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            //bouton de validation date
            button2.Anchor =  AnchorStyles.Right | AnchorStyles.Bottom;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Afficher la boîte de dialogue de sauvegarde de fichier
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Fichier CSV (*.csv)|*.csv";
            saveFileDialog.Title = "Enregistrer le fichier CSV";
            saveFileDialog.ShowDialog();

            //Vérifie si l'utilisateur a choisi un emplacement de fichier valide
            if (saveFileDialog.FileName != "")
            {
                string filePath = saveFileDialog.FileName;

                // Exporter les données vers le fichier CSV
                //sqlManager.TransformSqlToCsv(query, filePath);  //export la requete 
                ExportToCsv(filePath);  // export gridPane

               // MessageBox.Show("Fichier CSV exporté avec succès !");
            }

        }

        private void ExportToCsv(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            //En-têtes de colonnes
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                sb.Append(column.HeaderText);
                sb.Append(",");
            }
            sb.AppendLine();

            //Contenu des lignes
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    sb.Append(cell.Value?.ToString().Replace(",", "") ?? "");
                    sb.Append(",");
                }
                sb.AppendLine();
            }

            //Écrire le contenu dans le fichier CSV
            File.WriteAllText(filePath, sb.ToString());

            MessageBox.Show("Fichier CSV exporté avec succès !");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Créez une instance de la fenêtre de connexion à la base de données
            ConnexionForm connexionForm = new ConnexionForm();

            //Affichee la fenetre de connexion
            DialogResult result = connexionForm.ShowDialog();

            // est déclenché quand l'user appuie sur Ok depuis la fenetre de connexion
            if (result == DialogResult.OK)
            {
                try
                {
                    //récupére les infos depuis le panel de connexion
                    string serverName = connexionForm.ServerName.Text;
                    string databaseName = connexionForm.DatabaseName.Text;
                    string username = connexionForm.Username.Text;
                    string password = connexionForm.Password.Text;

                    this.sqlManager = new SqlManager(serverName, databaseName, username, password);
                    UpdateGridData(sqlManager);

                    MessageBox.Show($"Connexion à la base de données : {serverName}/{databaseName}");
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Impossible de se connecter à la base : {ex}");
                }
               
            }
        }

        public void UpdateGridData(SqlManager sql)
        {  
            dataGridView1.DataSource = sql.ExecuteQueryForDatatable(query); ;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //date selectionné
            DateTime selectedDate = dateTimePicker1.Value;
            // Formater la date dans le format SQL attendu
            string formattedDate = selectedDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

            query = "SELECT fdl.AR_Ref AS Ref_Article, AR_Design AS Designation, fas.AS_QteSto AS StockReel, SUM(Dl_Qte) AS Besoin_cumule  FROM F_DOCLIGNE fdl  " +
                   " INNER join F_ARTICLE fa ON fdl.AR_Ref = fa.AR_Ref  " +
                   " inner join F_ARTSTOCK fas ON fas.AR_Ref = fa.AR_Ref  " +
                   "WHERE DO_Type = 24 AND fdl.DO_Date < '" + formattedDate +
                   "' GROUP BY fdl.AR_Ref, AR_Design, fas.AS_QteSto";
            UpdateGridData(sqlManager);

            MessageBox.Show($"Actualisation des données réussi");
        }
    }
}
