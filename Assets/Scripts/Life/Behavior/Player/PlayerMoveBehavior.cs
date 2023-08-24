using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MoveBehavior
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update() { }

    protected override Vector3 GetMoveDirection(Observations obs)
    {
        int xAxis = Input.GetKeyDown(KeyCode.LeftArrow)
            ? -1
            : Input.GetKeyDown(KeyCode.RightArrow)
                ? 1
                : 0;
        int yAxis = Input.GetKeyDown(KeyCode.DownArrow)
            ? -1
            : Input.GetKeyDown(KeyCode.UpArrow)
                ? 1
                : 0;

        return new Vector3(xAxis, 0, yAxis).normalized;
    }
}
