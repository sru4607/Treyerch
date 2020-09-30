using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class DropZoneObject : MonoBehaviour
{
    public string objectID;
    public string ObjectID { get { return objectID; } }

    [ReadOnly]
    public bool ready = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(.5f);
        ready = true;
    }
}
