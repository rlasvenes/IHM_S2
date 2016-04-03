using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace myFeuilleDessin
{
    class Trait 
    {
        #region Attributs classe privés
        private Noeud source;
        private Noeud destination;
        #endregion

        #region Constructeur avec paramètres
        public Trait(Noeud sour, Noeud dest)
        {
            source = sour;
            destination = dest;
        }
        #endregion

        #region Constructeur 

        public Trait(XmlNode xNN2, List<Noeud> liste)
        {
            foreach (XmlNode xNNN2 in xNN2.ChildNodes)
            {
                switch (xNNN2.Name) {

                    case "SOURCE":
                        int s = int.Parse(xNNN2.InnerText);
                        source = liste.Find( x => x.ID == s);                        
                        break;

                    case "DESTINATION": 
                        int d = int.Parse(xNNN2.InnerText);
                        destination = liste.Find( x => x.ID == d);      
                        break;
                }
            }
        }

        #endregion

        #region Méthode Dessine(Graphics g)
        public void Dessine(Graphics g, Point pt, float zoom)
        {
            Point pSource = new Point((int) ( source.Position.X * zoom ) + pt.X, ((int) (source.Position.Y * zoom) + pt.Y)); // un point = coord.X + coord.Y (gg)
            Point pDestination = new Point((int) (destination.Position.X * zoom) + pt.X, ((int) (destination.Position.Y * zoom) + pt.Y));
            g.DrawLine(Pens.Blue, pSource, pDestination);
        }
        #endregion

        #region Méthode SauveTrait()

        public void SauveTrait(StreamWriter sw)
        {
            sw.WriteLine("<TRAIT>");

            sw.WriteLine("  <SOURCE>");
            sw.WriteLine(source.ID); // on récupère l'id avec notre propriété 
            sw.WriteLine("  </SOURCE>");

            sw.WriteLine("  <DESTINATION>");
            sw.WriteLine(destination.ID);
            sw.WriteLine("  </DESTINATION>");

            sw.WriteLine("</TRAIT>");
        }

        #endregion
    }
}
