using UnityEngine;

public class SelectSite : MonoBehaviour {

    public Camera cam;
    RaycastHit hit;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //print("Touchcount: "+Input.touchCount);
        if (Input.GetButton("Fire3"))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (hit.transform.name.Contains("pinPrefab"))
                {
                    hit.transform.GetComponent<PinSelected>().isSelected(true);

                }
                if (hit.transform.name.Contains("Radar"))
                {
                    hit.transform.parent.parent.parent.transform.GetComponent<PinSelected>().selectTimer = .3f;
                }
            }

        }
    }
}
