using UnityEngine;

public class Park : MonoBehaviour
{
    public Route route;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem fx;
    private ParticleSystem.MainModule fxMainModule;

    private void Start()
    {
        fxMainModule = fx.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Car car))
        {
            if(car.route == route)
            {
                Game.Instance.OnCarEntersPark?.Invoke(route);
                StartFX();
            }
        }
    }

    private void StartFX()
    {
        fx.Play();
    }

    public void SetColor(Color color)
    {
        spriteRenderer.sharedMaterials[0].color = color;
    }
}
