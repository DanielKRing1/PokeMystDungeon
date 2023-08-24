using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var src = GameObject.Find("Derived");
        var target = GameObject.Find("Base");
        ComponentUtils.Copy<Base>(src, target);
    }

    // Update is called once per frame
    void Update() { }
}
