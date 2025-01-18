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
using System.IO;
using System.Security.Cryptography;

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
                statusLabel.Text = "Zapis zakończony.";
            }
        }


        void MakeItPretty(string fileNameSrc, string fileNameDest)
        {
            XmlLine xmlLine = new XmlLine(2);
            int nodeLevel = 0;
            bool emptyNode = false;
            string line = "";
            LastOperation lastOp = LastOperation.None;
            using (var writer = new StreamWriter(fileNameDest))
            {
                XmlReader xmlReader = XmlReader.Create(fileNameSrc);
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        // znacznik początkowy
                        case XmlNodeType.Element:
                            if (lastOp != LastOperation.None)
                            {
                                // Zapis do pliku
                                line = xmlLine.GetXmlLine();
                                if (line != "")
                                {
                                    writer.WriteLine(line);
                                    //Log(line + "\n", Color.White);
                                    xmlLine.Clear();
                                }
                            }
                            // poziom wcięcia...
                            if (lastOp != LastOperation.EmptyNode && 
                                lastOp != LastOperation.Declaration) 
                                nodeLevel++;
                            if (xmlReader.IsEmptyElement)
                            {
                                xmlLine.SetEmpty();
                                emptyNode = true;
                            }
                            else 
                            {
                                emptyNode = false; 
                            }
                            xmlLine.SetStartNode(xmlReader.Name);
                            xmlLine.SetLevel(nodeLevel);
                            // atrybuty
                            for (int attInd = 0; attInd < xmlReader.AttributeCount; attInd++)
                            {
                                xmlReader.MoveToAttribute(attInd);
                                xmlLine.AddParameter(xmlReader.Name, xmlReader.Value);
                            }
                            if (emptyNode)
                                lastOp = LastOperation.EmptyNode;
                            else
                                lastOp = LastOperation.StartNode;
                            break;

                        // Deklaracja
                        case XmlNodeType.XmlDeclaration:
                            xmlLine.SetStartNode(xmlReader.Name);
                            xmlLine.SetDeclaration();
                            // atrybuty deklaracji
                            for (int attInd = 0; attInd < xmlReader.AttributeCount; attInd++)
                            {
                                xmlReader.MoveToAttribute(attInd);
                                xmlLine.AddParameter(xmlReader.Name, xmlReader.Value);
                            }
                            // ZAPIS LINII
                            line = xmlLine.GetXmlLine();
                            writer.WriteLine(line);
                            //Log(line + "\n", Color.White);
                            xmlLine.Clear();
                            lastOp = LastOperation.Declaration;
                            break;

                        // tekst między znacznikami - value
                        case XmlNodeType.Text:
                            xmlLine.SetValue(xmlReader.Value);
                            lastOp = LastOperation.Value;
                            break;

                        // znacznik końcowy
                        case XmlNodeType.EndElement:
                            if (lastOp==LastOperation.EmptyNode)
                            {
                                line = xmlLine.GetXmlLine();
                                writer.WriteLine(line);
                                xmlLine.Clear();
                                nodeLevel--;
                            }
                            xmlLine.SetEndNode(xmlReader.Name);
                            xmlLine.SetLevel(nodeLevel);
                            line = xmlLine.GetXmlLine();
                            writer.WriteLine(line);
                            //Log(line + "\n", Color.White);
                            xmlLine.Clear();
                            nodeLevel--;
                            lastOp = LastOperation.EndNode;
                            break;

                        // pusta linia
                        case XmlNodeType.Whitespace:
                            //Pomijamy puste linie
                            //lastOp = LastOperation.WhiteSpace;
                            break;

                        default:
                            Log(Tabs(nodeLevel, 2) + "Other node " + xmlReader.NodeType + " with value " + xmlReader.Value + "\n", Color.Cyan);
                            break;

                    }

                }
                writer.Close();
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
