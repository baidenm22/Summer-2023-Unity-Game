using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Animator cameraAnimator;
    Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody2D cameraRigid;
    Animator movingCameraAnimator;


    void OnEnable()
    {
        CharcterController.onHurt += shake;
    }
    void OnDisable()
    {
        CharcterController.onHurt += shake;
    }
    void Start()
    {
        cameraAnimator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
        cameraTransform = gameObject.GetComponent<Transform>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        movingCameraAnimator = gameObject.GetComponent<Animator>();
        cameraRigid = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            switch(playerToCamera())
            {
                case > 4:
                    cameraAnimator.SetBool("Grow3", true);
                    cameraAnimator.SetBool("Shrink", false);
                    break;

                case > 3:
                    cameraAnimator.SetBool("Grow2", true);
                    cameraAnimator.SetBool("Shrink", false);
                    break;

                case > 1:
                    cameraAnimator.SetBool("Grow1", true);
                    cameraAnimator.SetBool("Shrink", false);
                    break;
            }
        } else if (Input.GetMouseButtonUp(0))
        {
            cameraAnimator.SetBool("Grow3", false);
            cameraAnimator.SetBool("Grow2", false);
            cameraAnimator.SetBool("Grow1", false);
            cameraAnimator.SetBool("Shrink", true);
        } else
        {
            cameraRigid.AddForce(cameraToPlayer());
        }
    }

    float playerToCamera()
    {
        return Vector2.Distance(cameraTransform.position, playerTransform.position);
    }

    Vector2 cameraToPlayer()
    {
        return new Vector2(Mathf.Floor(-cameraTransform.position.x + playerTransform.position.x) * Time.deltaTime * 1000, Mathf.Floor(-cameraTransform.position.y + playerTransform.position.y)*Time.deltaTime*1000);
    }

    public void shake()
    {
        movingCameraAnimator.SetTrigger("Shake");
    }
}
