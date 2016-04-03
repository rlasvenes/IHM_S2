using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myFeuilleDessin
{
    public partial class ParamètresNoeud : Form
    {
        #region Constructeur avec paramètres
        public ParamètresNoeud(Color c, int e)
        {
            InitializeComponent();
            label_épaisseur.Value = e;
            label_couleur.BackColor = c;
            
        }
        #endregion

        #region Propriété Couleur
        public Color Couleur { get { return label_couleur.BackColor; } }
        #endregion

        #region Propriété Epaisseur
        public int Epaisseur { get { return (int) label_épaisseur.Value; } }
        #endregion

        #region Propriété EtatCheckBox
        // public CheckState EtatCheckBox { get { return checkbox_supprimer.CheckState; } }
        #endregion

        #region label2_Click
        private void label2_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Label couleur
        private void label_couleur_Click(object sender, EventArgs e)
        {
            ColorDialog col2 = new ColorDialog { Color = Couleur };
            if (col2.ShowDialog() == DialogResult.OK)
            {
                label_couleur.BackColor = col2.Color;
            }
        }
        #endregion

    }
}
