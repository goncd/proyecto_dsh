using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private bool gameStarted = false;
    private float velocity = 60f;
    private Vector3 lastVelocity;
    private AudioSource audioSource;
    public AudioClip paddleHit;
    public AudioClip blockBreak;
    public int healthPoints = 3;
    public LevelManager levelManager;
    public Transform player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(!gameStarted)
        {
            transform.position = player.position + new Vector3(0, 0.5f, 0);

            if(Input.GetButtonDown("Jump"))
            {
                rb.AddForce(new Vector3(0, 9.8f * velocity, 0));
                gameStarted = true;
            }
        }
        lastVelocity = rb.linearVelocity;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bottom"))
        {
            levelManager.ReduceHealth();
            rb.linearVelocity = Vector3.zero;
            transform.position = new Vector3(0, 3.5f, -0.4f);
            gameStarted = false;
        }        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(gameStarted)
            {
                audioSource.clip = paddleHit;
                audioSource.Play();
            }

            float hit = (transform.position.x - collision.transform.position.x) / (collision.collider.bounds.size.x / 2);
            Vector3 reflectDirection = new Vector3(hit, 1f, 0f).normalized;
            rb.linearVelocity = reflectDirection * lastVelocity.magnitude;
        }
        if(collision.gameObject.CompareTag("Wall"))
        {
            var speed = lastVelocity.magnitude;
            var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = direction * Mathf.Max(speed, 0f);
        }
        if(collision.gameObject.CompareTag("Block"))
        {
            audioSource.clip = blockBreak;
            audioSource.Play();
            levelManager.AddPoints(10);
        }
    }
}
