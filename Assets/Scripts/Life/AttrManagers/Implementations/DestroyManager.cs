using UnityEngine;

public class DestroyManager : MonoBehaviour
{
    public void Destroy()
    {
        // IDestroySensitive[] dss = this.GetComponents<IDestroySensitive>();

        // foreach (IDestroySensitive ds in dss)
        // {
        //     ds.OnDestroy();
        // }

        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
