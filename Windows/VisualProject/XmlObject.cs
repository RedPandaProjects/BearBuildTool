using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject
{
    class XmlObject
    {
        public virtual string GetName()
        {
            return String.Empty;
        }
        public virtual XmlObject Create()
        {
            return null;
        }
        public static void Save<T>(ref List<T> objects, ref XmlDocument xmlDocument) where T: XmlObject
        {
            foreach (var obj in objects)
            {
                obj.Save(ref xmlDocument);
            }
        }
        public void Save(ref XmlDocument xmlDocument)
        {
            var Element = xmlDocument.CreateElement(GetName());
            var Node = Element as XmlNode;
            SaveImpl(ref xmlDocument, ref Node);
            xmlDocument.AppendChild(Element);

        }
        public static void Save<T>(ref List<T> objects, ref XmlDocument xmlDocument, ref XmlNode parent) where T : XmlObject
        {
            foreach (var obj in objects)
            {
                obj.Save(ref xmlDocument, ref parent);
            }
        }
        public void Save( ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            var Element = xmlDocument.CreateElement(GetName());
            var Node = Element as XmlNode;
            SaveImpl(ref xmlDocument, ref Node);
            parent.AppendChild(Element);

        }
        public bool Load(ref XmlDocument xmlDocument)
        {
            foreach (XmlNode child in xmlDocument.ChildNodes)
            {
                if(child.Name==GetName())
                {
                    XmlNode xmlElement = child;
                    return LoadImpl(ref xmlElement);
                }
            }
            return false;
        }
        public bool Load( ref XmlNode parent)
        {

            foreach (XmlNode child in parent.ChildNodes)
            {
                if (child.Name == GetName())
                {
                    XmlNode xmlElement = child;
                    return LoadImpl(ref xmlElement);
                }
            }
            return false;
        }
        public  List<T> LoadList<T>(ref XmlDocument xmlDocument) where T: XmlObject
        {
            List<T> list = new List<T>();
            foreach (XmlNode child in xmlDocument.ChildNodes)
            {
                if (child.Name ==  GetName())
                {
                    XmlObject obj = Create();
                    XmlNode xmlElement = child;
                    if (obj.LoadImpl(ref xmlElement))
                    list.Add(obj as T);
                }
            }
            return list;
        }
        public  List<T> LoadList<T>(ref XmlNode parent) where T : XmlObject, new()
        {
            T t = new T();

            List<T> list = new List<T>();
            foreach (XmlNode child in parent.ChildNodes)
            {
                if (child.Name == t.GetName())
                {
                    XmlObject obj = t.Create();
                    XmlNode xmlElement = child;
                    if(obj.LoadImpl(ref xmlElement))
                    list.Add(obj as T);
                }
            }
            return list;
        }
        virtual protected void SaveImpl(ref XmlDocument xmlDocument,ref XmlNode parent)
        {
           

        }
        virtual protected bool LoadImpl(ref XmlNode parent)
        {
            return false;
        }
        public bool HasAttribute(ref XmlNode node, string name)
        {
            foreach (XmlAttribute xmlAttribute in node.Attributes)
            {
                if (xmlAttribute.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasNode(ref XmlNode node, string name)
        {
            foreach (XmlNode xmlNode in node.ChildNodes)
            {
                if (xmlNode.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        public static XmlAttribute GetAttribute(ref XmlNode node,string name)
        {
            foreach(XmlAttribute xmlAttribute in node.Attributes)
            {
                if(xmlAttribute.Name==name)
                {
                    return xmlAttribute;
                }
            }
            return null;
        }
        public static XmlNode GetNode(ref XmlNode node, string name)
        {
            foreach (XmlNode xmlNode in node.ChildNodes)
            {
                if (xmlNode.Name == name)
                {
                    return xmlNode;
                }
            }
            return null;
        }
        public static void AppendAttribute(ref XmlDocument xmlDocument, ref XmlNode node, string name, string value)
        {
            var obj = xmlDocument.CreateAttribute(name);
            obj.Value = value;
            node.Attributes.Append(obj);
        }
        public static void AppendNode(ref XmlDocument xmlDocument, ref XmlNode node, string name, string value)
        {
            var obj = xmlDocument.CreateElement(name);
            obj.InnerText = value;
            node.AppendChild(obj);
        }

        public void Save(string file)
        {
            XmlDocument xmlDocument = new XmlDocument();
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.AppendChild(xmlDeclaration);
            Save(ref xmlDocument);
            xmlDocument.Save(file);
        }
        public bool Load(string file)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(file);
           
            return Load(ref xmlDocument);
        }
    }
}
