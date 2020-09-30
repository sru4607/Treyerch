using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DoTween_Testing : MonoBehaviour
{
    public Vector3[] Positions;
    public float[] timeBetween;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = Positions[Positions.Length - 1];
        Sequence testSequence = DOTween.Sequence();
        if(timeBetween.Length != Positions.Length)
        {
            Debug.Log("Positions Length does not equal time between defaulting to 1 second");
            for(int i = 0; i<Positions.Length; i++){
                testSequence.Append(transform.DOMove(Positions[i], 1));
            }
        }
        else
        {
            for(int i = 0; i<Positions.Length; i++){
                testSequence.Append(transform.DOMove(Positions[i], timeBetween[i]));
            }
        }
        testSequence.SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
