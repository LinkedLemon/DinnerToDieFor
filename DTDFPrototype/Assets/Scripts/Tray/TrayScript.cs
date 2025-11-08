using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TrayScript : MonoBehaviour
{
    [SerializeField] private GameObject[] _foodPlacement = new GameObject[5];

    private List<GameObject> _food = new List<GameObject>();

    //Spawn food onto the tray
    public void SpawnFood(List<GameObject> listOfFood)
    {
        if (listOfFood.Count < 1)
        {
            Debug.LogError("The given list did not have any food");
            return;
        }

        for (int i = 0; i < listOfFood.Count; i++)
        {
            FoodScript _fs = listOfFood[i].GetComponent<FoodScript>();
            if (_fs == null)//Checking if it does have a script
            {
                Debug.LogWarning("Food number " + i + "does not have script, and cannot be given garnishes"); 
            }
            GameObject foodGM = Instantiate(_fs, _foodPlacement[i].transform);
            _food.Add(foodGM);
        }
    }
}
