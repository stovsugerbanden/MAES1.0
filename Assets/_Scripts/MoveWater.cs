using UnityEngine;
using System.Collections;

public class MoveWater : MonoBehaviour {
    float counter = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (counter < 20)
        {
            counter += Time.deltaTime;
            if (counter > 3)
            {
                transform.position += new Vector3(0, 0.01f, 0);
            }
        }
	}
}
