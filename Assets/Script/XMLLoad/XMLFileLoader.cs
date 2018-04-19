using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMLFileLoader {
    public XMLFileLoader()
    {
        init();
    }

    void init()
    {
        XmlTable = new Hashtable();

    }

    public XmlFile File(string path)
    {
        path = XMLPath + path;
        if (XmlTable.ContainsKey(path))
        {
            XmlFile result = XmlTable[path] as XmlFile;
            if (result != null)
                return result;
        }
        XmlFile tmp = new XmlFile();
        if(tmp.OpenXmlFile(path))
            XmlTable.Add(path, tmp);
        else
            XmlTable.Add(path, null);

        return XmlTable[path] as XmlFile;
    }
    static string XMLPath = "XML/";

    Hashtable XmlTable;

    static XMLFileLoader loader = null;
    public static XMLFileLoader Loader
    {
        get
        {
            if (loader == null)
                loader = new XMLFileLoader();
            return loader;
        }
    }

    public void CreateXml()
    {
        XmlDocument xmlDoc = new XmlDocument();

        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "CharacterInfo", string.Empty);
        xmlDoc.AppendChild(root);

        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Character", string.Empty);
        root.AppendChild(child);

        // 자식 노드에 들어갈 속성 생성
        XmlElement name = xmlDoc.CreateElement("Name");
        name.InnerText = "wergia";
        child.AppendChild(name);

        XmlElement lv = xmlDoc.CreateElement("Level");
        lv.InnerText = "1";
        child.AppendChild(lv);

        XmlElement exp = xmlDoc.CreateElement("Experience");
        exp.InnerText = "45";
        exp.SetAttribute("id", "needExp");
        child.AppendChild(exp);

        xmlDoc.Save(XMLPath+"tmp.xml");
    }
}
