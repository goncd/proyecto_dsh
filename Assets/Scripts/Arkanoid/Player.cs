using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10f;
    private float movement;

    void Update()
    {
        movement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.position += new Vector3(movement, 0, 0);

        if(transform.position.x < -4.75)
        {
            transform.position = new Vector3(-4.75f, transform.position.y, transform.position.z);
        }
        if(transform.position.x > 4.75)
        {
            transform.position = new Vector3(4.75f, transform.position.y, transform.position.z);
        }
    }
}
