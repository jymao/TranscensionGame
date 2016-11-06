using UnityEngine;
using System.Collections;

public class SliceScript : MonoBehaviour {

    private Vector3 velocity;
    private float initTime;

	// Use this for initialization
	void Start () {
        initTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        transform.position += velocity * Time.deltaTime;

        if(Time.time - initTime > 0.25f)
        {
            Destroy(transform.gameObject);
        }
	}

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }
}
