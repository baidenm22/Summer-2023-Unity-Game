using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orb : MonoBehaviour
{
    bool move = false;
    [SerializeField] Vector2[] positions;
    [SerializeField] string color;
    int index = 0;
    [SerializeField] Transform orbTransform;
    public static event Action<string> onOrbFinish;
    [SerializeField] int charge; 


    // Start is called before the first frame update
    void Start()
    {
        index = positions.Length-1;
    }

    // Update is called once per frame
    void Update()
    {
        switch (charge)
        {
            case >= 100:
                move = true;
                break;
        }

        switch (move)
        {
            case true:
                moveTowardPoint();
                break;
            case false:

                break;
        }
    }

    void moveTowardPoint()
    {
        if(Vector2.Distance(positions[index],orbTransform.position) > 0.1f)
        {
            float angle = Mathf.Asin((positions[index].y - orbTransform.position.y) / Vector2.Distance(positions[index], orbTransform.position));
            float angle2 = Mathf.Acos((positions[index].x - orbTransform.position.x) / Vector2.Distance(positions[index], orbTransform.position));
            orbTransform.Translate(new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle)) * 0.01f, Space.Self);
        } else if(index != 0)
        {
            index -= 1;
            move = false;
            charge = 0;
        } else
        {
            onOrbFinish?.Invoke(color);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       switch(collision.tag)
       {
            case "Splat":
                charge += 10;
                break;
       }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
