using UnityEngine;
using System.Collections;

public class MoveWater : MonoBehaviour
{
    float counter = 0;

    public Camera cam;
    bool done = false;
    public float startFieldOfView = 60;
    float endFieldOfView, field;
    public float goalFogDensity = 0.012f;
    public float startFogDensity = 0.022f;

    public float waterSpeed = 0.012f;
    public float fogSpeed = 0.0001f;

    float waterAcc = 0;

    // Use this for initialization
    void Start()
    {
        endFieldOfView = cam.fieldOfView;
        field = startFieldOfView;
        cam.fieldOfView = field;
        RenderSettings.fogDensity = startFogDensity;

    }

    // Update is called once per frame
    void Update()
    {
        if (!done) //Stop everything after 10 seconds (This script is only used in the beginning of the program)
        {

            //print(RenderSettings.fogDensity);
            counter += Time.deltaTime;
            if (counter > 1)
            {
                //print(RenderSettings.fogDensity - goalFogDensity);
                if (RenderSettings.fogDensity > goalFogDensity && counter > 6)
                {
                    RenderSettings.fogDensity -= fogSpeed;
                }
                //print(RenderSettings.fogDensity + " " + goalFogDensity);
                transform.position += new Vector3(0, waterAcc, 0);
                if (waterAcc <= waterSpeed)
                    waterAcc += Time.deltaTime*0.02f; 
                if (field > endFieldOfView)
                {
                    field -= Time.deltaTime;
                    cam.fieldOfView = field;
                }

                if (transform.position.y > cam.transform.position.y - 10 && field <= endFieldOfView && RenderSettings.fogDensity < goalFogDensity + 0.001f)
                {
                    print("done");
                    done = true;
                }
            }
        }
    }
}
