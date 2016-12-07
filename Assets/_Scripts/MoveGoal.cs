using UnityEngine;

public class MoveGoal : MonoBehaviour
{

    public float speed = 30;
    private float negative, positive;
    private float limX, limY, limZ;
    public float restrict = .8f;


    public enum directions { x, y, z }
    public directions dir;

    public Transform SpawnArea;

    void Start()
    {
        positive = -speed;
        negative = speed;

        //print(SpawnArea.localScale.x / 2 + " " + -SpawnArea.localScale.x / 2);

        limX = SpawnArea.localScale.x * .5f * restrict;
        limY = SpawnArea.localScale.y * .5f * restrict;
        limZ = SpawnArea.localScale.z * .5f * restrict;
    }

    void Update()
    {
        //if (Random.Range(0, 5000) < .5f)
        //speed = -speed;

        switch (dir)
        {
            case directions.x:
                {
                    if (transform.position.x >= limX)
                        speed = -speed;
                    else if (transform.position.x <= -limX)
                        speed = -speed;

                    transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
                    break;
                }
            case directions.y:
                {
                    if (transform.position.y >= limY)
                        speed = -speed;
                    else if (transform.position.y <= -limY)
                        speed = -speed;

                    transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
                    break;
                }
            case directions.z:
                {
                    if (transform.position.z >= limZ)
                        speed = -speed;
                    else if (transform.position.z <= -limZ)
                        speed = -speed;

                    transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
                    break;
                }
        }
    }
}
