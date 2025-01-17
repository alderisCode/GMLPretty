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
            XmlLine xmlLine = new XmlLine(2);
            int nodeLevel = 0;
            int parentID = -1;
            bool emptyNode = false;
            bool isFeatureMember = false;
            string line = "";
            string elStart = "";
            string elEnd = "";
            string elParams = "";
            string elValue = "";
            LastOperation lastOp = LastOperation.None;
            XmlReader xmlReader = XmlReader.Create(fileNameSrc);
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    // znacznik początkowy
                    case XmlNodeType.Element:
                        if (lastOp != LastOperation.None) 
                        {
                            Log(xmlLine.GetXmlLine() + "\n", Color.White);
                            xmlLine.Clear();
                            // Zapis do pliku
                        }
                        if (xmlReader.IsEmptyElement) 
                        { 
                            xmlLine.SetEmpty();
                            Log(Tabs(nodeLevel, 2) + xmlReader.Name + " <-- EMPTY\n", Color.IndianRed);
                        }
                        elStart = xmlReader.Name;
                        xmlLine.SetStartNode(xmlReader.Name);
                        xmlLine.SetLevel(nodeLevel);
                        lastOp = LastOperation.StartNode;
                        // poziom wcięcia...
                        nodeLevel++;
                        Log(Tabs(nodeLevel, 2) + xmlReader.Name + "\n", Color.IndianRed);
                        // atrybuty
                        for (int attInd = 0; attInd < xmlReader.AttributeCount; attInd++)
                        {
                            xmlReader.MoveToAttribute(attInd);
                            xmlLine.AddParameter(xmlReader.Name, xmlReader.Value);
                            Log(Tabs(nodeLevel, 2) + "    - " + xmlReader.Name + " = " + xmlReader.Value + "\n", Color.Coral);
                        }

                        break;

                    // Deklaracja
                    case XmlNodeType.XmlDeclaration:
                        xmlLine.SetStartNode(xmlReader.Name);
                        xmlLine.SetDeclaration();
                        Log(Tabs(nodeLevel, 2) + "Deklaracja: " + xmlReader.Name + " " + xmlReader.Value + "\n");
                        // atrybuty deklaracji
                        for (int attInd = 0; attInd < xmlReader.AttributeCount; attInd++)
                        {
                            xmlReader.MoveToAttribute(attInd);
                            xmlLine.AddParameter(xmlReader.Name, xmlReader.Value);
                            Log(Tabs(nodeLevel, 2) + "    - " + xmlReader.Name + " = " + xmlReader.Value + "\n", Color.Coral);
                        }
                        // ZAPIS LINII
                        line = xmlLine.GetXmlLine();
                        Log(line + "\n", Color.White);
                        xmlLine.Clear();

                        break;

                    // tekst między znacznikami - value
                    case XmlNodeType.Text:
                        Log(Tabs(nodeLevel, 2) + "Text Node: " + xmlReader.Value + "\n");
                        elValue = xmlReader.Value;
                        break;

                    // znacznik końcowy
                    case XmlNodeType.EndElement:
                        nodeLevel--;
                        Log(Tabs(nodeLevel, 2) + "End Element " + xmlReader.Name + "\n" + Tabs(nodeLevel, 2) + 
                            "Wracamy do LVL<" + nodeLevel.ToString() + ">\n" , Color.OrangeRed);
                        string t = Tabs(nodeLevel, 2);
                        line = String.Concat(t, "<", elStart, " ", elParams, ">", elValue);

                        Log(xmlLine.GetXmlLine() + "\n", Color.White);
                        xmlLine.Clear();
                        break;

                    // pusta linia
                    case XmlNodeType.Whitespace:
                        //Pomijamy puste linie
                        //Log(Tabs(nodeLevel, 2) + ".\n", Color.Gray);
                        break;

                    default:
                        Log(Tabs(nodeLevel, 2) + "Other node " + xmlReader.NodeType + " with value " + xmlReader.Value + "\n", Color.Cyan);
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

        void Log(string txt)
        {
            richTextBox1.AppendText(txt);
            richTextBox1.ScrollToCaret();
            Application.DoEvents();
        }

        void Log(string txt, Color color)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = color;
            richTextBox1.AppendText(txt);
            richTextBox1.ScrollToCaret();
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
            Application.DoEvents();
        }


    }
}
