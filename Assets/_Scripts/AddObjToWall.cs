using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObjToWall : MonoBehaviour {

    GameObject allObjects;
   	void Start () {
        allObjects = GameObject.FindGameObjectWithTag("AllObjects");
        foreach (Transform child in allObjects.transform)
        {
            child.gameObject.SetActive(false);
            //print(child.name);
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void setActiveObject(string name) {

        foreach (Transform child in allObjects.transform)
        {
            if(!(child.tag == "AllObjects")) //If this is not the parent GO(GameObject) disable it.
                child.gameObject.SetActive(false);

            if (child.name == name) { // Enable the GO with that name
                print("Success "+child.name + ""  + name);
                child.gameObject.SetActive(true);
            }

            if (child.parent.name == name) // Enable any potential child objects for the current GO
                child.gameObject.SetActive(true);
            
        }
    }
}
