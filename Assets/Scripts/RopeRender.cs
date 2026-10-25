using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeRender : MonoBehaviour
{
    LineRenderer _lineRender;
    Transform playersTransform;
    void Awake()
    {
        _lineRender = gameObject.GetComponent<LineRenderer>();
        playersTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            _lineRender.SetPosition(0, playersTransform.position);
            _lineRender.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            _lineRender.enabled = true;
        } else
        {
            _lineRender.enabled = false;
        }
    }
}
