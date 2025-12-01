using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CustomerScriptObject", menuName = "Scriptable Objects/CustomerScriptObject")]
public class CustomerScriptObject : ScriptableObject
{
    public string CustomerName;
    
    [Header("Preferences")]
    public List<GarnishEnums.GarnishType> GoodGarnishes;
    public List<GarnishEnums.GarnishType> MehGarnishes;
    public List<GarnishEnums.GarnishType> BadGarnishes;
    
    [Header("Allergy")]
    public GarnishEnums.GarnishType Allergy;
}