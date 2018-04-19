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
            XmlFilePath = path;
        }
        catch(SystemException e)
        {
            Debug.Log(e);
            return false;
        }
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

    public XmlElement GetNodeByID(string idValue, string tag)
    {
        XmlNodeList list = GetNodeListByTag(tag);
        if (list.Count == 0)
            return null;
        foreach(XmlElement node in list)
        {
            if (node.GetAttribute("id") == idValue)
                return node;
        }
        return null;
    }

    string XmlFilePath = "";
}