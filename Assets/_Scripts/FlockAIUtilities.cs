using UnityEngine;
using System.Collections;

public class FlockAIUtilities 
{
    #region Used by Both
    Transform t;
    public FlockAIUtilities(Transform t) {
        this.t = t;
    }

    public float slightlyRandomizeValue(float val, float modifier)
    {
        if (modifier > val)
            modifier = val*.9f;
        return val + val * Random.Range(-modifier, modifier);
    }

    public Vector3 setRandomPosInArea(GameObject area, float areaPercentage)
    {

        if (areaPercentage > 1)
            areaPercentage = 1;

        Transform t = area.transform;
        Vector3 p = new Vector3(Random.Range(-t.localScale.x / 2 * areaPercentage, t.localScale.x / 2 * areaPercentage),
                                Random.Range(-t.localScale.y / 2 * areaPercentage, t.localScale.y / 2 * areaPercentage),
                                Random.Range(-t.localScale.z / 2 * areaPercentage, t.localScale.z / 2 * areaPercentage));
        return p/*+area.transform.position*/;
    }

    #endregion

    #region Used by GlobalFlock.cs

    public GameObject[] randomizeSize(GameObject[] members, float sizeMod)
    {
        if (sizeMod >= 1f)
            sizeMod = 0.9f;

        foreach (GameObject fish in members)
        {
            fish.gameObject.transform.localScale += new Vector3(
                slightlyRandomizeValue(fish.transform.localScale.x, sizeMod),
                slightlyRandomizeValue(fish.transform.localScale.y, sizeMod),
                slightlyRandomizeValue(fish.transform.localScale.z, sizeMod));
        }
        return members;
    }

    #endregion

    #region Used by Fish.cs
    /*public float CalculateSpeed(float speed, float burst, float speedMod, float initSpeed) 
    {
        speed = initSpeed;
        speed *= burst;
        return slightlyRandomizeValue(speed, speedMod);
    }*/

    /*public float CalculateSpeed(float speed, float burst, float speedMod)
    {

        speed *= burst;
        return slightlyRandomizeValue(speed, speedMod);
    }*/
    #endregion





}
