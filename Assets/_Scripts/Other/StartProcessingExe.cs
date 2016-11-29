using UnityEngine;
using System.Diagnostics;


public class StartProcessingExe : MonoBehaviour {
    public bool finishedBuild;
    private bool alreadyStarted = false;

	void Start () {
        if (finishedBuild && !alreadyStarted)
        {
            Process p = new Process();
            p.StartInfo.FileName = "SVSProcessingUnity2.exe";
            if (p.Start())
                alreadyStarted = true;
            else
                print("Could not start processing app");
        }
    }
}
