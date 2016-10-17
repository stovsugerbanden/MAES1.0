using UnityEngine;
using System.Collections;

public class Current : MonoBehaviour {
    public float strLimit, step;
    private float str = 0;
    private float limitL, limitR;

    public Vector3 dir;
    //Rigidbody rb;
	void Start () {
        //rb = GetComponent<Rigidbody>();
        SetVals();
	}
	
	void Update () {
        if (str <= limitR)
        {
            step = -step;
            SetVals();
            str = limitR;
        }

        if (str >= limitL) {
            step = -step;
            SetVals();
            str = limitL;
        }
        str += step;

        dir = new Vector3(0, 0, str);
        //rb.AddForce(dir);
	}

    void SetVals() {
        limitR = Random.Range(0, -strLimit);
        limitL = Random.Range(0, strLimit);
    }

    public Vector3 GetDir() {
        return dir;
    }
}
