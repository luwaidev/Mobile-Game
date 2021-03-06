using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PowerupManager : MonoBehaviour
{
    [Header("References")]
    public Projectile projectilePrefab;
    public TMP_Text gemText;
    private PlayerController pc;
    private GameManager gm;
    private InputManager input;
    private Animator anim;
    public Button[] buttons;
    private EventSystem eventSystem;
    public bool menuOpened;

    [Header("Activated Powerups")]
    public bool canTripleJump;
    public bool slowDownTime;
    public bool extraLife;
    public bool slowDownProjectiles;
    public bool doubleCoins;

    [Header("Powerup Prices")]
    public int tripleJumpPrice;
    public int slowDownTimePrice;
    public int extraLifePrice;
    public int slowDownProjectilesPrice;
    public int doubleCoinsPrice;

    [Header("Settings")]
    public bool tripleJumped;
    public float timeSlowFactor;
    public float slowerProjectileSpeed;
    public int lastGems;
    public float upVelocity;
    public float velocity;
    private bool canStart;


    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        input = GameObject.FindGameObjectWithTag("Input").GetComponent<InputManager>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        anim = GetComponent<Animator>();

        // Set powerup prices
        foreach (Button button in buttons)
        {
            if (button.name == "Triple Jumping")
            {
                button.gameObject.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = tripleJumpPrice.ToString();
            }
            else if (button.name == "Slow Down Time")
            {

                button.gameObject.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = slowDownTimePrice.ToString();
            }
            else if (button.name == "Extra Life")
            {
                button.gameObject.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = extraLifePrice.ToString();
            }
            else if (button.name == "Slow Down Projectiles")
            {
                button.gameObject.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = slowDownProjectilesPrice.ToString();
            }
            else if (button.name == "Double Coins")
            {
                button.gameObject.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = doubleCoinsPrice.ToString();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        gemText.text = gm.gems.ToString();

        if (canTripleJump) TripleJumping();
        if (slowDownTime) SlowDowntime();
        if (extraLife) ExtraLife();
        if (slowDownProjectiles) SlowDownProjectiles();

        if (canStart && input.tapped)
        {
            pc.GetComponent<CapsuleCollider2D>().enabled = true;
            pc.GetComponent<Rigidbody2D>().isKinematic = false;
            pc.dead = false;
            GameObject.FindGameObjectWithTag("Background").GetComponent<Animator>().enabled = true;
            GameObject.FindGameObjectWithTag("Level").GetComponent<LevelManager>().velocity = velocity;
        }


    }

    public void TriggerMenu()
    {
        gm.TriggerPowerupMenu();
        anim.SetTrigger("Toggle");
        pc.atShop = false;
        eventSystem.SetSelectedGameObject(null);
    }
    public void BuyItem(Button button)
    {
        if (button.name == "Triple Jumping")
        {
            if (gm.candies >= tripleJumpPrice)
            {

                canTripleJump = true;
                gm.candies -= tripleJumpPrice;
            }
        }
        else if (button.name == "Slow Down Time")
        {
            if (gm.candies >= slowDownTimePrice)
            {

                slowDownTime = true;
                gm.candies -= slowDownTimePrice;
            }
        }
        else if (button.name == "Extra Life")
        {
            if (gm.candies >= extraLifePrice)
            {

                extraLife = true;
                gm.candies -= extraLifePrice;
            }
        }
        else if (button.name == "Slow Down Projectiles")
        {
            if (gm.candies >= slowDownProjectilesPrice)
            {

                slowDownProjectiles = true;
                gm.candies -= slowDownProjectilesPrice;
            }
        }
        else if (button.name == "Double Coins")
        {
            if (gm.candies >= doubleCoinsPrice)
            {

                doubleCoins = true;
                gm.candies -= doubleCoinsPrice;
            }
        }
        CheckButtons();
    }
    public void CheckButtons()
    {
        foreach (Button button in buttons)
        {
            if (button.name == "Triple Jumping")
            {
                button.interactable = !canTripleJump;
            }
            else if (button.name == "Slow Down Time")
            {
                button.interactable = !slowDownTime;
            }
            else if (button.name == "Extra Life")
            {
                button.interactable = !extraLife;
            }
            else if (button.name == "Slow Down Projectiles")
            {
                button.interactable = !slowDownProjectiles;
            }
            else if (button.name == "Double Coins")
            {
                button.interactable = !doubleCoins;
            }
        }
    }
    void TripleJumping()
    {
        if (pc.jumped && pc.doubleJumped && !tripleJumped)
        {
            pc.doubleJumped = false;
            tripleJumped = true;
        }
        tripleJumped = !(pc.isGrounded && tripleJumped);
    }

    void SlowDowntime()
    {
        if (!gm.paused) Time.timeScale = timeSlowFactor;
    }

    void ExtraLife()
    {
        gm.hasExtraLife = true;
        if (pc.dead)
        {
            StartCoroutine(ResetLife());
            gm.hasExtraLife = false;
            extraLife = false;
        }
    }
    void SlowDownProjectiles()
    {
        if (projectilePrefab.velocity != slowerProjectileSpeed) projectilePrefab.velocity = slowerProjectileSpeed;
    }

    void DoubleCoins()
    {
        if (gm.gems > lastGems) gm.AddGems();
        lastGems = gm.gems;
    }

    IEnumerator ResetLife()
    {
        velocity = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelManager>().velocity;
        GameObject.FindGameObjectWithTag("Level").GetComponent<LevelManager>().velocity = 0;
        yield return new WaitForSeconds(2);

        GameObject.FindGameObjectWithTag("Level").GetComponent<LevelManager>().velocity = 0.75f;
        yield return new WaitForSeconds(1);
        GameObject.FindGameObjectWithTag("Level").GetComponent<LevelManager>().velocity = 0;


        // Reset animation
        Animator anim = pc.GetComponent<Animator>();
        anim.Rebind();
        anim.Update(0f);

        pc.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        pc.GetComponent<Rigidbody2D>().isKinematic = true;

        pc.transform.position = new Vector2(0, -10);
        while (pc.transform.position.y < -0.5)
        {
            pc.transform.position = new Vector2(0, pc.transform.position.y + upVelocity);
            yield return null;
        }

        yield return new WaitForSeconds(0f);


    }
}
