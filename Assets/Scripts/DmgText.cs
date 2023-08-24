using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgText : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        this.transform.position = new Vector3(
            this.transform.position.x,
            this.transform.position.y + 1,
            this.transform.position.z
        );

        await Task.Delay(500);
        Destroy(this.gameObject);
    }
}
