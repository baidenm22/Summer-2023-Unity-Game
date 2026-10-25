using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CharcterController : MonoBehaviour
{
    //Character Movements
    Rigidbody2D playerRigid;
    Animator playerAnimator;
    Transform playerTransform;

    //Spawnables
    [SerializeField] int walkSpeed; //The speed at which the character moves
    [SerializeField] int rollSpeed1; //The first correction speed
    [SerializeField] int rollSpeed2; //The second correction speed
    [SerializeField] GameObject wallCollideSplat;
    [SerializeField] GameObject tower;
    [SerializeField] GameObject splatterCircle;
    bool towerClear = true;
    //[SerializeField] int MaxSplatJuice;
    //int splatJuice;
    //bool juiceRefillable = true;
    bool doubledClick = false;
    bool hitWait = false;

    //Character Health & events
    public static CharcterController current;
    [SerializeField] playerStats playerStats;
    public static event Action onHurt;
    public static event Action<int, int> onJuiceChange;
    public static event Action onOrbCollected;


    private void Start()
    {
        playerRigid = gameObject.GetComponent<Rigidbody2D>();
        playerAnimator = gameObject.GetComponent<Animator>();
        playerTransform = gameObject.GetComponent<Transform>();
        current = this;
    }

    /*
     * 
     *  INPUTS
     * 
     */


    private void Update()
    {

        //Update the animation with the Rigidbody's velocity
        setAnimation(new Vector2(playerRigid.velocity.x,playerRigid.velocity.y));


        //Summons a tower if pressing an alt fire --> will be the a touch without MOVEMENT
        //Also spawns a circle splat if quick single tapped --> will be if/'/' a double tap without MOVEMENT
        //Uses playerStats.getHealth() with the curreny in splat
        if (Input.GetMouseButtonDown(1) && playerStats.getHealth() >= 10 && !doubledClick)
        {
                doubledClick = true;
                StartCoroutine(doubleClick());
            
        } else if (Input.GetMouseButton(1) && playerStats.getHealth() >= 5 && !Input.GetMouseButton(0) && !doubledClick  && playerRigid.velocity.magnitude > 5)
        {
            spawnTowers();
        } else if (Input.GetMouseButtonUp(1) && doubledClick)
        {
            splatterSmash();
        } /*else
        {
            if(juiceRefillable && splatJuice < MaxSplatJuice)
            {
                StartCoroutine(juiceRefill());
            }
        }*/
        
    }

    private void FixedUpdate()
    {
        //Allows you to move if pressing the main trigger --> will be the 1ST touch
        if(Input.GetMouseButton(0))
        {
            //Calcuate the distance to the mouse to control the speed and if we are in roll mode
            int distanceToMouse = (int)calcuateVector().magnitude;
            switch (distanceToMouse)
            {
                case > 250:
                    playerRigid.drag = 1;
                    playerAnimator.SetBool("Roll", true);
                    movement(calcuateVector().x,calcuateVector().y, rollSpeed2);
                    break;
                case > 150:
                    playerRigid.drag = 3;
                    playerAnimator.SetBool("Roll", true);
                    movement(calcuateVector().x, calcuateVector().y, rollSpeed1);
                    break;
                case < 150:
                    playerRigid.drag = 10;
                    playerAnimator.SetBool("Roll", false);
                    movement(calcuateVector().x, calcuateVector().y, walkSpeed);
                    break;
            }
        } else if(playerRigid.velocity.magnitude < 5 && playerAnimator.GetBool("Roll"))
        {
            playerAnimator.SetBool("Roll", false);
        }

    }
    
    /*
     *  Character Movements
     * 
     */
    
    
    
    //Public movement code, this is to be ability to have events interact with the main character by moving him
    public void movement(float XDir, float YDir, int force)
    {
        playerRigid.AddForce(new Vector2(XDir * force *Time.fixedDeltaTime, YDir * force * Time.fixedDeltaTime));
    }

    //This will change the animation but only from this script
    private void setAnimation(Vector2 velocity)
    {
        //Changes the velocity to be either -1, 0, or 1
        Vector2 clampedVelocity = new Vector2(Mathf.Ceil(Mathf.Clamp(velocity.x, -1, 1)), Mathf.Ceil(Mathf.Clamp(velocity.y, -1, 1)));
        switch (clampedVelocity.ToString())
        {
            case "(1.00, 1.00)":
                playerAnimator.SetTrigger("NE");
                break;
            case "(1.00, 0.00)":
                playerAnimator.SetTrigger("E");
                break;
            case "(0.00, 1.00)":
                playerAnimator.SetTrigger("N");
                break;
            case "(-1.00, -1.00)":
                playerAnimator.SetTrigger("SW");
                break;
            case "(-1.00, 0.00)":
                playerAnimator.SetTrigger("W");
                break;
            case "(0.00, -1.00)":
                playerAnimator.SetTrigger("S");
                break;
            case "(-1.00, 1.00)":
                playerAnimator.SetTrigger("NW");
                break;
            case "(1.00, -1.00)":
                playerAnimator.SetTrigger("SE");
                break;
            case "(0.00, 0.00)":
                playerAnimator.SetTrigger("Idle");
                break;
        }
    }

    //This subtracts the player coords vs the mouse coords. ON SCREEN. Subtracted from the mouse in order to make player go toward the mouse4
    private Vector2 calcuateVector()
    {
        Vector2 mouse = Input.mousePosition;
        Vector2 player = Camera.main.WorldToScreenPoint(playerTransform.position);
        Vector2 distanceVector = new Vector2(mouse.x - player.x, mouse.y - player.y);
        return distanceVector;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Wall":
                if(playerAnimator.GetBool("Roll"))
                {
                    GameObject splat = Instantiate(wallCollideSplat, playerTransform.position, Quaternion.Euler(0, 0, 0));
                    juiceRefill(10);
                }
                break;
            case "Enemy":
                if(!hitWait)
                {
                    CharacterStats colObj = collision.gameObject.GetComponent<CharacterStats>();
                    playerStats.changeHealth(colObj.getBasicDamage());
                    takenDamage(colObj.getBasicDamage());
                    if(colObj.getBasicDamage() < 0)
                    {
                        StartCoroutine(collisionDelay());
                    }
                }
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                if (!hitWait)
                {
                    CharacterStats colObj = collision.gameObject.GetComponent<CharacterStats>();
                    playerStats.changeHealth(colObj.getBasicDamage());
                    takenDamage(colObj.getBasicDamage());
                    if (colObj.getBasicDamage() < 0)
                    {
                        StartCoroutine(collisionDelay());
                    }
                }
                break;
            case "Orb":
                onOrbCollected?.Invoke();
                break;
        }
    }

    /*
     * 
     *  Character Spawnables
     * 
     */



    //Spawns an attack Tower
    private void spawnTowers()
    {
        if (towerClear)
        {
            towerClear = false;
            StartCoroutine(spawnTowerDelay());
            playerStats.changeHealth(-5);
            onJuiceChange?.Invoke(playerStats.getHealth(), playerStats.getMaxHealth());
        }
    }
    //Spawns the SMASH TOWER
    private void splatterSmash()
    {
        playerRigid.velocity = Vector2.zero;
        resetPlayerTriggers();
        Instantiate(splatterCircle, playerTransform.position, Quaternion.Euler(0, 0, 0));
        playerAnimator.SetTrigger("Ground");
        playerStats.changeHealth(-10);
        onJuiceChange?.Invoke(playerStats.getHealth(), playerStats.getMaxHealth());
    }
    //Resets the triggers for the animations
    private void resetPlayerTriggers()
    {
        playerAnimator.ResetTrigger("N");
        playerAnimator.ResetTrigger("S");
        playerAnimator.ResetTrigger("E");
        playerAnimator.ResetTrigger("W");
        playerAnimator.ResetTrigger("NE");
        playerAnimator.ResetTrigger("NW");
        playerAnimator.ResetTrigger("SW");
        playerAnimator.ResetTrigger("SE");
    }

    //This stops the towers from being right on top of the player and with the speed based on velocity help keep it consitent
    IEnumerator spawnTowerDelay()
    {
       
        Instantiate(tower, playerTransform.position, Quaternion.Euler(0,0,0));
        yield return new WaitForSeconds(0.25f/playerRigid.velocity.magnitude);
        towerClear = true;
    }
    //This just a timer to allow the juice to refill at a set interval
    void juiceRefill(int amount)
    {
        playerStats.changeHealth(amount);
        onJuiceChange?.Invoke(playerStats.getHealth(), playerStats.getMaxHealth());
    }
    /*IEnumerator juiceRefill()
    {
        splatJuice += 1;
        juiceRefillable = false;
        yield return new WaitForSeconds(0.1f);
        juiceRefillable = true;
        onJuiceChange?.Invoke(getSplatJuice(), getMaxSplatJuice());
    }*/
    //This function checks if there has been half second between the last time the screen was pressed
    IEnumerator doubleClick()
    {
        
        yield return new WaitForSeconds(0.25f);
        doubledClick = false;
        towerClear = true;
    }
/*    public int getMaxSplatJuice()
    {
        return MaxSplatJuice;
    }*/
    /*
     *  Character Stats 
     *
     */
    private void takenDamage(int damage)
    {
        juiceRefill(damage);
        onHurt?.Invoke();
        //onHurt?.Invoke(playerStats.getHealth(),playerStats.getMaxHealth(),damage);

    }
    private void death()
    {
        
    }
    IEnumerator collisionDelay()
    {
        hitWait = true;
        yield return new WaitForSeconds(1f);
        hitWait = false;
    }
}
