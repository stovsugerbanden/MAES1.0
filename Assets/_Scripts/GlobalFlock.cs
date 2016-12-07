using UnityEngine;
using System.Collections;

public class GlobalFlock : MonoBehaviour
{
    #region Refs
    // Flocking reference, AI for Game Developers by Glenn Seemann, David M Bourg Chapter 4
    // https://www.safaribooksonline.com/library/view/ai-for-game/0596005555/ch04.html

    // The ApplyBasicFlockingRules() function handles the three basic flocking rules, and is originally based on Holistic3d's imlementation described in a video by herself here:
    // https://www.youtube.com/watch?v=eMpI1eCsIyM
    #endregion
    #region Description
    /*
    Description of the project

    */
    #endregion

    //private Vector3 goalPos = Vector3.zero; 
    private FlockAIUtilities utilities;
    private GameObject[] allFish;           //Holds all members

    public int numFish = 30;                //How many members
    public float changeGoalPosFreq = 2f;   //Randomizes goalPos around <changeGoalPosFreq> times in 1000 frames

    public GameObject[] goals;              //Contains the amount of goal meshes to use. Each goal is a common posistion that used to adjust the position of all members in one group. The amount of goals determines how many groups your flock can form.
    public GameObject[] restingAreas;       //Contains areas fit for resting. Use box meshes to define these. 

    public GameObject fishPrefab;           //Whatever fish/bird/human/particle/bacteria you want. The Fish.cs needs to be attached to the prefab.
    public GameObject SpawnArea;            //The area members can spawn and move in.
    public bool randomizePrefabSize = true; //Whether or not the size of the prefab should be randomized.
    public float sizeModifier = .55f;        //If size is randomized, this is how much they will differ from their original size


    void Start()
    {
        utilities = new FlockAIUtilities(transform);
        allFish = new GameObject[numFish];

        foreach (GameObject goal in goals)
        {

                goal.transform.position -= utilities.setRandomPosInArea(SpawnArea, .7f);
            
        }

        //Instantiate all the fish at random positions, and store them in allFish[]
        for (int i = 0; i < numFish; i++)
        {
            Vector3 pos = transform.position-utilities.setRandomPosInArea(SpawnArea, .5f);
            GameObject fishObj = Instantiate(fishPrefab, pos, Quaternion.identity) as GameObject;
            fishObj.GetComponent<Fish>().SetFlockReference(this);
            fishObj.transform.parent = transform;
            allFish[i] = fishObj;
        }
        //print(allFish.Length);

        if (randomizePrefabSize) //Slightly modify the size of the model prefab if chosen
            allFish = utilities.randomizeSize(allFish, sizeModifier);
    }

    void Update()
    {
        //Change the position of each goal once in a while (Frequency determined by 'changeGoalPosFreq') 
        foreach (GameObject goal in goals)
        {
            if (Random.Range(0, 1000) < changeGoalPosFreq)
            {
                goal.transform.position = utilities.setRandomPosInArea(SpawnArea, .85f);
            }
        }

        //DEBUG
        /*string p = "";
        foreach (GameObject f in allFish) {
            p += (int)f.GetComponent<Fish>().exhausted + " ";
            
        }
        print(p);*/
    }

    public Vector3 setRandomPosShortcut(float mod)
    {
        return utilities.setRandomPosInArea(SpawnArea, mod);
    }

   public Vector3 getRandomRestingPosInRange(Vector3 pos, float detectDistance) {
        //restingAreas = RandomizeArray(restingAreas);
        if (restingAreas.Length == 0)
            return pos;
        GameObject newPosGO = restingAreas[0];
        bool found = false;
        restingAreas = RandomizeArray(restingAreas);
        foreach (GameObject area in restingAreas)
        {
            if (Vector3.Distance(pos, area.transform.position) <= detectDistance &&
               Vector3.Distance(area.transform.position, pos) < Vector3.Distance(newPosGO.transform.position, pos))
            {
                
                newPosGO = area;
                found = true;
            }
        }

        //return area; //+utilities.setRandomPosInArea(area,1f)*/
        if (found)
            return newPosGO.transform.position;//utilities.setRandomPosInArea(newPosGO, 1f);
        else
            return pos;
    }

    static GameObject[] RandomizeArray(GameObject[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int r = (int)Random.Range(0, i);
            GameObject tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
        return arr;
    }

    public GameObject[] getAllFish()
    {
        return allFish;
    }

    public GameObject getSpawnArea()
    {
        return SpawnArea;
    }

    public GameObject getGoalPos()
    {
        return goals[Random.Range(0, goals.Length)];
    }
    #region Deprecated
    /*private GameObject[] randomizeSize(GameObject[] members, float sizeMod)//allFish, sizeModifier
    {
        if (sizeModifier >= 1f)
            sizeModifier = 0.9f;

        foreach (GameObject fish in members)
        {
            fish.gameObject.transform.localScale += new Vector3(
                slightlyRandomizeValue(fish.transform.localScale.x, sizeMod),
                slightlyRandomizeValue(fish.transform.localScale.y, sizeMod),
                slightlyRandomizeValue(fish.transform.localScale.z, sizeMod));
        }
        return allFish;
    }*/
    /*public float slightlyRandomizeValue(float val, float modifier)
    {
        return val + val * Random.Range(-modifier, modifier);
    }*/
    /*    public Vector3 setRandomPosInArea(float areaPercentage) {

            if (areaPercentage > 1)
                areaPercentage = 1;

            Transform t = SpawnArea.transform;
            Vector3 p = new Vector3(Random.Range(-t.localScale.x / 2 * areaPercentage, t.localScale.x / 2 * areaPercentage),
                                    Random.Range(-t.localScale.y / 2 * areaPercentage, t.localScale.y / 2 * areaPercentage),
                                    Random.Range(-t.localScale.z / 2 * areaPercentage, t.localScale.z / 2 * areaPercentage));
            return p;
        }*/
    #endregion
}
