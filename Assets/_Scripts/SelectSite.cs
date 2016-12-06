using UnityEngine;

public class SelectSite : MonoBehaviour {

    public Camera cam;
    public Check1 c;
    RaycastHit hit;

    public bool hitting = false;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //print("Touchcount: "+Input.touchCount);
        if (/*Input.GetButton("Fire1") || */c.fingers == 1)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                hitting = false;
                if (hit.transform.name.Contains("pinPrefab"))
                {
                    hitting = true;
                    hit.transform.GetComponent<PinSelected>().isSelected(true);

                }
                if (hit.transform.name.Contains("Radar"))
                {
                    hitting = true;

                    hit.transform.parent.parent.parent.transform.GetComponent<PinSelected>().selectTimer = .3f;
                }
            }

        }
    }
}
