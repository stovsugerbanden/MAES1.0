using UnityEngine;
using System.Collections;

public class ObjectProperties : MonoBehaviour {
    public float mass, drag;
    Current c;

    void Start(){
        c = GameObject.FindGameObjectWithTag("Global").GetComponent<Current>();
    }

    void Update() {
        
        if (GetComponent<Rigidbody>()){
            GetComponent<Rigidbody>().AddForce(c.GetDir());
            //print(gameObject + "is moving towards " + c.GetDir());
        }
    }
}
