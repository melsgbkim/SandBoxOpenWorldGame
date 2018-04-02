using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class XmlFile
{
    XmlDocument xmlDoc;
    public XmlFile()
    {
        xmlDoc = new XmlDocument();
    }

    public bool OpenXmlFile(string path)
    {
        try
        {
            TextAsset textAsset = (TextAsset)Resources.Load(path);
            Debug.Log(textAsset);
            xmlDoc.LoadXml(textAsset.text);
        }
        catch(SystemException e)
        {
            Debug.Log(e);
            return false;
        }
        XmlFilePath = path;
        XmlElement tmp = GetNodeByID("needExp", "Experience");
        Debug.Log(tmp);

        return true;
    }

    public XmlNodeList GetNodeListByTag(string tag)
    {
        return xmlDoc.GetElementsByTagName(tag);
    }

    public int GetNodeByTagWithCount(string tag,out XmlNode value)
    {
        XmlNodeList list = GetNodeListByTag(tag);
        value = null;
        if (list.Count > 0)
            value = list.Item(0);
        return list.Count;
    }

    public XmlElement GetNodeByID(string id,string tag)
    {
        XmlNodeList list = GetNodeListByTag(tag);
        if (list.Count == 0)
            return null;
        foreach(XmlElement node in list)
        {
            if (node.GetAttribute("id") == id)
                return node;
        }
        return null;
    }

    string XmlFilePath = "";
}