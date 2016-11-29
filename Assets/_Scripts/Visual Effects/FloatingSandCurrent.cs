using UnityEngine;
using System.Collections;

public class FloatingSandCurrent : MonoBehaviour {
    Current c;
    float curMaxStr;
	// Use this for initialization
	void Start () {
        c = GameObject.FindGameObjectWithTag("Global").GetComponent<Current>();
        curMaxStr = c.strLimit;
    }
	
	// Update is called once per frame
	void Update () {
        //print(c.GetDir().z + ", "+ map(c.GetDir().z, -curMaxStr, curMaxStr, -90, 90));
        //float rotVal = map(c.GetDir().z, -curMaxStr, curMaxStr, -90, 90);
        //transform.rotation = new Quaternion( 0,rotVal,0,0);
        //transform.Rotate(0, c.GetDir().z, 0);
        //transform.rotation.eulerAngles = new Vector3(0,90,0);
        if(c.GetDir().z > 1)
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        if (c.GetDir().z < -1)
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
    }

    public float map(float value, float istart, float istop, float ostart, float ostop)
    {
        return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
    }
}
