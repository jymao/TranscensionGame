using UnityEngine;
using System.Collections;

public class SortLayerScript : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Bounds bounds;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bounds = spriteRenderer.bounds;
	}
	
	// Update is called once per frame
	void Update () {
        //change sorting order based on y position of the bottom of the sprite
        spriteRenderer.sortingOrder = (int) ((transform.position.y - (bounds.size.y / 2f)) * -100);
	}
}
