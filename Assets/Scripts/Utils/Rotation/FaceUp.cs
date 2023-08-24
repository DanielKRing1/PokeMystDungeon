using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUp : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.LookAt(
            transform.position + this.transform.rotation * Vector3.up,
            this.transform.rotation * Vector3.right
        );
        // this.transform.rotation = this.transform.rotation * Vector3.up;
    }
}
