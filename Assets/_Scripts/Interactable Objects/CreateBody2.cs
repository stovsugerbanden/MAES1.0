using UnityEngine;
using System.Collections;

public class CreateBody2 : MonoBehaviour {

	Rigidbody rb;
	RaycastHit hit;
	float mass, drag;
	ObjectProperties op;

	void Start () {
		rb = GetComponent<Rigidbody>();
		op = transform.parent.GetComponent<ObjectProperties>();
		mass = op.mass;
		drag = op.drag;

	}

	void Update () {

		if (rb == null) {
			if (Physics.Raycast(transform.position, new Vector3(0,-1,0), out hit)) {//
				//print(hit.transform.name.Contains("OctreeNode"));
				if (hit.distance > 0.2f && hit.transform.name.Contains("OctreeNode"))
				{
					transform.parent.gameObject.AddComponent<Rigidbody>();
					rb = GetComponent<Rigidbody>();
					rb.drag = drag;
					rb.mass = mass;
				}
			}

		}
	}
}
