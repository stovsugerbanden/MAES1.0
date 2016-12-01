using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PinSelected : MonoBehaviour {
    public Transform radar;
    public GameObject selectedObj, loadingObj;
    public Camera cam;//, cam1;
    public float initLoadSceneTimer = 3;

    public float selectTimer = 0;
    private bool selected = false;

    private float loadSceneTimer = 0;


    // Use this for initialization
    void Start () {
        loadSceneTimer = initLoadSceneTimer;
	}
	
	void Update () {
        if (selected)
        {
            loadSceneTimer -= Time.deltaTime;
            selectedObj.SetActive(true);
            radar.LookAt(cam.transform.position);
            float scale = map(loadSceneTimer,0,initLoadSceneTimer,0,1f);
            print(loadSceneTimer+", "+scale);
            radar.transform.localScale = new Vector3(scale,scale,scale);
        }
        else {
            selectedObj.SetActive(false);
            loadSceneTimer = initLoadSceneTimer;    
        }

        selectTimer -= Time.deltaTime;
        if (selectTimer <= 0)
        {
            selected = false;
        }
        //print (selectTimer);
        if (loadSceneTimer <= 0)
        {
            loadingObj.SetActive(true);
            loadingObj.transform.LookAt(cam.transform.position);
            SceneManager.LoadScene("SVS scene");

            //cam.enabled = true;
            //cam1.enabled = true;

        }

    }

    public void isSelected(bool isSelect) {
        selectTimer = .3f;
        selected = isSelect;

    }

    float map(float value, float istart, float istop, float ostart, float ostop)
    {
        return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
    }
}
