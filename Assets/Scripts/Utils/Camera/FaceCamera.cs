using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{
    private GameObject m_Camera;

    void Start()
    {
        this.m_Camera = GameObject.FindWithTag("MainCamera");
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void FixedUpdate()
    {
        this.transform.LookAt(
            transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up
        );
    }
}
