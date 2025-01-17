using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GMLPretty
{
    internal class XmlLine
    {
        private string startNode = "";
        private string endNode = "";
        private int paramCount = 0;
        private List<XmlParameter> paramList = new List<XmlParameter>();
        private string value = "";
        private int level = 0;
        private int tabSize = 2;
        private bool isDeclaration = false;
        private bool isEmpty = false;

        public XmlLine(int tabSize) 
        { 
            this.tabSize = tabSize;
        }

        public void Clear() 
        {
            startNode = "";
            endNode = "";
            paramCount = 0;
            paramList.Clear();
            value = "";
            level = 0;
            isDeclaration = false;
            isEmpty = false;
        }

        public void SetStartNode(string node)
        {
            this.startNode = node;
        }

        public void SetEndNode(string node) 
        { 
            this.endNode = node; 
        }

        public void SetValue(string value) 
        {  
            this.value = value; 
        }

        public void SetLevel(int level) 
        { 
            this.level = level; 
        }

        public void SetTabSize(int tabSize) 
        { 
            this.tabSize = tabSize; 
        }

        public void AddParameter(string name, string value) 
        { 
            this.paramList.Add(new XmlParameter(name, value));
            this.paramCount++;
        }

        public void SetDeclaration() 
        { 
            this.isDeclaration = true; 
        } 

        public void SetEmpty()
        {
            this.isEmpty = true;
        }

        public string GetXmlLine()
        {
            string line = "";
            if (isDeclaration) 
            {
                line = String.Concat("<?", startNode);
            }
            else if (startNode != "") 
            { 
                line = String.Concat(Tabs() + "<", startNode); 
            }
            if (paramCount > 0)
            {
                foreach (XmlParameter p in paramList)
                {
                    line = String.Concat(line, " ", p.Name, "=\"", p.Value, "\"");
                }
            }
            if (isDeclaration)
            {
                line = String.Concat(line, "?>");
                return line;
            }
            if (!isEmpty) 
            { 
                line = String.Concat(line, ">", value);
            }
            else
            {
                line = String.Concat(line, " />", value);
            }
            if (endNode != "")
            {
                line = String.Concat(line, "<", endNode, ">");
            }
            
            return line;
        }


        private string Tabs()
        {
            string tab = "";
            string tabs = "";
            for (int i = 0; i < tabSize; i++) { tab += " "; }
            for (int i = 0; i < level; i++) { tabs += tab; }
            return tabs;
        }



        // --------------- PARAM --------------------------

        class XmlParameter 
        { 
            public string Name { get; set; }
            public string Value { get; set; }


            public XmlParameter(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
        }
    }
}
