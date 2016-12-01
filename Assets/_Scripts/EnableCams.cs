using UnityEngine;
using System.Collections;

public class EnableCams : MonoBehaviour {
    public Camera cam0, cam1;/*, cam2;*/	// Use this for initialization
	void Start () {
        cam0.enabled = true;
        cam1.enabled = true;
        Debug.Log("displays connected EnableCams: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        /*if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();*/
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
