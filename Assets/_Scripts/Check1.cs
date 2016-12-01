using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Runtime.InteropServices;

using System.Diagnostics;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class tTouchData
{
    public int m_x;
    public int m_y;
    public int m_ID;
    public int m_Time;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]

public class Check1 : MonoBehaviour {
    public Text t;
    public int fingers = 0;
    [Tooltip("IMPORTANT! This needs to be the same as the name or the build, or touch will not work.")]
    public String buildName;
	public bool m_Initialised;
    [DllImport("TouchOverlay", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int Initialise(string Str);
    
    [DllImport("TouchOverlay")]
	public static extern int GetTouchPointCount();
	[DllImport ("TouchOverlay")]
	public static extern void GetTouchPoint(int i, tTouchData n);
	
	// Use this for initialization
	void Start () {
		m_Initialised = false;
        t.text = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
        //t.text = "OnGUI called "+m_Initialised;
		string Str;
		int NumTouch = 0;
		if (!m_Initialised)
		{
            //Str = "TouchTest";

            Str = buildName;
            if (Initialise(Str) < 0)
			{
                //t.text = "ERROR";
				// ERROR STATE
			}
            //t.text = "init true";
            m_Initialised = true;
		}
		
		NumTouch = GetTouchPointCount ();
        fingers = NumTouch;
		Str = "Number of Touch Points: " + NumTouch.ToString();
        //t.text = "Number of Touch Points: " + NumTouch.ToString();
        /*GUI.Label (new Rect (10,10,150,40), Str);
		for (int p=0; p<NumTouch; p++)
		{
			tTouchData TouchData = new tTouchData();
			GetTouchPoint (p, TouchData);
			GUI.Label (new Rect (10,10 + (p+1) * 40, 200, 40), 
				"ID:" + TouchData.m_ID + 
				"Time:" + TouchData.m_Time.ToString() + 
				"(" + TouchData.m_x.ToString() + "," + TouchData.m_y.ToString() + ")");
			
		}*/
	}	
}
