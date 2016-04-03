using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Linq;

namespace myFeuilleDessin
{
    public class Noeud : ISupprimable
    {
        #region Attributs classe privés
        private Point myPosition;
        private Size myTaille;
        private int myEpaisseur;
        private Color m_couleur;

        private static int m_id = 0; // y'a qu'ici qu'on peut le faire car "static"
        private int idCounter;


        public int ID
        {
            get { return idCounter; }
        }

        #endregion

        #region Constructeur avec paramètres
        public Noeud(Point p, Size s, int épaisseur, Color couleur) // constructeur
        {
            myTaille = s;
            Position = p; // correspond à la propriété Position ( <=> à un get dans ce cas là)
            myEpaisseur = épaisseur;
            m_couleur = couleur;

            m_id++;
            idCounter = m_id;
        }
        #endregion

        #region Constructeur Noeud(XmlNode)
        public Noeud(XmlNode xNN)
        {

            foreach (XmlNode xNNN in xNN.ChildNodes)
            {
                switch (xNNN.Name)
                {
                    case "POSITION":
                        string[] data = xNNN.InnerText.Split(',');
                        int x = int.Parse(data[0].Split('=')[1]); // on stockerais le X
                        int y = int.Parse(data[1].Replace("}", "").Split('=')[1]); // idem avec Y
                        Position = new Point(x, y); // et on en fait un Point
                        break;

                    case "COULEUR":
                        int c = int.Parse(xNNN.InnerText);
                        Couleur = Color.FromArgb(c);
                        break;

                    case "TAILLE":
                        string[] data2 = xNNN.InnerText.Split(',');
                        int w = int.Parse(data2[0].Split('=')[1]);
                        int h = int.Parse(data2[1].Replace("}", "").Split('=')[1]);
                        myTaille = new Size(w, h);
                        break;

                    case "EPAISSEUR":
                        int ep = int.Parse(xNNN.InnerText);
                        Epaisseur = ep;
                        break;

                    case "ID":
                        int i = int.Parse(xNNN.InnerText);
                        idCounter = i;

                        if (idCounter > m_id) // si il est strictement supérieur, il est au moins à +1 donc inutile de refaire +1
                        {
                            m_id = idCounter; // +1; // + 2 car sinon ça met deux fois le même
                        }
                        break;
                }
            }

            //if ()
        }

        #endregion

        #region Propriété épaisseur
        public int Epaisseur // propriété Epaisseur pour pouvoir "get" la couleur actuelle du noeud
        {
            get { return myEpaisseur; }
            set { myEpaisseur = value; } // pour prendre en compte les modifications de l'user
        }
        #endregion

        #region Propriété Couleur
        public Color Couleur
        {
            get { return m_couleur; }
            set { m_couleur = value; } // pour prendre en compte les 
        }
        #endregion

        #region Propriété Position
        public Point Position // propriété qui retourne le centre de l'objet à dessiner
        {
            get { return new Point(myPosition.X + myTaille.Width / 2, myPosition.Y + myTaille.Height / 2); }
            set { myPosition = new Point(value.X - myTaille.Width /2, value.Y - myTaille.Height/2) ; }
        }
        #endregion

        #region Méthode Dessine(Graphics g)
        public void Dessine(Graphics g, Point pt, float zoom) // méthode qui va déssiner un rectangle (ou cercle) 
                                                              // pt = origine dans l'appel de fonction
        {
            if (!Supprimé)
            {
                Point pCourant = new Point(myPosition.X + pt.X, myPosition.Y + pt.Y);
                Rectangle r = new Rectangle(pCourant, myTaille);

                Rectangle rect = new Rectangle((int) (r.X * zoom) + pt.X,
                                               (int) (r.Y * zoom) + pt.Y, 
                                               (int) (r.Width * zoom),
                                               (int) (r.Height * zoom));

                Pen pen = new Pen(m_couleur, myEpaisseur);
                g.DrawEllipse(pen, rect);
            }
        }
        #endregion

        #region Méthode Contient(Point p)
        public bool Contient(Point p) // pour savoir si la souris est au dessus d'un objet géométrique si un point est contenu dans un rectangle
        {
            Rectangle r = new Rectangle(myPosition, myTaille);
            return r.Contains(p);
        }
        #endregion

        #region Méthode Déplace(Point p)
        public void Déplace(Point p) // pour déplacer les noeuds en question
        {
            Position = p;
        }
        #endregion

        #region Méthode Modifier()
        public void Modifier()  // méthode au lieu de le gérer comme un évènement
        {
            Reflection refl = new Reflection(this);
            refl.Width = 500; // permet juste d'agrandir un peu la fenêtre pour pouvoir tester des choses

            if (refl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // refl.Close();
            }
            /*ParamètresNoeud param = new ParamètresNoeud(m_couleur, myEpaisseur);
            if (param.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                myEpaisseur = param.Epaisseur;
                m_couleur = param.Couleur;
            }*/
             
        }
        #endregion

        #region Méthode Sauve()

        public void SauveNoeud(StreamWriter sw) // SauveXML
        {
            String texte = "<NOEUD>";
            texte += "  <POSITION>";
            texte += "  " + Position.ToString();
            texte += "  </POSITION>";
            texte += "  <COULEUR>";
            texte += "  " + Couleur.ToArgb().ToString();
            texte += "  </COULEUR>";
            texte += "  <TAILLE>";
            texte += "  " + myTaille.ToString(); 
            texte += "  </TAILLE>";
            texte += "  <EPAISSEUR>";
            texte += "  " + myEpaisseur.ToString();
            texte += "  </EPAISSEUR>";
            texte += "  <ID>";
            texte += "  " + ID.ToString();
            texte += "  </ID>";
            texte += "</NOEUD>";

            sw.WriteLine(texte);
        }

        #endregion

        #region méthode degresNoeud(Noeud n)

       /* public int degrèsNoeud(Noeud n)
        {
            // à faire
        }*/

        #endregion

        public bool Supprimé { get; private set; }
        public void Supprime() { Supprimé = true; }
        public void Restaure() { Supprimé = false; }
    
    }
}