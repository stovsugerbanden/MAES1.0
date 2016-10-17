using UnityEngine;
using System.Collections;
using Cubiquity;

public class trackingCast : MonoBehaviour {
    RaycastHit hitInf;
    GameObject terrain;
    ClickToCarveTerrainVolume carve;
	middleMan mm;
    private TerrainVolume terrainVolume;
    int finalY;

    int range;
    public float differenceLimit = 2f;
    public float angle = .5f;
    public float length = 5;
    public float wsDisplace = 6;
    public int finalYDisplace = 2;
    Vector3 p;

	// Use this for initialization
	void Start () {
        terrain = GameObject.FindGameObjectWithTag("ProTerrain");
		mm = GameObject.FindGameObjectWithTag("Global").GetComponent<middleMan>();
        carve = terrain.GetComponent<ClickToCarveTerrainVolume>();
        terrainVolume = terrain.GetComponent<TerrainVolume>();
        range = carve.range;
	}
	
	// Update is called once per frame
	void Update () {
		//print (mm.GetNData ());
		if (!mm.GetNData ())
			return;
		mm.SetNData (false);
        Vector2 mousePos = Input.mousePosition;
        Ray ray = new Ray(transform.position, Vector3.down);
        PickSurfaceResult pickResult;
        bool hit = Picking.PickSurface(terrainVolume, ray, 1000.0f, out pickResult);

        if (hit)
        {
            /*Check whether there is a too large difference in the distances of rays, casted at an angle in all four sideways
            directions and down. If there is, move the digging point up. This is done to avoid violently digging into the
            sides of shallow spots, or "craters"*/

            Vector3 wsPos = pickResult.worldSpacePos + Vector3.up * wsDisplace;
            ArrayList dists = new ArrayList();

            //Draw rays
            Debug.DrawRay(wsPos, new Vector3(angle, -1, 0) * length, Color.green, 5, true);//west
            Debug.DrawRay(wsPos, new Vector3(-angle, -1, 0) * length, Color.green, 5, true);//east
            Debug.DrawRay(wsPos, new Vector3(0, -1, angle) * length, Color.green, 5, true);//north
            Debug.DrawRay(wsPos, new Vector3(0, -1, -angle) * length, Color.green, 5, true);//south
            Debug.DrawRay(wsPos, new Vector3(0, -1, 0) * length, Color.green, 5, true);//down

            //find distances and add to an array
            if (Physics.Raycast(wsPos, new Vector3(angle, -1, 0), out hitInf,length))
                dists.Add(hitInf.distance);
            if (Physics.Raycast(wsPos, new Vector3(-angle, -1, 0), out hitInf))
                dists.Add(hitInf.distance);
            if (Physics.Raycast(wsPos, new Vector3(0, -1, angle), out hitInf,length))
                dists.Add(hitInf.distance);
            if (Physics.Raycast(wsPos, new Vector3(0, -1, -angle), out hitInf,length))
                dists.Add(hitInf.distance);
            if (Physics.Raycast(wsPos, new Vector3(0, -1, 0), out hitInf,length))
                dists.Add(hitInf.distance);

            /*print it
            string printer = "";

            foreach (float d in dists) {
                printer += d + " ";
            }
            print(printer);*/

            //check for large differences 
            dists.Sort();
            finalY = (int)pickResult.volumeSpacePos.y;

            if (dists.Count > 0){
                float biggestD = (float)dists[dists.Count - 1] - (float)dists[0];
                if (biggestD >= differenceLimit){
                    print("Difference: " + biggestD);
                    
                    //finalY += (int)biggestD*finalYDisplace; //this is supposed to be used instead of the if in the try catch. 
                }

                //Call the vacuum script
                try
                {
                    if ((float)dists[dists.Count - 1] - (float)dists[0] < differenceLimit)//not very pretty!
                        carve.DestroyVoxels((int)pickResult.volumeSpacePos.x, finalY, (int)pickResult.volumeSpacePos.z, range);
                }
                catch (CubiquityException e)
                {
                    print("Cubiquity exception");
                    //This is cast if we try to call destroyVoxels on a y value that is out of the voxelvolume
                }
            }


        }
        /* if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
             print("Dig here: " + hit.point);
             p = hit.point;
             Debug.DrawRay(transform.position, Vector3.down, Color.green, 3, true);
             carve.DestroyVoxels((int)p.x, (int)p.y, (int)p.z,range);
         }*/
    }

    void SetRange(int r) {
        range = r;
    }
}
