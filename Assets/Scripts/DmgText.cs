using System;
using System.Threading.Tasks;
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
        try
        {
            if (this != null)
                Destroy(this.gameObject);
        }
        catch (Exception e) { }
    }
}
