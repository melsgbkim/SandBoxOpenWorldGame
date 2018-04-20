using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class XMLUtil
{
    public static XmlElement FindOneByTag(XmlElement element, string tag)
    {
        XmlNodeList list = element.GetElementsByTagName(tag);
        foreach (XmlElement node in list)
            return node;
        return null;
    }

    public static XmlElement FindOneByIdValue(XmlNodeList list,string id,string idValue)
    {
        foreach (XmlElement node in list)
        {
            if (node.GetAttribute(id) == idValue)
                return node;
        }
        return null;
    }

    public static XmlElement FindOneByTagIdValue(XmlElement element,string tag, string id = "", string idValue = "")
    {
        XmlNodeList list = element.GetElementsByTagName(tag);
        if (list.Count == 0)    return null;
        if (id == "")           return list[0] as XmlElement;
        return FindOneByIdValue(list, id, idValue);
    }

}