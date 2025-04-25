using UnityEngine;

public class ArkanoidBlock : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            gameObject.SetActive(false);
        }
    }
}
