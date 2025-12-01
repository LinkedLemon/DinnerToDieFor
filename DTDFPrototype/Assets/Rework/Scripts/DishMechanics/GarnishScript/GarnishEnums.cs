using UnityEngine;

public enum GarnishAddResult
{
    Success_Preferred,
    Success_Meh,
    Failure_BadGarnish,
    Failure_Allergy,
    Failure_DuplicateOrFull
}

public class GarnishEnums : MonoBehaviour
{
    public enum GarnishType
    {
        none,cherry,olive,paprika,sword,thyme
    }

}
