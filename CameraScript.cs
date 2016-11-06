using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public Transform target;
    public GameObject map;
    public float camSpeed;

    private Camera cam;
    private Bounds mapBounds;

    /* Unused: for smoothdamp in camera movement
    private Vector3 velocityX = Vector3.zero;
    private Vector3 velocityY = Vector3.zero;
    */

	// Use this for initialization
	void Start () {

        cam = GetComponent<Camera>();
        mapBounds = map.GetComponent<Renderer>().bounds;
	}
	
	// Update is called once per frame
	void Update () {

        //keep pixel art size consistent across different resolutions
        cam.orthographicSize = (Screen.height / 100f) / 1.75f;

        //track target position but stay within bounds of map
        if (target)
        {
            
            HandleCameraMovement(mapBounds);

        }
	}

    //Helper function for tracking target position while staying in bounds of the map
    private void HandleCameraMovement(Bounds mapBounds)
    {
        float mapHeight = mapBounds.size.y;
        float mapWidth = mapBounds.size.x;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        //whether camera should follow target on the axis specified
        bool trackTargetX = true;
        bool trackTargetY = true;

        if (target.position.x - (camWidth / 2f) < (-mapWidth / 2f))
        {
            trackTargetX = false;
        }
        if(target.position.x + (camWidth / 2f) > (mapWidth / 2f)) 
        {
            trackTargetX = false;
        }
        if(target.position.y - (camHeight / 2f) < (-mapHeight / 2f))
        {
            trackTargetY = false;
        }
        if (target.position.y + (camHeight / 2f) > (mapHeight / 2f))
        {
            trackTargetY = false;
        }

        //separate the components so camera can still follow target in the other axis
        Vector3 xComponent = new Vector3(transform.position.x, 0, 0);
        Vector3 yComponent = new Vector3(0, transform.position.y, 0);

        if(trackTargetX)
        {
            xComponent = Vector3.Lerp(xComponent, new Vector3(target.position.x, 0, 0), camSpeed);
            //xComponent = Vector3.SmoothDamp(xComponent, new Vector3(target.position.x, 0, 0), ref velocityX, camSpeed);
        }

        if(trackTargetY)
        {
            yComponent = Vector3.Lerp(yComponent, new Vector3(0, target.position.y, 0), camSpeed);
            //yComponent = Vector3.SmoothDamp(yComponent, new Vector3(0, target.position.y, 0), ref velocityY, camSpeed);
        }

        transform.position = xComponent + yComponent + new Vector3(0, 0, -10);
    }
}
