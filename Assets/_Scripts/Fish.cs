using UnityEngine;
using System.Collections;
//using UnityEditor.Animations;
//using Cubiquity;

public class Fish : MonoBehaviour
{
    #region Variables
    private GlobalFlock flock;              //Reference to the instantiated "parent" script, Flock.cs 
    private FlockAIUtilities utilities;     //Own instance of utilities

    private GameObject[] allFish;           //Contains all members of this flock
    private ScaryObject[] scaryObjects;
    private AvoidObject[] avoidObjects;
    private bool instatiated = false;       //This becomes true when all members have been retrieved from the Flock.cs instantiation.

    private float initSpeed;                //Each members original speed. Not changing. 
    private float initTurnTimer;            //Used for resetting the turntimer
    private float initStateChangeTimer;
    private float initRotateTimer;
    //private float initInteractTimer;
    //private Vector3 averageHeading;
    //private Vector3 averagePosition;
    private Vector3 scaredDirection;        //Stores the direction to move in when scared
    private Vector3 avoidDirection;         //Stores the direction to move in when avoiding
    private Vector3 avoidTerrainDirection;
    private Vector3 headPos;

    private GameObject goalPos;             //The overall position the member is moving towards

    private bool turning = false;           //Determines whether the fish recently turned around from one of the sides
    //private bool interacting = false;       //Determines whether the fish recently interacted with a static object like terrain, or another fish

    private enum States { flocking, resting, playing };
    private States state = States.flocking;

    private float exhausted;

    private Vector3 targetPos;
    private float targetSpeed;

    Vector3 downSurfaceAngle;


    /* Public Variables Description
    All of the below public varibles, and the similar fields in GlobalFlock.cs, change the behavior of the members. Be aware that many of them have 
    conseqenses on the effect of other variables as well when changed. As an example, changing the amount of members in a given area without also changing 
    the neighborRange, will change the amount of flocking the members do. Another example is how the speedModifier, if set to a high value, will reduce 
    flocking as well, because the members are then more likely to move out of neighborRange.
    */
    public float speed = 2.8f;              //The approximate starting speed
    public float speedModifyer = 0.5f;      //The modifier used to make the member move with varying speeds. (between 0-1f)
    public float rotationSpeed = 2f;        //How fast the member turns
    public float applyRulesFactor = 5f;     //Apply the 3 basic rules only one in <applyRulesFactor> times. This allows fish to sometimes swim away from the group, etc.
    public float neighborRange = 5f;        //When is a member considered a neighbor, and is thus eligible for grouping.
    public float restingNeighborDistance = 5f;
    public float restingGroundDistance;


    public float tooCloseRange = 1f;        //When is another member too close.
    public float groupSpeedReset = .7f;     //The approximate speed a group will start with in the beginning of a frame. The avarage speed is then calculated including this number.

    public float scaredDist = 1.5f;         // How close to a scary object does the member need to be, to start fleeing.
    public float fleeSpeed = 5f;            //The speed modifier used for fleeing

    public float avoidDist = .5f;           //How close to an avoidable object does the member need to be, to swim away.
    public float aloneSpeed = 1.5f;         //Move faster while alone

    private float restDownSpeed = 0;

    public float turnTimer = 2f;
    public float stateChangeTimer = 3f;
    public float rotateTimer;
    //public float interactTimer = .2f;
    public float viewDistance = 2f;

    public string[] dontCollideWith;        //Strings with the names of objects you dont want fish to collide with. Other fish is one example since the rules and raycasts detrmine what to do when they get near. Glass sides are handled more efficiently by RotationSwitch() as another example, and should therefore not be included in CollisionDetection.
    //public string[] visibleObjects;         //Names of objects that will be processed by the fishVision function.
    public string terrainNodeName = "OctreeNode";   //(Not needed with regular terrains, use tag instead) Replace with the name (or part of it) of the terrain or terrain nodes you want to hit with raycasting. Can be done simpler with tags, but not when using a voxel terrain generated with Cubiquity. 
    public Transform head;

    private Animator anim;

    public float exhaustionLimit = 100f;
    public bool canRest;
    private Vector3 restPosition;
    private bool grounded;
    private float restInRangeTimer;
    private float neighborInRangeTimer;
    private Vector3 restGoalPos;
    private Quaternion targetRotation;



    #endregion

    void Start()
    {
        scaryObjects = (ScaryObject[])FindObjectsOfType(typeof(ScaryObject));
        avoidObjects = (AvoidObject[])FindObjectsOfType(typeof(AvoidObject));
 
        utilities = new FlockAIUtilities(GameObject.FindGameObjectWithTag("GlobalFlock").transform);
        GUITexture texture = (GUITexture)FindObjectOfType(typeof(GUITexture));

        anim = GetComponent<Animator>();
        //volume = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainVolume>();

        initSpeed = speed;
        initTurnTimer = turnTimer;
        initStateChangeTimer = stateChangeTimer;
        initRotateTimer = rotateTimer;
        //initInteractTimer = interactTimer;

        //Set the speed to a value slightly higher or lower, so the fish will move with varying speeds.
        speed = CalculateSpeed(1f, true);

        //Choose a random initial goal.
        goalPos = flock.getGoalPos();

        exhausted = Random.Range(0, 90);
    }

    void Update()
    {   
        //Fill the array of fish from the GlobalFlock instance. Can't do this in Start() since the array is not full yet until the last fish is instatiated. Every fish needs to do this of course
        if (!instatiated)
            InstatiateFish(); 

        //What needs to happen before the state update
        PreStateUpdate();

        //Apply rules depending on state.
        if(!turning)
            switch (state)
            {
                //State 1: Flocking
                case States.flocking:
                    FlockingUpdate();
                    break;
                //State 2: Resting
                case States.resting:
                    RestingUpdate();
                    break;
                //State 3: Playful
                case States.playing:
                    PlayingUpdate();
                    break;
            }

        //Whatever needs to happen after the state update
        PostStateUpdate();

        //Move along the fish's local Z axis (Forward)
        float diff = speed - targetSpeed;
        if (diff < 0f)
            speed += 0.1f;
        if (diff > 0)
            speed -= 0.1f;
        //speed = targetSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        if(!(anim == null))
            anim.speed = speed * .4f;

    }

    #region Pre & Post Updates
    private void PreStateUpdate()
    {
        if (restInRangeTimer > 0)
            restInRangeTimer -= Time.deltaTime;

        if (stateChangeTimer > 0)
            stateChangeTimer -= Time.deltaTime;

        if (exhausted <= exhaustionLimit && state != States.resting)
        {
            exhausted += Time.deltaTime;
        }

        if (turning)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                turning = false;
                turnTimer = initTurnTimer;
            }
        }
    }

    private void PostStateUpdate() //This will owerwrite changes made by the current state.
    {
        //If the fish is out of allowed range, find a new target position within the area.
        if (TurnAroundSwitch())
        {
            //Turn the fish around ..
            //print("here");
            Vector3 dir = utilities.setRandomPosInArea(flock.getSpawnArea(), .4f) - transform.position; //.. find a new target position
            Rotate(dir);
            CalculateSpeed(1f, true);// .. and randomize the speed
            turning = true;
        }

        //If an avoidable object is nearby ..
        if (AvoidObjectNearby())
        {
            Rotate(avoidDirection);
            //CalculateSpeed(1f, true); // .. move away 
        }

        //If a scary object is nearby ..
        if (ScaryObjectNearby())
        {
            Rotate(scaredDirection);
            CalculateSpeed(fleeSpeed, true); // .. flee fast 
        }

        //Use raycasting to give the fish information on its surroundings
        fishVision();

        SwitchToRestStateIfExhausted();
    }

    private void SwitchToRestStateIfExhausted()
    {   //If exhausted or already resting
        if ((exhausted >= exhaustionLimit * .85f || state == States.resting) && canRest)
        {
            
            bool hitSomething = false;
            float restGroundDistance = 1.7f;
            RaycastHit hit;

            Ray rayRestUp = new Ray(headPos, Vector3.up);
            if (Physics.Raycast(rayRestUp, out hit, viewDistance * restGroundDistance))
            {
                if (hit.transform.name.Contains(terrainNodeName) || hit.transform.tag.Equals("Prop"))
                {
                    Debug.DrawRay(headPos, Vector3.up * viewDistance * restGroundDistance, Color.black, 2f);
                    hitSomething = true;
                }
            }

            Ray rayRestDown = new Ray(headPos, -Vector3.up);
            if (Physics.Raycast(rayRestDown, out hit, viewDistance * restGroundDistance))
            {
                if (hit.transform.name.Contains(terrainNodeName) || hit.transform.tag.Equals("Prop"))
                {
                    Debug.DrawRay(headPos, -Vector3.up * viewDistance * restGroundDistance, Color.black, 2f);
                    hitSomething = true;
                }
            }



            if (hitSomething)
            {
                //If the fish needs to rest, switch to resting state
                if (state != States.resting && stateChangeTimer <= 0)
                {
                    state = States.resting;
                    stateChangeTimer = initStateChangeTimer;
                }
                if (restInRangeTimer < 3f)
                    restInRangeTimer = 3f;

                //If there are a rest area nearby restGoalPos is set to that, otherwise it is set to transform.position
                restGoalPos = flock.getRandomRestingPosInRange(transform.position, 7f) - transform.position;
                bool rotate = false;
                Vector3 dir = Vector3.zero;

                //If the distance between this object and the potential goal is long enough modify the direction and set the rotate flag to true
                if (restGoalPos != transform.position && Vector3.Distance(restGoalPos, transform.position) > restingGroundDistance)
                {
                    dir += restGoalPos;
                    rotate = true;
                }
                else
                    restGoalPos = transform.position;

                //If the distance to the group or a surface above is 
                if (Vector3.Distance(transform.position, hit.point) >= restingGroundDistance && hit.point.y < transform.position.y)
                {
                    dir += new Vector3(0, -.5f, 0);
                    rotate = true;          
                }
                if (Vector3.Distance(transform.position, hit.point) >= restingGroundDistance && hit.point.y > transform.position.y)
                {
                    dir += new Vector3(0, .5f, 0);
                    rotate = true;
                }


                if (rotate)
                    Rotate(dir * Time.deltaTime);

            }
        }
    }

    #endregion

    #region Collision
    void OnCollisionEnter(Collision col)
    {
        //if(!interacting)
        CollisionReaction(col, Color.green);
    }


    void OnCollisionStay(Collision col)
    {
        //if (!interacting)
        CollisionReaction(col, Color.red);
    }

    private void CollisionReaction(Collision col, Color c)
    {
        foreach (ContactPoint contact in col.contacts)
        {
            bool collide = true;
            foreach (string dontCol in dontCollideWith)
            {
                if (contact.otherCollider.transform.name.Contains(dontCol))
                    collide = false;
            }

            if (collide)
            {
                //interacting = true;
                //print(contact.otherCollider.transform.name);
                Debug.DrawRay(contact.point, contact.normal, Color.green, 3f);
                avoidTerrainDirection = contact.normal;
                Rotate(avoidTerrainDirection);
                CalculateSpeed(.9f, true);

            }
        }
    }
    #endregion

    #region Vision
    private void fishVision()
    {
        RaycastHit hit;
        headPos = head.transform.position;
        grounded = false;
        float drawTime = .3f;
        //bool turned = false;

        //Forward Ray
        //Quaternion rot = transform.rotation;
        Ray rayForward = new Ray(headPos, transform.forward);
        if (Physics.Raycast(rayForward, out hit, viewDistance*.8f))
        {

            //Rotate to match the x and y eulerAngles of the part of the terrain hit.
            if (hit.transform.name.Contains(terrainNodeName) || hit.transform.tag.Equals("Prop"))
            {
                Debug.DrawRay(headPos, transform.forward * viewDistance*.8f, Color.cyan, drawTime);
                //print(hit.normal.x + " " +transform.position.normalized.x);
                //print(hit.transform.tag + " or "+ hit.transform.name);
                Rotate(new Vector3(hit.normal.x, 0, 0));//hit.normal.x, hit.normal.z
                                                                   //turned = true; interacting = true;
                                                                   //Stransform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-transform.forward), rotationSpeed * Time.deltaTime);
            }

            //If the hit result is another fish in front of this one and it's in another group, sometimes switch to that group
            if (hit.transform.tag.Equals("FlockEntity"))
            {
                Debug.DrawRay(transform.position, transform.forward * viewDistance*.8f, Color.white, drawTime);
                if (Random.Range(0, 20) < 1f && hit.transform.GetComponent<Fish>().GetGoal() != goalPos && transform.parent == hit.transform.parent)
                {
                    goalPos = hit.transform.GetComponent<Fish>().GetGoal();
                }
            }

        }

        bool turnedUpDown = false;
        //Up Ray       
        Ray rayUp = new Ray(headPos, transform.up);
        if (Physics.Raycast(rayUp, out hit, viewDistance*.5f))
        {
            if (hit.transform.name.Contains(terrainNodeName) || hit.transform.tag.Equals("Prop"))
            {
                Debug.DrawRay(headPos, transform.up * viewDistance*.5f, Color.red,drawTime);
                Rotate(new Vector3(0, hit.normal.y, 0));
                turnedUpDown = true;
            }
        }

        //Down Ray
        Ray rayDown = new Ray(headPos, -transform.up);
        if (Physics.Raycast(rayDown, out hit, viewDistance*.5f) && !turnedUpDown)
        {
            if (hit.transform.name.Contains(terrainNodeName) || hit.transform.tag.Equals("Prop"))
            {
                grounded = true; //If the fish needs to rest, go in to resting state
                Debug.DrawRay(headPos, -transform.up * viewDistance * .5f, Color.red, drawTime);
                downSurfaceAngle = new Vector3(hit.normal.x, hit.normal.y, hit.normal.z);
                Rotate(new Vector3(0, hit.normal.y, 0));//*.65f                                                      
            }
        }

        bool turned = false;
        //Right Ray
        Ray rayRight = new Ray(headPos, transform.right);
        if (Physics.Raycast(rayRight, out hit, viewDistance*0.5f))
        {
            if (hit.transform.name.Contains(terrainNodeName) || hit.transform.tag.Equals("Prop"))
            {
                Debug.DrawRay(transform.position, (transform.right) * viewDistance * 0.5f, Color.blue,drawTime);
                Rotate(new Vector3(hit.normal.x, 0, 0));
                turned = true;
            }
        }

        //Left Ray
        Ray rayLeft = new Ray(transform.position, -transform.right);
        if (Physics.Raycast(rayLeft, out hit, viewDistance*0.5f) && !turned)
        {
            if (hit.transform.name.Contains(terrainNodeName) || hit.transform.tag.Equals("Prop"))
            {
                Debug.DrawRay(transform.position, -transform.right * viewDistance * 0.5f, Color.blue, drawTime);
                Rotate(new Vector3(hit.normal.x, 0,0));
                //turned = true; interacting = true;
            }
        }
    }
    #endregion

    #region State Updates
    private void FlockingUpdate()
    {
        if (Random.Range(0, applyRulesFactor) < 1)
        {
            Vector3 thisPos = transform.position;
            Vector3 anotherPos;

            Vector3 groupCenter = Vector3.zero;
            Vector3 avoidFishDir = Vector3.zero;

            float groupSpeed = CalculateSpeed(groupSpeedReset, true);
            float dist;

            int groupSize = 1;

            foreach (GameObject anotherFish in allFish)
            {
                if (anotherFish != gameObject && anotherFish.GetComponent<Fish>().state == States.flocking)//If not this fish and the fish is in flocking state
                {
                    anotherPos = anotherFish.transform.position;
                    dist = Vector3.Distance(thisPos, anotherPos);
                    if (dist <= neighborRange) //If the current other fish is in 'neighborRange'..
                    {
                        // .. add its' position to adjust the groupCenter, and increment the groupSize.
                        groupCenter += anotherPos;
                        groupSize++;

                        if (dist < tooCloseRange) // If the other fish gets too close however, turn away from it.
                        {
                            avoidFishDir = avoidFishDir + (thisPos - anotherPos);
                        }

                        //Prepare to adjust the groupSpeed by adding in the other fish's speed. 
                        groupSpeed += anotherFish.GetComponent<Fish>().GetSpeed();

                    }
                }
            }

            if (groupSize > 1) // If in a group
            {
                //Calculate direction to turn towards the center of the group, ..
                groupCenter = groupCenter / groupSize;
                // .. add in the direction of the goal..
                groupCenter += (goalPos.transform.position - thisPos)*1.35f;
                speed = groupSpeed / groupSize; //.. and change the speed of this fish to the avarage speed of the group.
                speed = utilities.slightlyRandomizeValue(speed, 0.3f);

                //Turn towards groupCenter and the goalPos, and add in avoidFishDir to turn away from nearby fish.
                Vector3 dir = groupCenter + avoidFishDir;
                if (dir != Vector3.zero)
                    Rotate(dir);
            }
            else { // If alone
                CalculateSpeed(aloneSpeed, false);
                Vector3 dir = groupCenter + goalPos.transform.position;

                if (dir != Vector3.zero)
                    Rotate(dir);
            }
        }
    }

    private void PlayingUpdate()
    {
        
    }

    private void RestingUpdate()
    {
        //Return to flocking if rested
        exhausted -= Time.deltaTime * 5;
        if (exhausted <= exhaustionLimit * 0.01f)
        {
            state = States.flocking;
        }

        //Dont rest if alone too long.
        neighborInRangeTimer -= Time.deltaTime;

        //Apply rest grouping rules
        if (Random.Range(0, applyRulesFactor) < 1)
        {
            Debug.DrawRay(transform.position, Vector3.up, Color.magenta, .3f);
            targetSpeed = utilities.slightlyRandomizeValue(initSpeed, .6f) * .4f;

            //Resting group behavior
            ArrayList restingGroup = new ArrayList();
            Vector3 avoidFishDir = Vector3.zero;
            foreach (GameObject otherFish in allFish)
            {
                if (otherFish.GetComponent<Fish>().state == States.resting && otherFish != gameObject)
                {
                    float dist = Vector3.Distance(transform.position, otherFish.transform.position);
                    if (dist < restingNeighborDistance)
                    {
                        restingGroup.Add(otherFish);
                        if (neighborInRangeTimer < 3f)
                            neighborInRangeTimer = 3;

                        if (dist < tooCloseRange) // If the other fish gets too close however, turn away from it.
                        {
                            avoidFishDir = avoidFishDir + (transform.position - otherFish.transform.position);
                        }
                    }
                }
            }

            Vector3 groupCenter = Vector3.zero;
            int size = 0;
            foreach (GameObject restGroupMember in restingGroup)
            {
                groupCenter += restGroupMember.transform.position;
                size++;
            }
            if (restGoalPos != transform.position)
                groupCenter = groupCenter / size + restGoalPos - transform.position;
            else
                groupCenter = groupCenter / size - transform.position;

            Debug.DrawRay(transform.position, groupCenter, Color.yellow, .3f);
            //Debug.DrawRay(transform.position, groupCenter, Color.yellow, .3f);

            //rotate towards groupCenter and avoidFishDir
            Rotate(groupCenter + avoidFishDir * 1.5f);

        }

        if (restInRangeTimer <= 0.01 || neighborInRangeTimer <= 0.01 && stateChangeTimer <= 0.01)
        {
            state = States.flocking;
            stateChangeTimer = initStateChangeTimer;
        }
    }
    #endregion

    #region Interactable Object Methods 
    private bool AvoidObjectNearby()
    {
        avoidDirection = Vector3.zero;
        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, scaredDist);
        foreach (AvoidObject ao in avoidObjects)
        {
            if (Vector3.Distance(ao.transform.position, transform.position) < avoidDist)
            {
                avoidDirection = -(ao.transform.position - transform.position);
                state = States.flocking;
                return true;
            }

        }
        return false;
    }

    private bool ScaryObjectNearby()
    {
        scaredDirection = Vector3.zero;
        foreach (ScaryObject so in scaryObjects)
        {
            if (Vector3.Distance(transform.position, so.transform.position) < scaredDist)
            {
                scaredDirection = -(so.transform.position - transform.position);
                state = States.flocking;
                return true;
            }

        }
        return false;



        /*scaredDirection = Vector3.zero;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, scaredDist);
        foreach (Collider c in hitColliders)
        {
            if (c.gameObject.GetComponent<ScaryObject>() != null)
            {
                scaredDirection = -(c.gameObject.transform.position - transform.position);
                state = States.flocking;
                return true;
            }

        }
        return false;*/
    }
    #endregion

    #region Utilities
    private void Rotate(Vector3 dir)
    {
            if (dir != Vector3.zero)
                targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
    }
    //

    private float CalculateSpeed(float burst, bool init)
    {   //The burst parameter in CalulateSpeed is a speed modifyer. 1f means base speed, higher = faster, lower = slower.
        if (init)
            targetSpeed = initSpeed;

        targetSpeed *= burst;
        return utilities.slightlyRandomizeValue(targetSpeed, speedModifyer);
    }

    private bool TurnAroundSwitch()
    {
        Vector3 x = new Vector3(transform.position.x, 0, 0);
        Vector3 y = new Vector3(0, transform.position.y, 0);
        Vector3 z = new Vector3(0, 0, transform.position.z);
        Transform t = flock.SpawnArea.transform;

        if (Vector3.Distance(x, Vector3.zero) > t.localScale.x / 2 ||
            Vector3.Distance(y, Vector3.zero) > t.localScale.y / 2 ||
            Vector3.Distance(z, Vector3.zero) > t.localScale.z / 2)
            return true;
        else
            return false;
    }

    private void InstatiateFish()
    {
        allFish = flock.getAllFish();
        instatiated = true;
    }

    #endregion

    #region Accessors & Internal Utilities

    public float GetSpeed()
    {
        return speed;
    }

    public void SetFlockReference(GlobalFlock f)
    {
        flock = f;
    }

    public GameObject GetGoal() {
        return goalPos;
    }

    #endregion
}
