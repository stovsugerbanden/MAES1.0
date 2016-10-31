using UnityEngine;
using System.Collections;
using System.Diagnostics;


public class StartProcessingExe : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Process foo = new Process();
        foo.StartInfo.FileName = "SVSProcessingUnity2.exe";
        foo.Start();
    }
}
