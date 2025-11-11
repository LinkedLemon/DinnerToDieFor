using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    [SerializeField] private FoodObject _foodObject;

    private List<EspionageGameObject> _addedEspionage = new List<EspionageGameObject>();
    private List<GarnishGameObject> _addedGarnish = new List<GarnishGameObject>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject _garnishGM = other.gameObject;
        AdditiveScript _additiveScript = _garnishGM.GetComponent<AdditiveScript>();//Getting the script to get the garnish
        if (_additiveScript == null )
        {
            Debug.Log("Non-additive touched food");
            return;
        }
        var _additive = _additiveScript.GiveFoodObject();
        if (_additive is EspionageGameObject)//If the additive is a espionage
        {
            _addedEspionage.Add((EspionageGameObject)_additive);
        }
        else if (_additive is GarnishGameObject)//If the additive is a garnish
        {
            _addedGarnish.Add((GarnishGameObject)_additive);
        }

    }

    public List<EspionageGameObject> GiveEspionages()
    {
        return _addedEspionage;
    }

    public List<GarnishGameObject> GiveGarnishes()
    {
        return _addedGarnish;
    }


    public FoodObject GiveFoodObject()
    {
        return _foodObject;
    }
}
