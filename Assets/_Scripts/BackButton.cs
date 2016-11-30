using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour {
    public Camera cam;
    RaycastHit hit;
    void Update() {
        if (Input.GetButton("Fire1")) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (hit.transform.name.Contains("BackButton")) {
                    loadSitefinder();
                }
            }
            
        }
    }
    public void loadSitefinder() {
        print("Loading SiteFinder");
        SceneManager.LoadScene("SiteFinder");

    }
}
