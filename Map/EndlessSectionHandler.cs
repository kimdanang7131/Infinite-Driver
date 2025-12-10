using UnityEngine;

public class EndlessSectionHandler : MonoBehaviour
{
    public MapType mapType; 
    public PropSetting[] propSetting; 
    public GameObject ground;

    public void MoveGround()
    {
        ground.transform.localPosition = new Vector3(0, -0.1f, 0);  
    }

    public void ReturnToPool()
    {
        foreach(PropSetting propSet in propSetting)
        {
            propSet.ReturnToPool();
        }
        PoolManager.poolInstance.ReturnMapToPool(gameObject , mapType);
    }
}
