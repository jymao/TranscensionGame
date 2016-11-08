using UnityEngine;
using System.Collections;

public class MageSpellScript : MonoBehaviour {

    private bool canMove = false;
    private Vector3 velocity;
    private float initTime;
    private float timeAlive = 5f;
    private float timeVelocity = 1f; //time after initTime when spell should have non-zero velocity

	// Use this for initialization
	void Start () {
        velocity = Vector3.zero;
        initTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	
        if(canMove)
        {
            transform.position += velocity * Time.deltaTime;
        }

        //if non-zero velocity by this time, remove
        if (Time.time > initTime + timeVelocity && velocity == Vector3.zero)
        {
            Destroy(gameObject);
        }

        if(Time.time > initTime + timeAlive)
        {
            Destroy(gameObject);
        }
	}

    public void SetVelocity(float speed)
    {
        GameObject player = GameObject.Find("Player");
        velocity = player.transform.position - transform.position;
        velocity = velocity.normalized * speed;
        canMove = true;
    }
}
