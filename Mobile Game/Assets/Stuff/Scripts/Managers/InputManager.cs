using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Settings")]
    public bool tapped;
    public bool doubleTapped;
    public bool touching;
    public bool holding;
    public int currentTaps;
    public float tapTime;
    private float touchingTime;

    // Update is called once per frame
    void Update()
    {
        tapped = false;
        doubleTapped = false;
        touching = false;

        if (Input.GetKeyDown(KeyCode.Space)) tapped = true;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                tapped = touch.tapCount == 1;
                doubleTapped = touch.tapCount == 2;
            }
            else if (touchingTime > tapTime && !touching)
            {
                touching = true;
            }
            else if (touchingTime > tapTime) touching = true;
        }
        if (Input.touchCount == 0)
        {
            touchingTime = 0;
            holding = false;
        }
        else
        {
            touchingTime += Time.deltaTime;
            holding = true;
        }
    }

}
