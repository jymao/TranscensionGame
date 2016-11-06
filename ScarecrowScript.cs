using UnityEngine;
using System.Collections;

public class ScarecrowScript : MonoBehaviour {

    public int maxHealth;
    //public int attack;
    public float speed;
    public float jumpCooldown;
    public float jumpRange;
    public float invulnPeriod;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Animator animator;

    private int health;
    private Vector3 targetPosition;
    private float lastJumped;
    private float lastDamaged;

    private Vector3 velocity = Vector3.zero; //for smooth damp

	// Use this for initialization
	void Start () {
        health = maxHealth;
        animator = GetComponent<Animator>();
        lastJumped = Time.time;
        lastDamaged = Time.time;
        targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - lastJumped > jumpCooldown)
        {
            StartCoroutine(FindJumpPosition());
            lastJumped = Time.time;
        }
	}

    void FixedUpdate()
    {
        //transform.position = Vector3.Lerp(transform.position, targetPosition, speed);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, speed);
    }

    private IEnumerator FindJumpPosition()
    {
        animator.SetBool("isJumping", true);

        yield return new WaitForSeconds(0.5f);

        bool invalid = true;

        while (invalid)
        {
            //pick a direction
            float angle = Random.Range(0, 360);
            Vector3 direction = new Vector3(1, 0, 0);
            direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * direction;

            //find landing point
            Vector3 landPoint = transform.position + (direction * jumpRange);

            //check bounds
            if(landPoint.x < maxX && landPoint.x > minX && landPoint.y < maxY && landPoint.y > minY)
            {
                invalid = false;
                targetPosition = landPoint;
            }
        }

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("isJumping", false);

    }

    public void TakeDamage(int damage)
    {
        if (damage <= health && health != 0)
        {
            health -= damage;
        }
        else
        {
            health = 0;
        }

        if(health == 0)
        {
            Destroy(transform.gameObject);
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        Color transparent = new Color(1f, 0f, 0f, 1f);
        Color original = new Color(1f, 1f, 1f, 1f);
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        //toggle alpha value of sprite for animation when hit and i-frames
        int x = 0;
        while (x < (invulnPeriod / 0.1f))
        {
            renderer.color = transparent;
            yield return new WaitForSeconds(0.05f);
            renderer.color = original;
            yield return new WaitForSeconds(0.05f);
            x++;
        }

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "PlayerAttack")
        {
            if (Time.time - lastDamaged > invulnPeriod)
            {
                StartCoroutine(InvincibilityFrames());
                TakeDamage(GameObject.Find("Player").GetComponent<PlayerScript>().attack);
                lastDamaged = Time.time;
            }
        }
    }
}
