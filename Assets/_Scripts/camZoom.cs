using UnityEngine;
using UnityEngine.EventSystems;

//using System.Windows;
//using System.Windows.Input;

//using System.Windows.Media;
//using System.Windows.Shapes;

public class camZoom : MonoBehaviour
{
    public float rotSpeed = 1;
    public float zoomSpeed = 4;
    public float maxZoom = 4.5f;

    float minZoom;
    float step = 1;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        minZoom = cam.fieldOfView;

    }

    void Update()
    {

        RaycastHit hit;
        if (Input.GetButton("Fire1") || Input.GetButton("Fire2") || Input.GetButton("Fire3"))
        {
            step = map(cam.fieldOfView, maxZoom, minZoom, 0.1f, 1.5f);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100))
            {
                Quaternion targetRot = Quaternion.LookRotation(hit.point - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);//Rotate towards click point

                if (cam.fieldOfView > maxZoom && Input.GetButton("Fire1"))//Zoom in 
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, cam.fieldOfView - step, zoomSpeed * Time.deltaTime);
                if (cam.fieldOfView < minZoom && Input.GetButton("Fire2"))//Zoom out
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, cam.fieldOfView + step, zoomSpeed * Time.deltaTime);
            }
        }
    }

    float map(float value, float istart, float istop, float ostart, float ostop)
    {
        return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
    }
}
