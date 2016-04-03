using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Drawing.Printing;
using System.Reflection;

namespace myFeuilleDessin
{
    public partial class myFeuilleDessin : Form
    {
        List<Noeud> noeuds; // ma liste de noeuds (vector<Noeud> noeuds)
        List<Trait> traits; // Idem avec Trait

        Stack<Action> actions; // notre pile d'actions
        Stack<Action> actions_redo;

        PrintDocument Impression;

        #region Attributs classe privés
        private Noeud sélection;
        private bool enMouvement;
        private bool enDessinTrait;
        private Point pointCourant;
        private int épaisseurTrait;
        private Color couleurParDéfaut;

        private Point origine;

        private float zoom;

        #endregion

        #region Constructeur par défaut
        public myFeuilleDessin() // constructeur de la Windows Form
        {
            InitializeComponent();
            this.MouseWheel += myFeuilleDessin_MouseWheel;

            actions = new Stack<Action>(50); // On initialise la taille de la pile à 50
            actions_redo = new Stack<Action>(50); // On initialise la taille de la pile à 50

            noeuds = new List<Noeud>();
            traits = new List<Trait>();
            épaisseurTrait = 5;
            couleurParDéfaut = Color.Blue;

            origine = new Point(0, 0);

            zoom = 1;

            // déplacement.Image = ((Properties.Resources.draw)); // on set l'image dès le lancement du programme (par défaut)
        }
        #endregion

        #region Evenement myFeuilleDessin_Paint
        private void myFeuilleDessin_Paint(object sender, PaintEventArgs e)
        {
            
            foreach (Noeud n in noeuds)
                n.Dessine(e.Graphics, origine, zoom);

            foreach (Trait t in traits)
                t.Dessine(e.Graphics, origine, zoom);

            if (enDessinTrait)
            {
                e.Graphics.DrawLine(Pens.Red, model2ecran(sélection.Position), model2ecran(pointCourant));
            }
        }
        #endregion

        #region Méthode Sélection(Point p)
        private Noeud Sélection(Point p)
        {
            foreach (Noeud n in noeuds)
                if (n.Contient(p)) return n;
            return null;
        }
        #endregion

        #region Evenement myFeuilleDessin_MouseDown
        private void myFeuilleDessin_MouseDown(object sender, MouseEventArgs e)
        {
            #region Initialisation
            sélection = Sélection(ecran2model(e.Location));
            #endregion
             
            if (déplacement.Checked)
            {
                // déplacement.Image = (Properties.Resources.move) ;
                // Refresh();// quand on clique sur le bouton, mettre une image avec des touches directionnelles
                #region Déplacement
                enMouvement = sélection != null;
                #endregion

                // code pour le rectangle de sélection
                

            }
            else
            {
                
                #region Dessin
                if (e.Button == MouseButtons.Left) // BOOLEEN UNDO REDO afficher que les noeuds qui ont un booléen à faux
                {
                    déplacement.Image = (Properties.Resources.draw);

                    if (sélection == null)
                    { // si on clique là où il n'y a rien
                        
                        Noeud n = new Noeud(ecran2model(e.Location), new Size(20, 20), épaisseurTrait, couleurParDéfaut);
                        noeuds.Add(n);

                        Action action = new Action(Type_Action.Créer, new List<ISupprimable>() { n });
                        actions.Push(action);
                        
                    }   
                    else
                    {
                        enDessinTrait = true;
                        pointCourant = ecran2model(e.Location);
                    }
                }
                else
                {
                    if (sélection != null)
                    {
                        sélection.Modifier(); // on appel la méthode modfier qui permet de générer des boites de dialogues génériques
                    }
                }
                #endregion
            }
            Refresh();
        }
        #endregion

        #region Evenement myFeuilleDessin_MouseMove
        private void myFeuilleDessin_MouseMove(object sender, MouseEventArgs e)
        {
            if (enMouvement)
            {
                sélection.Déplace(ecran2model(e.Location));
                
                Refresh();
            }
            if (enDessinTrait)
            {
                pointCourant = ecran2model(e.Location);
            }
            Refresh();
        }
        #endregion

        #region Evenement myFeuilleDessin_MouseUp
        private void myFeuilleDessin_MouseUp(object sender, MouseEventArgs e)
        {
            enMouvement = false;
            if (enDessinTrait)
            {
                Noeud fin = Sélection(ecran2model(e.Location));
                if (fin != null)
                {
                    traits.Add(new Trait(sélection, fin));
                }
                else    // si je suis en train de dessiner un trait, MAIS que je relache le clique sur aucun autre noeud
                {
                    fin = new Noeud(pointCourant, new Size(20, 20), épaisseurTrait, couleurParDéfaut);
                    noeuds.Add(fin); // je l'ajoute dans la liste
                    traits.Add(new Trait(sélection, fin)); // et on ajoute le trait correspondant
                }

                enDessinTrait = false;
                Refresh();
            }
        }
        #endregion

        #region Evenement réduire_Click
        private void réduire_Click(object sender, EventArgs e)
        {
            épaisseurTrait--;   
        }
        #endregion

        #region Evenement agrandir_Click
        private void agrandir_Click(object sender, EventArgs e)
        {
            épaisseurTrait++;   
        }
        #endregion

        #region Evenement bouton épaisseur par défaut
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            épaisseurTrait = 5;
        }
        #endregion

        #region Evenement Ma_Couleur_Click - BOUTON COULEUR
        private void Ma_couleur_Click(object sender, EventArgs e)
        {
            ColorDialog col = new ColorDialog { Color = couleurParDéfaut };
            if (col.ShowDialog() == DialogResult.OK)
            {
                couleurParDéfaut = col.Color;
            }
        }
        #endregion

        #region BOUTON SUPPRIMER
        private void bouton_supprimer_tout_Click(object sender, EventArgs e)
        {
            traits.Clear();
            noeuds.Clear(); 
            Refresh();
        }
        #endregion

        #region Evenement ENREGISTRER
        private void enregistrerToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog
            {
                Filter = "Fichier xml|*.xml",
                InitialDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (svd.ShowDialog() == DialogResult.OK)
            {
                // code de sauvegarde
                StreamWriter sw = new StreamWriter(svd.FileName);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                sw.WriteLine("<DESSIN>");
                noeuds.ForEach(c => c.SauveNoeud(sw));
                traits.ForEach(c => c.SauveTrait(sw));
                sw.WriteLine("</DESSIN>");
                sw.Close();
            }
        }

        #endregion
        
        #region Evenement OUVRIR
        private void ouvrirToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog opfd = new OpenFileDialog
            {
                Filter = "Fichier xml|*.xml",
                Title = "Choisir le fichier",
                InitialDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (opfd.ShowDialog() == DialogResult.OK)
            {
                // code de relecture
                XmlDocument doc = new XmlDocument();
                doc.Load(opfd.FileName);
                foreach (XmlNode xN in doc.ChildNodes)
                {
                    if (xN.Name == "DESSIN")
                    {
                        foreach (XmlNode xNN in xN.ChildNodes)
                        {
                            if (xNN.Name == "NOEUD")
                            {
                                Noeud relecture = new Noeud(xNN); // que faire ?
                                noeuds.Add(relecture);
                            }

                            if (xNN.Name == "TRAIT")
                            {
                                Trait relecture_trait = new Trait(xNN, noeuds);
                                traits.Add(relecture_trait);
                            }
                        }
                    }
                }
                Refresh();
                
            }
        }

        #endregion

        private void annulerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actions.Count > 0)
            {
                Action a = actions.Pop();
                a.Undo();
                actions_redo.Push(a);
                Refresh();
            }
        }

        private void rétablirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actions_redo.Count > 0)
            {
                Action a = actions_redo.Pop();
                a.Redo();
                actions.Push(a);
                Refresh();
            }            
        }

        private void aperçuavantimpressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog ptPRev = new PrintPreviewDialog();
            Impression = new PrintDocument();
            //Impression.PrintPage += Impression_PrintPage;
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            vScrollBar.Value = e.NewValue;
            origine.Y = -vScrollBar.Value;
            Refresh();
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            hScrollBar.Value = e.NewValue;
            origine.X = -hScrollBar.Value;
            Refresh();
        }

        private Point ecran2model(Point p)
        {
            Point res = new Point((int) ((p.X - origine.X)/zoom), (int) ((p.Y - origine.Y)/zoom));
            return res;
        }
         
        private Point model2ecran(Point p)
        {
            Point res = new Point( (int) (p.X * zoom) + origine.X, (int) (p.Y * zoom) + origine.Y);
            return res;
        }

        void myFeuilleDessin_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) 
            {
                if (e.Delta > 0)
                {
                    zoom *= 1.1f;
                    if (zoom > 15f)
                        zoom = 15f;
                }
                else if (e.Delta < 0)
                {
                    zoom /= 1.1f;
                    if (zoom < 1f)
                        zoom = 1f;
                }

                Refresh();
            }

            /*if ((Control.ModifierKeys & Keys.Control) == Keys.Control) // a voir ==> pour le scroll horizontale
            {
                vScrollBar.Value = e.Delta;
            }*/
        }

    }
}

// sur quel objet je suis en train d'attraper (? what )
// marche sur une hiérarchie d'héritage
// pendant l'éxécution, on appel une fonction sur le haut de la hiérarchie
