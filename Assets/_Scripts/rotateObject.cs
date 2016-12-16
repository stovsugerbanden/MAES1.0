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

        count++;
        if (count > destFrequency) {
            targetRot = Random.rotation;
            count = 0;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);
	}
}
