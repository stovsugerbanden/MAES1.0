using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

	void Update () {
        RaycastHit hit;
        Vector3 pos = transform.position;
        if (Physics.Raycast(pos, -Vector3.up, out hit))
        {
            //print(hit.distance);
        }
        else {
            transform.position = new Vector3(pos.x, pos.y+5, pos.z);
            //GetComponent<MoveCube>();
        }
    }
}
