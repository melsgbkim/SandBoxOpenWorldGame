﻿using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    // Use this for initialization
    void Start () {
        //XMLFileLoader.Loader.CreateXml();
        /*XmlFile file = XMLFileLoader.Loader.File("XML/tmp");
        XmlElement tmp = file.GetNodeByID("tmp", "Character");
        print(tmp);*/
        XmlElement CubeItem = XMLFileLoader.Loader.File("Cube").GetNodeByID("cube_00000003", "Cube");
        print(CubeItem);
    }
    float deltaTime = 0.0f;
    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
