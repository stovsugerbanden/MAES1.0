using UnityEngine;
using System.Collections;
using System.Diagnostics;


public class StartProcessingExe : MonoBehaviour {
    public bool finishedBuild;
	// Use this for initialization
	void Start () {
        if (finishedBuild)
        {
            Process foo = new Process();
            foo.StartInfo.FileName = "SVSProcessingUnity2.exe";
            foo.Start();
        }
    }
}
