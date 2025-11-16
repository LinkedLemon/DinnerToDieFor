using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetSO", menuName = "Scriptable Objects/TargetSO")]
public class TargetSO : ScriptableObject
{
    public string _hint;
    public List<EspionageEnums.EspionageType> _success;
    public EspionageEnums.EspionageGoal _goal;
    public List<EspionageEnums.EspionageType> _fail;
}
