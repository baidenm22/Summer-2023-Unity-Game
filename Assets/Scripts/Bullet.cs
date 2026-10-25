using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector2 direction;
    [SerializeField] float speed;
    [SerializeField] int TYPE;

    public void setDirection(Vector2 direction)
    {
        this.direction = direction;
        doLogic();
    }
    void doLogic()
    {
        switch (TYPE)
        {
            case 0:
                GetComponent<Rigidbody2D>().velocity = direction * speed;
                break;
            case 1:
                //Spawns in front of enemy so doesn't know the direction to face
                GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(-direction.x, direction.y));
                break;
        }
       
    }
}
