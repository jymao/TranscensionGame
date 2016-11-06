using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

    //player stats
    public int maxHealth;
    public int attack;
    public int speed;
    public float attackCooldown;
    public float invulnPeriod;

    //for healthbar
    public Camera camera;
    public GameObject healthBar;
    public float healthBarSpeed;
    public GameObject heart;
    public GameObject emptyHeart;

    private Vector3 healthBarVelocity;
    private List<GameObject> hearts;

    //player attacks
    public GameObject upSlice;
    public GameObject downSlice;
    public GameObject leftSlice;
    public GameObject rightSlice;

    private Rigidbody2D rigidBody;
    private Animator animator;

    private int health;
    private bool canMove = true;
    private bool isDead = false;
    private float lastAttacked;
    private float lastDamaged;

    private Vector2 movement_vector;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = maxHealth;
        lastAttacked = Time.time;
        lastDamaged = Time.time;

        hearts = new List<GameObject>();

        initHealthBar();
	}
	
	// Update is called once per frame
	void Update () {

        if (canMove)
        {
            movement_vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (movement_vector != Vector2.zero)
            {
                //prevent moving while attacking and standing
                if (!animator.GetBool("isAttacking"))
                {
                    animator.SetBool("isRunning", true);
                    animator.SetFloat("input_x", movement_vector.x);
                    animator.SetFloat("input_y", movement_vector.y);
                }
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }

        //Mapped to "X" on keyboard and "PageUp" = "X" on Steam Controller
        if (Input.GetAxisRaw("Fire1") != 0)
        {
           
            if(Time.time - lastAttacked > attackCooldown)
            {
                StartCoroutine(Attack());
                lastAttacked = Time.time;
            }
        }

	}

    //Handle movement here
    void FixedUpdate()
    {
       
        if (canMove)
        {
            rigidBody.MovePosition(rigidBody.position + movement_vector * speed * Time.deltaTime);
        }
    }

    //for healthBar following camera
    void LateUpdate()
    {
        UpdateHealthBarPos();
    }

    private IEnumerator Attack()
    {
        //can't move when attacking while standing
        if (!animator.GetBool("isRunning"))
        {
            canMove = false;
        }

        //handle animation
        animator.SetBool("isAttacking", true);

        float input_x = animator.GetFloat("input_x");
        float input_y = animator.GetFloat("input_y");

        //Debug.Log("input_x: " + input_x);
        //Debug.Log("input_y: " + input_y);

        //spawn attack hitbox
        float offset = 0.75f;
        GameObject slice;

        //default is up
        if(input_x == 0 && input_y == 0)
        {
            slice = (GameObject) Instantiate(upSlice, new Vector3(transform.position.x, transform.position.y + offset, 0), Quaternion.identity);
            slice.GetComponent<SliceScript>().SetVelocity(new Vector3(0, 1, 0));
        }
        //left
        if(input_x == -1)
        {
            slice = (GameObject) Instantiate(leftSlice, new Vector3(transform.position.x - offset, transform.position.y, 0), Quaternion.identity);
            slice.GetComponent<SliceScript>().SetVelocity(new Vector3(-1, 0, 0));
        }
        //right
        if(input_x == 1)
        {
            slice = (GameObject)Instantiate(rightSlice, new Vector3(transform.position.x + offset, transform.position.y, 0), Quaternion.identity);
            slice.GetComponent<SliceScript>().SetVelocity(new Vector3(1, 0, 0));
        }
        //up
        if(input_y == 1 && input_x == 0)
        {
            slice = (GameObject)Instantiate(upSlice, new Vector3(transform.position.x, transform.position.y + offset, 0), Quaternion.identity);
            slice.GetComponent<SliceScript>().SetVelocity(new Vector3(0, 1, 0));
        }
        //down
        if(input_y == -1 && input_x == 0)
        {
            slice = (GameObject)Instantiate(downSlice, new Vector3(transform.position.x, transform.position.y - offset, 0), Quaternion.identity);
            slice.GetComponent<SliceScript>().SetVelocity(new Vector3(0, -1, 0));
        }

        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(0.2f);
        canMove = true;
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
            Death();
        }
    }

    private void Death()
    {
        canMove = false;
        isDead = true;
        animator.SetBool("isDead", true);
    }

    public void Pause()
    {
        canMove = false;

        //store time to keep track of i-frames properly
    }

    public void UnPause()
    {
        canMove = true;
    }

    private IEnumerator InvincibilityFrames()
    {
        Color transparent = new Color(1f, 0f, 0f, 1f);
        Color original = new Color(1f, 1f, 1f, 1f);
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        //toggle alpha value of sprite for animation when hit and i-frames
        int x = 0;
        while(x < (invulnPeriod / 0.1f))
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
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "EnemyAttack")
        {
            if (Time.time - lastDamaged > invulnPeriod && !isDead)
            {
                if (health != 1)
                    StartCoroutine(InvincibilityFrames());

                TakeDamage(1);
                UpdateHealthBar();

                lastDamaged = Time.time;
            }
        }
        
    }

    //update health bar position so it follows the camera
    private void UpdateHealthBarPos()
    {
        float camHeight = 2f * camera.orthographicSize;
        float camWidth = camHeight * camera.aspect;

        float offsetX = 0.3f;
        float offsetY = -0.3f;
        float offsetZ = 10f;

        Vector3 target = camera.transform.position + new Vector3(-camWidth / 2f, camHeight / 2f, 0) + new Vector3(offsetX, offsetY, offsetZ);

        healthBar.transform.position = Vector3.SmoothDamp(healthBar.transform.position, target, ref healthBarVelocity, healthBarSpeed);
    }

    //set hearts in proper place
    private void initHealthBar()
    {
        int heartsPerRow = 10;
        int rows = (maxHealth / heartsPerRow) + 1;
        int heartsLeft = maxHealth;

        Vector3 offset = new Vector3(0, 0, 0);

        for(int i = 0; i < rows; i++)
        {
            for (int j = 0; j < heartsPerRow && heartsLeft > 0; j++)
            {
                GameObject currHeart = (GameObject) Instantiate(heart, Vector3.zero, Quaternion.identity);
                GameObject currEmptyHeart = (GameObject) Instantiate(emptyHeart, Vector3.zero, Quaternion.identity);
                currHeart.transform.parent = healthBar.transform;
                currEmptyHeart.transform.parent = healthBar.transform;
                currHeart.transform.position += offset;
                currEmptyHeart.transform.position += offset;

                hearts.Add(currHeart);

                offset += new Vector3(0.45f, 0, 0);
                heartsLeft--;
            }

            offset += new Vector3(-offset.x, -0.45f, 0);
        }
    }

    //change health max and reflect the change on the health bar graphic
    private void UpdateHealthMax(int delta)
    {

    }

    //change health bar graphic when taking damage or healing
    private void UpdateHealthBar()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if(i < health)
            {
                hearts[i].SetActive(true);
            }
            else
            {
                hearts[i].SetActive(false);
            }
        }
    }
}
