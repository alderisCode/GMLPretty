using DarkUI.Controls;
using DarkUI.Forms;
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
using System.Xml.Linq;
using System.Xml;

namespace GMLPretty
{
    public partial class Form1 : DarkForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //openFileDialog1.FileName;
                btnStart.Enabled = true;
            }
        }

        private void zamknijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
            { 
                statusLabel.Text = "Zapisuję do nowego pliku...";
                Application.DoEvents();
                MakeItPretty(openFileDialog1.FileName, saveFileDialog1.FileName);
            }
        }


        void MakeItPretty(string fileNameSrc, string fileNameDest)
        {
            int nodeLevel = 0;
            //GMLNode node = new GMLNode("");
            int parentID = -1;
            bool emptyNode = false;
            bool isFeatureMember = false;
            string line = "";
            string elStart = "";
            string elEnd = "";
            string elParams = "";
            string elValue = "";
            XmlReader xmlReader = XmlReader.Create(fileNameSrc);
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    // znacznik początkowy
                    case XmlNodeType.Element:
                        elStart = xmlReader.Name;

                        // poziom wcięcia...
                        nodeLevel++;

                        // atrybuty
                        for (int attInd = 0; attInd < xmlReader.AttributeCount; attInd++)
                        {
                            xmlReader.MoveToAttribute(attInd);
                            //Log(Lvl(nodeLevel) + "    - " + xmlReader.Name + " = " + xmlReader.Value + "\n", Color.Coral);
                            // -- DB --
                            //GMLAttribute attr = new GMLAttribute(node, xmlReader.Name, xmlReader.Value);
                            //node.Attributes.Add(attr);
                        }

                        break;

                    // Deklaracja
                    case XmlNodeType.XmlDeclaration:
                        line = xmlReader.Name;

                        break;

                    // tekst między znacznikami - value
                    case XmlNodeType.Text:
                        //Log(Lvl(nodeLevel) + "Text Node: " + xmlReader.Value + "\n");
                        elValue = xmlReader.Value;
                        break;

                    // znacznik końcowy
                    case XmlNodeType.EndElement:
                        //node = DB_GetNode(node.ParentID);
                        //parentID = node.ID;
                        nodeLevel--;
                        //Log(Lvl(nodeLevel) + "End Element " + xmlReader.Name + "\n" + Lvl(nodeLevel) + 
                        //    "Wracamy do <" + node.Name + ">" , Color.OrangeRed);
                        string t = Tabs(nodeLevel, 2);
                        line = String.Concat(t, "<", elStart, " ", elParams, ">", elValue);
                        break;

                    // pusta linia
                    case XmlNodeType.Whitespace:
                        //Log(Lvl(nodeLevel) + ".\n", Color.Gray);
                        break;

                    default:
                        //Log(Lvl(nodeLevel) + "Other node " + xmlReader.NodeType + " with value " + xmlReader.Value + "\n", Color.Cyan);
                        break;

                }

            }



        }


        private string Tabs(int tabCount, int tabSize)
        {
            string tab = "";
            string tabs = "";
            for (int i = 0; i < tabSize; i++) { tab += " "; }
            for (int i = 0; i < tabCount; i++) { tabs += tab; }
            return tabs;
        }

    }
}
