using UnityEngine;
using System.Collections;



public class ReadWebcam : MonoBehaviour {
    WebCamTexture wct;

    // Use this for initialization
    void Start () {
        WebCamDevice[] hjortesebastian = WebCamTexture.devices;
        //print(hjortesebastian.Length);
        for (int i = 0; i < hjortesebastian.Length; i++)
        {
            print(hjortesebastian[i].name);
        }
        wct = new WebCamTexture();

        print(wct.width +" "+ wct.requestedHeight);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
