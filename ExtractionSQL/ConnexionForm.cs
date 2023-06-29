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
    public partial class ConnexionForm : Form
    {
        //public TextBox ServerName { get; set; }


        public ConnexionForm()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            // Fermez la fenêtre de connexion
            this.Close();
        }
    }
}
