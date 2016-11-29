using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("r") )
		{

            SceneManager.LoadScene(Application.loadedLevelName);
			//Application.LoadLevel (Application.loadedLevelName);

		}
	}
}
