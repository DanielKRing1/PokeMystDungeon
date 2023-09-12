using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = new Vector3(
            this.player.transform.position.x,
            this.player.transform.position.y + 30,
            this.player.transform.position.z - 10
        );
    }
}
