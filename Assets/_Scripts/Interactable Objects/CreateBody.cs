using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreateBody : MonoBehaviour {

    public float distanceToGround = 1.0f;
    Rigidbody rb;
    RaycastHit hit;
    float mass, drag;
    string name;
    string infoText;
    ObjectProperties op;
    AddObjToWall activeObj;
    float startTimer = 0;

    Text t;



    void Start () {
        activeObj = GameObject.FindGameObjectWithTag("Global").GetComponent<AddObjToWall>();
        t = GameObject.FindGameObjectWithTag("WallCanvas").GetComponent<Text>();

        rb = GetComponent<Rigidbody>();
        op = GetComponent<ObjectProperties>();
        mass = op.mass;
        drag = op.drag;
        name = op.name;
        infoText = op.infoText;
        
	}
	
	void Update () {
        startTimer += Time.deltaTime;
        if (rb == null && startTimer > 4) {
			if (Physics.Raycast(transform.position, new Vector3(0,-1,0), out hit))
            {
                if (hit.distance > distanceToGround && hit.transform.name.Contains("OctreeNode"))
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
        activeObj.setActiveObject(name);
        t.text = infoText;
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
