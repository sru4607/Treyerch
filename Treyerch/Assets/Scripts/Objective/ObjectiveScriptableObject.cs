using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Objective Data", menuName = "Objective Data", order = 51)]
public class ObjectiveScriptableObject : ScriptableObject
{
    [SerializeField]
    private string _objectiveTitle;
    [SerializeField]
    private string _description;
    [SerializeField]
    private Sprite _icon;
    [SerializeField]
    private int _sortValue;

    public string objectiveTitle {get{return _objectiveTitle;}}
    public string description {get{return _description;}}
    public Sprite icon {get{return _icon;}}
    public int sortVaule {get{return _sortValue;}}


    
}
