using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {
    public void loadSitefinder() {
        print("Loading SiteFinder");
        SceneManager.LoadScene("SiteFinder");

    }
}
