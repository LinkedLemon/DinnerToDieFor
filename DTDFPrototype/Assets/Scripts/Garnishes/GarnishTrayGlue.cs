using UnityEngine;

public class GarnishTrayGlue : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Tray"))
        {
            gameObject.transform.SetParent(collision.gameObject.transform, true);
        }
    }
}
