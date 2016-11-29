using UnityEngine;
using System.Collections;

public class rotateObject : MonoBehaviour {
    [Tooltip("Rotation speed")]
    public float speed;
    [Tooltip("How many frames should pass before a new target destination is found.")]
    public int destFrequency;
    int count = 0;
    Quaternion targetRot;
	// Use this for initialization
	void Start () {
        targetRot = Random.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        //transform.rotation = Quaternion.Euler(new Vector3(0,.1f,0));
        /*float rotX = ((float)Random.Range(0, 100));
        float rotY = ((float)Random.Range(0, 100));
        float rotZ = ((float)Random.Range(0, 100));
        transform.Rotate(new Vector3(rotX, rotY, rotZ)*Time.deltaTime);*/
        count++;
        if (count > destFrequency) {
            targetRot = Random.rotation;
            count = 0;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);
	}
}
