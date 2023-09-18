using UnityEngine;

public class DestroyManager : MonoBehaviour
{
    private bool isInitted = false;

    public void Init()
    {
        if (this.isInitted)
            return;
        this.isInitted = true;
    }

    public void Destroy()
    {
        // IDestroySensitive[] dss = this.GetComponents<IDestroySensitive>();

        // foreach (IDestroySensitive ds in dss)
        // {
        //     ds.OnDestroy();
        // }

        // 1. Deactivate GameObject
        this.gameObject.SetActive(false);

        // 2. Prepare to Destroy GameObject (Destroy all DmgEl's)
        // foreach (IDestroySensitive ds in dss)
        // {
        //     await ds.PrepareToDestroy();
        // }

        // 3. Destroy GameObject
        Destroy(this.gameObject);
    }
}
