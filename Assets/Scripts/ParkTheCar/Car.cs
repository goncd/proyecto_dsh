using DG.Tweening;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Route route;
    public Transform bottomTransform;
    public Transform bodyTransform;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] ParticleSystem smokeFX;
    [SerializeField] Rigidbody rb;
    [SerializeField] float danceValue;
    [SerializeField] float durationMultiplier;

    private void Start()
    {
        bodyTransform.DOLocalMoveY(danceValue, .1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.TryGetComponent(out Car otherCar))
        {
            StopDancingAnimation();
            rb.DOKill(false);

            Vector3 hitPoint = collision.contacts[0].point;
            AddExplosionForce(hitPoint);
            smokeFX.Play();

            Game.Instance.OnCarCollision?.Invoke();
        }
    }

    private void AddExplosionForce(Vector3 point)
    {
        rb.AddExplosionForce(25f, point, 2f);
        rb.AddForceAtPosition(Vector3.up * 2f, point, ForceMode.Impulse);
        rb.AddTorque(new Vector3(GetRandomAngle(), GetRandomAngle(), GetRandomAngle()));
    }

    private float GetRandomAngle()
    {
        return Random.value > 0.5f ? 10f : -10f;
    }

    public void Move(Vector3[] path)
    {
        rb.DOLocalPath(path, 2f * durationMultiplier * path.Length).SetLookAt(0.1f, false).SetEase(Ease.Linear);
    }

    public void StopDancingAnimation()
    {
        bodyTransform.DOKill(true);
    }

    public void SetColor(Color color)
    {
        meshRenderer.sharedMaterials[0].color = color;
    }
}
