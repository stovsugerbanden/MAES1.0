using UnityEngine;
using System.Collections;

public class CreateBody : MonoBehaviour {

    public float distanceToGround = 1.0f;
    Rigidbody rb;
    RaycastHit hit;
    float mass, drag;
    ObjectProperties op;

	void Start () {
        rb = GetComponent<Rigidbody>();
        op = GetComponent<ObjectProperties>();
        mass = op.mass;
        drag = op.drag;
        
	}
	
	void Update () {

        if (rb == null) {
			if (Physics.Raycast(transform.position, new Vector3(0,-1,0), out hit))
            {
                if (hit.distance > distanceToGround/* && !hit.transform.name.Contains("OctreeNode")*/)
                {
                    addBody();
                }
            }

        }
	}

    private void addBody()
    {
        gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.mass = mass;
        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
    }

    /*
    void OnCollisionEnter(Collision col) {
        //print(col.gameObject);
    }

    void OnCollisionExit(Collision col) {
        //print(gameObject.name);
        print(gameObject.name + ", " + col.gameObject.name + " exit");
    }


    void OnCollisionStay(Collision col)
    {
        if(gameObject.name.Contains("Octree"))
        print(gameObject.name + ", " +col.gameObject.name + " stay");
    }
    */


}
