using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            //this.Resize += Form1_Resize;
            this.sqlManager = sqlManager;
            this.query = query;
        }

        private void Window_Load_1(object sender, EventArgs e)
        {
            dataGridView1.DataSource = sqlManager.ExecuteQueryForDatatable(query);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
           // dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //dataGridView1.AutoResizeRows();

            //ancrage  du grid
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            //angrage du bouton 
            button1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            //button1.Dock = DockStyle.None;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Afficher la boîte de dialogue de sauvegarde de fichier
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Fichier CSV (*.csv)|*.csv";
            saveFileDialog.Title = "Enregistrer le fichier CSV";
            saveFileDialog.ShowDialog();

            // Vérifier si l'utilisateur a choisi un emplacement de fichier valide
            if (saveFileDialog.FileName != "")
            {
                string filePath = saveFileDialog.FileName;

                // Exporter les données vers le fichier CSV
                sqlManager.TransformSqlToCsv(query, filePath);

                // Afficher un message de succès ou effectuer d'autres actions nécessaires
                MessageBox.Show("Fichier CSV exporté avec succès !");
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
