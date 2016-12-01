using UnityEngine;
using System.Collections;

public class DisplayScript : MonoBehaviour
{
    static bool started = false;
    // Use this for initialization
    void Start()
    {
        print(started);
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        if (!started)
        {
            if (Display.displays.Length > 1)
                Display.displays[1].Activate();
            if (Display.displays.Length > 2)
                Display.displays[2].Activate();

            started = true;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}