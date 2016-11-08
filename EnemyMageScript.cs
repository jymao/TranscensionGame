using UnityEngine;
using System.Collections;

public class EnemyMageScript : MonoBehaviour {

    public int maxHealth;
    public float teleportCooldown;
    public float invulnPeriod;
    public float spellSpeed;

    public GameObject spell;

    private Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private Bounds mapBounds;

    private int health;
    private float lastTeleported;
    private float lastDamaged;

	// Use this for initialization
	void Start () {
        health = maxHealth;
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapBounds = GameObject.Find("Background").GetComponent<Renderer>().bounds;
        lastTeleported = Time.time - teleportCooldown;
        lastDamaged = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	    if(Time.time - lastTeleported > teleportCooldown)
        {
            StartCoroutine(Teleport());
            lastTeleported = Time.time;
        }
	}

    private IEnumerator Teleport()
    {
        animator.SetBool("isTeleportingAway", true);
        yield return new WaitForSeconds(0.4f);
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        animator.SetBool("isTeleportingAway", false);

        transform.position = WarpLocation();
        yield return new WaitForSeconds(1f);
        spriteRenderer.enabled = true;
        animator.SetBool("isTeleportingBack", true);

        yield return new WaitForSeconds(0.4f);
        boxCollider.enabled = true; //enable after showing up so teleports onto player are more fair
        animator.SetBool("isTeleportingBack", false);

        yield return new WaitForSeconds(1f);
        StartCoroutine(Attack());
    }

    private Vector3 WarpLocation()
    {
        //spawn within map bounds
        float mapHeight = mapBounds.size.y;
        float mapWidth = mapBounds.size.x;
        //offset for arena
        float offsetX = 4f;
        float offsetY = 3f;

        float x = Random.Range((-mapWidth / 2f) + offsetX, (mapWidth / 2f) - offsetX);
        float y = Random.Range((-mapHeight / 2f) + offsetY, (mapHeight / 2f) - offsetY);

        return new Vector3(x, y, 0);
    }

    private IEnumerator Attack()
    {
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.1f);
        float offsetY = 1f;
        GameObject spellAttack = (GameObject)Instantiate(spell, transform.position + new Vector3(0, offsetY, 0), Quaternion.identity);

        yield return new WaitForSeconds(0.8f);
        spellAttack.GetComponent<MageSpellScript>().SetVelocity(spellSpeed);

        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isAttacking", false);
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

        if (health == 0)
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(0.25f);
        GameObject gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<GameScript>().EnemyKilled();
        Destroy(transform.gameObject);
    }

    private IEnumerator InvincibilityFrames()
    {
        Color hurt = new Color(1f, 0f, 0f, 1f);
        Color original = new Color(1f, 1f, 1f, 1f);
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        //toggle alpha value of sprite for animation when hit and i-frames
        int x = 0;
        while (x < (invulnPeriod / 0.1f))
        {
            renderer.color = hurt;
            yield return new WaitForSeconds(0.05f);
            renderer.color = original;
            yield return new WaitForSeconds(0.05f);
            x++;
        }

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerAttack")
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
