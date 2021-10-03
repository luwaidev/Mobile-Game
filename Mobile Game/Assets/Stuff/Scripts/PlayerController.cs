using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public LayerMask groundLayer;
    private InputManager input;
    private GameManager gm;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private Animator anim;

    [Header("Player State")]
    public bool jumped;
    public bool isGrounded;
    public bool dead;
    private bool atHouse;

    [Header("Movement Variables")]
    public float jumpHeight;
    public bool doubleJumped;
    public float velocity;
    public Vector2 deathVelocity;

    [Header("Level Interactions")]
    public float jumpPadHeight;

    [Header("Effects")]
    public ParticleSystem runParticle;
    public MMFeedbacks jumpFeedback;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

    }
    // Start is called before the first frame update
    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Input").GetComponent<InputManager>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        anim.runtimeAnimatorController = gm.playerSkin;
    }


    // Update is called once per frame
    void Update()
    {
        jumped = false;
        // Check if player is on ground
        RaycastHit2D groundCast = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        isGrounded = groundCast == true;
        if (isGrounded && !runParticle.isEmitting) runParticle.Play();
        if (!isGrounded && runParticle.isEmitting) runParticle.Stop();

        if (!dead)
        {
            // If just touched screen && can jump
            if ((isGrounded || !doubleJumped) && input.tapped && !atHouse)
            {
                doubleJumped = !isGrounded;
                rb.velocity = new Vector2(0, jumpHeight);
                anim.SetTrigger("Jump");
                jumpFeedback.PlayFeedbacks();
                jumped = true;
            }
            else if (atHouse && input.tapped)
            {
                gm.DepositCandies();
            }
            anim.SetBool("IsGrounded", isGrounded);

            // Keep position in center
            rb.velocity = new Vector2(-Mathf.Lerp(transform.position.x, 0, velocity), rb.velocity.y);
        }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Hurt" && !dead)
        {
            anim.SetTrigger("Died");
            dead = true;
            GetComponent<CapsuleCollider2D>().enabled = false;
            GameObject.FindGameObjectWithTag("Background").GetComponent<Animator>().enabled = false;
            rb.velocity = new Vector2(-Mathf.Sign(transform.position.x) * deathVelocity.x, deathVelocity.y);
            gm.PlayerDied();
        }
        else if (other.tag == "Jump")
        {
            other.gameObject.GetComponent<Animator>().SetTrigger("Launch");
            rb.velocity = new Vector2(0, jumpPadHeight);
        }
        else if (other.tag == "House")
        {
            atHouse = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "House")
        {
            atHouse = false;
        }
    }
}