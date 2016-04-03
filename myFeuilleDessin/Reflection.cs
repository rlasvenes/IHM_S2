using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myFeuilleDessin
{
    public partial class Reflection : Form
    {
        private Object currentObject;
        int x, y;
        public Reflection(Object ob)
        {
            // InitializeComponent();
            currentObject = ob;
            x = 25;
            y = 10;
            PropertyInfo[] properties = ob.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                if (p.SetMethod != null)
                {
                    Label lb = new Label();
                    lb.Text = p.Name;

                    if (p.PropertyType.Name.ToString() == "Point")
                        lb.Text = p.Name.ToString() + " " + "(X, Y)";

                    lb.Location = new Point(x, y);
                    Controls.Add(lb); // permet juste d'avoir les "titres" pour chaque attributs de notre objet

                    Control tb = new TextBox();

                    switch (p.PropertyType.Name)
                    {
                        case "Color":
                            tb = new Label();
                            tb.Height = 20;
                            tb.Tag = p;
                            tb.Text = "\t\t";
                            tb.BackColor = (Color)p.GetValue(ob);
                            tb.Click += label_couleur_Click;
                            break;

                        case "Boolean":
                            tb = new NumericUpDown();
                            tb.Width = 75;
                            ((NumericUpDown)tb).Maximum = 1; // soit false
                            ((NumericUpDown)tb).Minimum = 0; // soit true
                            // ((NumericUpDown)tb;
                            tb.Text = p.GetValue(ob).ToString();
                            break;

                        case "Point":
                            TextBox pos_X = new TextBox();
                            pos_X.Width = 100;
                            pos_X.Tag = p;
                            pos_X.Text = ""; // s'arranger pour ne get que la valeur du Point.X
                            pos_X.Location = new Point(x, y);
                            Controls.Add(pos_X);

                            TextBox pos_Y = new TextBox();
                            pos_Y.Width = 100;
                            pos_Y.Tag = p;
                            pos_Y.Text = ""; // idem, get que le Point.Y // p.GetValue(ob).ToString()
                            pos_Y.Location = new Point(x + 2*(tb.Width + 15), y); // ici je fais * 2 car on a deux TextBox sur une même ligne
                            Controls.Add(pos_Y);
                            break;

                        default :
                            break;
                    }
                    tb.Location = new Point(x + lb.Width + 25, y);
                    tb.Tag = p;
                    Controls.Add(tb);


                    if (p.GetValue(ob) != null)
                    {
                        
                        tb.Text = p.GetValue(ob).ToString();
                        y += 25;
                    }

                    if (p.PropertyType.Name.ToString() == "Color" || p.PropertyType.Name.ToString() == "Point")
                        tb.Text = "";
                }

            }

            foreach (string s in new string[] { "OK", "Annuler" })
            {
                Button bouton = new Button();
                bouton.Text = s;
                Controls.Add(bouton);
                bouton.Location = new Point(x, y);
                bouton.Click += ok_button_Click; // répond à un évènement
                if (Controls.Count < 2 ) // juste pour pouvoir alligne horizontalement les deux boutons OK et Annuler
                {
                    y += bouton.Height + 10; // on incrémente la "hauteur" que si on en est à créer le premier boutons
                }
                x += 2 * (bouton.ToString().Length) + 5;
                
            }
            
        } // fin constructeur

        private void ok_button_Click(Object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Text)
            {
                case "OK":

                    foreach (Control c in Controls)
                    {
                        if (c.Tag != null)
                        {
                            PropertyInfo p = (PropertyInfo)c.Tag;
                            
                            switch (p.PropertyType.Name)
                            {
                                case "String":
                                    //c.Text = p.GetValue(currentObject).ToString();
                                    p.SetValue(currentObject, c.Text);
                                    break;

                                case "Int32":
                                    int val;
                                    if (int.TryParse(c.Text, out val))
                                        p.SetValue(currentObject, val);
                                    break;

                                case "Color":
                                    p.SetValue(currentObject, c.BackColor);
                                    break;

								case "Point":
									Point pt;
									int x1, y1;
									int.TryParse (c.Text, out x1);
									int.TryParse (c.Text, out y1);
									
									p.SetValue(currentObject, x1);
                                    // p.SetValue(currentObject, new Point(x, y));
                                    break;

                                case "Boolean":
                                    int _bool;
                                    if (int.TryParse(c.Text, out _bool))
                                        p.SetValue(currentObject, Convert.ToBoolean(_bool)); // OMG it works !!!!!!!! #OMG recktC#
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    break;

                case "Annuler":
                    
                    break;

                default:
                    break;
            }

            this.Close();
        }

        private void label_couleur_Click(object sender, EventArgs e)
        {
            Label tb = (Label)sender;

            ColorDialog col2 = new ColorDialog { Color = BackColor };
            if (col2.ShowDialog() == DialogResult.OK)
            {
                tb.BackColor = col2.Color;
            }
        }

      }
}
