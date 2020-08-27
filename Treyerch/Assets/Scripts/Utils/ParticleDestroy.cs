using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
    public ParticleSystem particleSystem;

    // Update is called once per frame
    void Update()
    {
        if(!particleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
