using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{

    //Imported Data
    string type;
    enum states { patrol, follow, slam, ram, shoot, inbetween, circle, dead };
    states eState = states.patrol;
    [SerializeField] Enemy enemyData;
    int movementSpeed;
    float maxVision = 0;
    float rangeVision = 0;
    float closeVision = 0;
    Animator eAnimator;
    Transform playerTransform;
    Transform currentTransform;
    Rigidbody2D eRigid;
    float attackSpeed;
    bool circleShootAttack;
    int numberOfBullets;
    float timeBetweenFires;
    float distanceToPlayer;
    float angleAdd;
    bool straightShootAttack;
    float slamDelay;
    float ramDelay;


    //Working Data
    Vector2 goToPoint;
    Vector2 Direction;
    bool atTarget = true;
    bool wait = false;
    [SerializeField]bool atWall = false;

    //Spawnables
    GameObject Projectile;
    GameObject Tower;
    GameObject Slam;
    [SerializeField] GameObject Target;
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        type = enemyData.getType();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentTransform = GetComponent<Transform>();
        eRigid = GetComponent<Rigidbody2D>();
        eAnimator = GetComponent<Animator>();
        movementSpeed = enemyData.getMovementSpeed();
        maxVision = enemyData.getVision(0);
        rangeVision = enemyData.getVision(1);
        closeVision = enemyData.getVision(2);
        attackSpeed = enemyData.getAttack_Speed();
        circleShootAttack = enemyData.getCircleShoot();
        numberOfBullets = enemyData.getNumberOfBullets();
        timeBetweenFires = enemyData.getTimeBetweenFire();
        Projectile = enemyData.getBulletPrefab();
        Tower = enemyData.getTowerPrefab();
        Slam = enemyData.getSlamPrefab();
        angleAdd = enemyData.getAngleAdd();
        straightShootAttack = enemyData.getStraightShoot();
        slamDelay = enemyData.times(0);
        ramDelay = enemyData.times(1);
        setUpTarget();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector2.Distance(playerTransform.position, currentTransform.position);
        switch (type)
        { 
            case "all":
                //basicType();
                break;
            case "ram":
                ramType();
                break;
            case "range":
                rangeType();
                break;
            case "bomb":
                breakType();
                break;
        }

        updateTarget();
    }
    void ramType()
    {
        switch (eState)
        {
            case states.patrol:
                patrol(7);
                patrolVision(states.follow);
                break;
            case states.ram:
                StartCoroutine(ram(4f, attackSpeed/2));
                ramVision();
                break;
            case states.follow:
                eAnimator.SetBool("Ram", false);
                follow(3f);
                followVision(0);
                break;      
        }
    }
    void breakType()
    {
        switch (eState)
        {
            case states.patrol:
                patrol(1);
                patrolVision(states.follow);
                break;
            case states.slam:
                StartCoroutine(slam(0.5f));
                break;
            case states.follow:
                follow(7f);
                followVision(1);
                break;
            case states.dead:
                Destroy(this.gameObject);
                break;
        }
    }

   /* void basicType()
    {
        switch (eState)
        {
            case states.patrol:
                patrol(5);
                patrolVision();
                break;
            case states.ram:
                StartCoroutine(ram(20f, attackSpeed * 1.5f));
                break;
            case states.follow:
                follow(1.5f);
                followVision();
                break;
            case states.shoot:
                shootStraight(attackSpeed,numberOfBullets,timeBetweenFires);
                break;
            case states.slam:
                StartCoroutine(slam(attackSpeed));
                slamVision();
                break;
            case states.inbetween:
                states[] whatStates = { states.ram, states.circle, states.shoot, states.circle};
                randomAttack(whatStates);
                break;
            case states.circle:
                circle(5f);
                circleVision();
                break;
        }
    }*/
    void rangeType()
    {
        switch (eState)
        {
            case states.patrol:
                patrol(2);
                patrolVision(states.follow);
                break;
            case states.follow:
                follow(1.1f);
                followVision(2);
                break;
            case states.shoot:
                shootStraight(attackSpeed, numberOfBullets, timeBetweenFires);
                break;
        }
    }

    //This cho0ses a point inbetween 0 to distance units and walks toward it
    //If the target runs into a wall it will set a new direction
    void patrol(int distance)
    {
        if(atTarget && !atWall)
        {
            goToPoint = new Vector2(Random.Range(-distance, distance), Random.Range(-distance, distance)) + (Vector2)currentTransform.position;
            atTarget = false;
        } else if(Vector2.Distance(goToPoint,currentTransform.position) < 0.25f)
        {
            atTarget = true;
        } else if(atWall)
        {
            atTarget = true;
            movement(Direction.x, Direction.y, -movementSpeed);
        } else
        {
            float angle = Mathf.Asin((goToPoint.y - currentTransform.position.y) / Vector2.Distance(goToPoint, currentTransform.position));
            float angle2 = Mathf.Acos((goToPoint.x - currentTransform.position.x) / Vector2.Distance(goToPoint, currentTransform.position));

            Direction = new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle));

            movement(Direction.x, Direction.y, movementSpeed);
            setAnimation(new Vector2(Direction.x, Direction.y));
        }
    }

    void patrolVision(states goToState)
    {
        if(distanceToPlayer <= maxVision)
        {
            eState = goToState;
        }
    }
    
    //This follows the player
    //If the player is behind a wall it will change its direction (WIP)
    void follow(float movementMultipler)
    {
       /* Vector2 offSet = Vector2.zero;
        float addTo = 0;
        RaycastHit2D hit = Physics2D.Raycast(currentTransform.position, Direction, 0.5f);
        if(hit.collider.gameObject.tag == "Wall")
        {
            offSet =
        }*/
            goToPoint = playerTransform.position;
            Direction = calcuatePlayerDirection();
            movement(Direction.x, Direction.y, movementSpeed * movementMultipler);
            setAnimation(new Vector2(Direction.x,Direction.y));
    }
    void followVision(int i)
    {
        switch (i)
        {
            //RamType
            case 0:
                if(distanceToPlayer >= 1.2 * maxVision)
                {
                    eState = states.patrol;
                    atTarget = true;
                } else if(distanceToPlayer <= rangeVision)
                {
                    //This makes sure the enemy doesn't instantly target the player
                    goToPoint = currentTransform.position;
                    eState = states.ram;
                }
                break;
            //BombType
            case 1:
               if (distanceToPlayer <= closeVision)
               {
                   eState = states.slam;
               }
                break;
            //RangeType
            case 2:
                if(distanceToPlayer <= rangeVision)
                {
                    eState = states.shoot;
                }
                break;
        }
    }
  
    /*void circleVision()
    {
        if (distanceToPlayer >= maxVision)
        {
            eState = states.patrol;
            goToPoint = currentTransform.position;
        }
        else if (distanceToPlayer > rangeVision * 1.2f)
        {
            eState = states.follow;
        } else if (distanceToPlayer <= closeVision)
        {
            wait = false;
            StopAllCoroutines();
            eState = states.slam;
        } else if(!wait)
        {
            StartCoroutine(waitTheseSeconds(3f, circleSwitchChance));
        }
    }
    void circleVision1()
    {
        if (distanceToPlayer >= 1.5 * maxVision)
        {
            eState = states.patrol;
            goToPoint = currentTransform.position;
        }
        else if (distanceToPlayer <= closeVision)
        {
            eState = states.ram;
        }
        else if (!wait)
        {
            StartCoroutine(waitTheseSeconds(3f, circleSwitchChance));
        }
    }

    void circle(float movementMultipler)
    {
        goToPoint = playerTransform.position;
        movement(goToPoint.x - currentTransform.position.x, goToPoint.y - currentTransform.position.y, movementSpeed * 0.1f);
        
        float angle = Mathf.Asin((goToPoint.y - currentTransform.position.y) / Vector2.Distance(goToPoint, currentTransform.position));
        float angle2 = Mathf.Acos((goToPoint.x - currentTransform.position.x) / Vector2.Distance(goToPoint, currentTransform.position));

        movement(-Mathf.Sin(angle), Mathf.Cos(angle2),(int)(movementSpeed*movementMultipler));
        setAnimation(new Vector2(goToPoint.x - currentTransform.position.x, goToPoint.y - currentTransform.position.y));
    }*/
   
    IEnumerator slam(float timeToWait)
    {
        states[] switchState = { states.dead };
        //Turn into a Coroutine 
        if (!wait)
        {
            StartCoroutine(waitTheseSeconds(timeToWait, switchState));
            yield return new WaitForSeconds(slamDelay);
            Instantiate(Slam, currentTransform.position + (Vector3)(calcuatePlayerDirection() * 0.5f), Quaternion.identity);
            eAnimator.SetTrigger("Slam");

        }
    }


    //Ram will first set animation to Ram for a cue. Then it will start the delay time that changes the unversial call for wait
    //After ramDealy time it will then ram the player by going toward the player direction for 5 units; 
    IEnumerator ram(float movementMultipler, float timeToWait)
    {
        states[] switchState = { states.follow };
        if (!wait)
        {
            StartCoroutine(waitTheseSeconds(timeToWait, switchState));
            yield return new WaitForSeconds(ramDelay);
            eAnimator.SetBool("Ram", true);
            goToPoint = (Vector2)currentTransform.position + (calcuatePlayerDirection() * 5f);
            Direction = new Vector2(goToPoint.x - currentTransform.position.x, goToPoint.y - currentTransform.position.y);
            setAnimation(Direction);
            
        } else if(Vector2.Distance(goToPoint,currentTransform.position) < 0.1f)
        {
            eAnimator.SetBool("Ram", false);
        } else
        {
            //This will make the ram less fast as it gets closer the target
            movement(goToPoint.x - currentTransform.position.x, goToPoint.y - currentTransform.position.y, movementSpeed * movementMultipler);
        }
      
    }
    //Checks if it hits a wall and if so it will be stunned
    void ramVision()
    {
        if (atWall)
        {
            StopAllCoroutines();
            eAnimator.SetBool("Ram", false);
            Debug.Log("Stunned");
            states[] stunState = { states.patrol };
            StartCoroutine(waitTheseSeconds(2f, stunState));
            atWall = false;
        }
    }

   void shootStraight(float timeToWait, int numberOf, float inbetweenShootTime)
    {
        states[] goTo = { states.shoot };
        if(!wait)
        {
            eAnimator.SetTrigger("Shoot");
            StartCoroutine(shootMultiple(numberOf, inbetweenShootTime));
            StartCoroutine(waitTheseSeconds(timeToWait, goTo));
        } else
        {
            setAnimation(calcuatePlayerDirection());
        }
    }
   /*
    void shootCircular(float timeToWait, int numberOf, float inbetweenShootTime, float addAngle)
    {
        if (!wait)
        {
            eAnimator.SetTrigger("Shoot");
            StartCoroutine(shootCircle(numberOf, inbetweenShootTime, addAngle));
            StartCoroutine(waitTheseSeconds(timeToWait, shootSwitchChance));
        }
        else
        {
            float angle = Mathf.Asin((playerTransform.position.y - currentTransform.position.y) / Vector2.Distance(playerTransform.position, currentTransform.position));
            float angle2 = Mathf.Acos((playerTransform.position.x - currentTransform.position.x) / Vector2.Distance(playerTransform.position, currentTransform.position));

            Vector2 directionPro = new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle));
            setAnimation(directionPro);
        }
    }*/
    IEnumerator shootMultiple(int number, float time)
    {
        for (int i = 0; i < number; i++)
        {
            float angle = Mathf.Asin((playerTransform.position.y - currentTransform.position.y) / Vector2.Distance(playerTransform.position, currentTransform.position));
            float angle2 = Mathf.Acos((playerTransform.position.x - currentTransform.position.x) / Vector2.Distance(playerTransform.position, currentTransform.position));

            Vector2 directionPro = new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle));
            setAnimation(directionPro);
            goToPoint = (Vector2)currentTransform.position + directionPro;
            GameObject pro = Instantiate(Projectile, goToPoint, Quaternion.identity);
            pro.GetComponent<Bullet>().setDirection(directionPro);
            yield return new WaitForSeconds(time);
        }
    }

    IEnumerator shootCircle(int number, float time, float addAngle)
    {
        float angle = Mathf.Asin((playerTransform.position.y - currentTransform.position.y) / Vector2.Distance(playerTransform.position, currentTransform.position));
        float angle2 = Mathf.Acos((playerTransform.position.x - currentTransform.position.x) / Vector2.Distance(playerTransform.position, currentTransform.position));
        for (int i = 0; i < number; i++)
        {
            Vector2 directionPro = new Vector2(Mathf.Cos(angle2 + (i * addAngle)), Mathf.Sin(angle + (i * addAngle)));
            setAnimation(directionPro);
            goToPoint = (Vector2)currentTransform.position + directionPro;
            GameObject pro = Instantiate(Projectile, goToPoint, Quaternion.identity);
            pro.GetComponent<Bullet>().setDirection(directionPro);

            yield return new WaitForSeconds(time);
        }
    }

    void randomAttack(states[] whatStates)
    {
        int randomIndex = Random.Range(0, whatStates.Length);
        eState = whatStates[randomIndex];

    }


    //Wait for these seconds until the action is over
    //Will choose between a random endState to switch to
    IEnumerator waitTheseSeconds(float timeToWait, states[] endStates)
    {
        wait = true;
        yield return new WaitForSeconds(timeToWait);
        wait = false;
        int index = Random.Range(0, endStates.Length - 1);
        eState = endStates[index];
    }

    private Vector2 calcuatePlayerDirection()
    {
        float angle = Mathf.Asin((playerTransform.position.y - currentTransform.position.y) / Vector2.Distance(playerTransform.position, currentTransform.position));
        float angle2 = Mathf.Acos((playerTransform.position.x - currentTransform.position.x) / Vector2.Distance(playerTransform.position, currentTransform.position));

       return new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle));
    }
    public void movement(float XDir, float YDir, float force)
    {
        eRigid.AddForce(new Vector2(XDir * force * Time.fixedDeltaTime, YDir * force * Time.fixedDeltaTime));
    }
    private void setAnimation(Vector2 velocity)
    {
        //Changes the velocity to be either -1, 0, or 1
        Vector2 clampedVelocity = new Vector2(Mathf.RoundToInt(Mathf.Clamp(velocity.x, -1, 1)), Mathf.RoundToInt(Mathf.Clamp(velocity.y, -1, 1)));
        switch (clampedVelocity.ToString())
        {
            case "(1.00, 1.00)":
                eAnimator.SetTrigger("NE");
                break;
            case "(1.00, 0.00)":
                eAnimator.SetTrigger("E");
                break;
            case "(0.00, 1.00)":
                eAnimator.SetTrigger("N");
                break;
            case "(-1.00, -1.00)":
                eAnimator.SetTrigger("SW");
                break;
            case "(-1.00, 0.00)":
                eAnimator.SetTrigger("W");
                break;
            case "(0.00, -1.00)":
                eAnimator.SetTrigger("S");
                break;
            case "(-1.00, 1.00)":
                eAnimator.SetTrigger("NW");
                break;
            case "(1.00, -1.00)":
                eAnimator.SetTrigger("SE");
                break;
            case "(0.00, 0.00)":
                eAnimator.SetTrigger("Idle");
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (type.Equals("bomb") && eState == states.follow)
        {
            eState = states.slam;
        }
        

        switch(collision.gameObject.tag)
        {
            case "Wall":
                atWall = true;
                break;
            case "Tower":
                if(eState == states.ram)
                {
                    CharacterStats colObj = collision.gameObject.GetComponent<CharacterStats>();
                    int healthleft = this.gameObject.GetComponent<CharacterStats>().changeHealth(colObj.getBasicDamage());
                    
                    if (healthleft <= 0)
                    {
                        death();
                    }
                }

                break;
            
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Splat":
                if (eState == states.ram)
                {
                    CharacterStats colObj = collision.gameObject.GetComponent<CharacterStats>();
                    int healthleft = this.gameObject.GetComponent<CharacterStats>().changeHealth(colObj.getBasicDamage());
                    if (healthleft <= 0)
                    {
                        death();
                    }
                }
                break;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            atWall = false;
        }
    }
    
    private void death()
    {
        Instantiate(enemyData.getDeathSpawn(), currentTransform.position, Quaternion.identity);
        eAnimator.SetTrigger("Death");
        Destroy(this.gameObject, 3f);
        this.GetComponent<enemyAI>().enabled = false;
    }
    private void setUpTarget()
    {
        target = Instantiate(Target);
    }
    private void updateTarget()
    {
        target.GetComponent<Transform>().position = goToPoint;
    }

    private void OnDrawGizmos()
    {
        if(currentTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentTransform.position, maxVision);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentTransform.position, rangeVision);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTransform.position, closeVision);
        }
    }
}
