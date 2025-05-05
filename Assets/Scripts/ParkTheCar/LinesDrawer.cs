using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinesDrawer : MonoBehaviour
{
    [SerializeField] UserInput userInput;
    [SerializeField] int interactableLayer;
    private Line currentLine;
    private Route currentRoute;
    RaycastDetector raycastDetector = new();
    public UnityAction<Route, List<Vector3>> OnParkLinkedToLine;
    public UnityAction<Route> OnBeginDraw;
    public UnityAction OnDraw;
    public UnityAction OnEndDraw;
    private bool isDrawingBlocked = false;

    private void Start()
    {
        userInput.OnMouseDown += OnMouseDownHandler;
        userInput.OnMouseMove += OnMouseMoveHandler;
        userInput.OnMouseUp += OnMouseUpHandler;
    }

    private void OnMouseDownHandler()
    {
        ContactInfo contactInfo = raycastDetector.RayCast(interactableLayer);

        if (contactInfo.contacted)
        {
            bool isCar = contactInfo.collider.TryGetComponent(out Car _car);
            if (isCar && _car.route.isActive)
            {
                Debug.Log("🚗 Coche tocado (desde MouseDown)");
                currentRoute = _car.route;
                currentLine = currentRoute.line;
                currentLine.Init();
                isDrawingBlocked = false; // ← Permite volver a pintar
                OnBeginDraw?.Invoke(currentRoute);
                return;
            }
        }

        if (currentRoute != null)
        {
            if (contactInfo.contacted)
            {
                bool isPark = contactInfo.collider.TryGetComponent(out Park _park);
                if (currentLine.pointsCount < 2 || !isPark)
                    currentLine.Clear();
                else
                    currentRoute.Disactivate();
            }
            else
            {
                currentLine.Clear();
            }
        }

        ResetDrawer();
        OnEndDraw?.Invoke();
    }

    private void OnMouseMoveHandler()
    {
        if (currentRoute != null && !isDrawingBlocked)
        {
            ContactInfo contactInfo = raycastDetector.RayCast(interactableLayer);
            if (contactInfo.contacted)
            {
                Vector3 newPoint = contactInfo.point;

                if (currentLine.lenght >= currentRoute.maxLineLength)
                {
                    currentLine.Clear();
                    isDrawingBlocked = true;
                    OnEndDraw?.Invoke();
                    ResetDrawer();
                    return;
                }

                currentLine.AddPoint(newPoint);
                OnDraw?.Invoke();

                bool isPark = contactInfo.collider.TryGetComponent(out Park _park);
                if (isPark)
                {
                    Route parkRoute = _park.route;
                    if (parkRoute == currentRoute)
                    {
                        currentLine.AddPoint(contactInfo.transform.position);
                        OnParkLinkedToLine?.Invoke(currentRoute, currentLine.points);
                        OnDraw?.Invoke();
                        currentRoute.Disactivate();
                        ResetDrawer();
                    }
                    else
                    {
                        currentLine.Clear();
                    }
                    OnMouseUpHandler();
                }
            }
        }
    }

    private void OnMouseUpHandler()
    {
        ContactInfo contactInfo = raycastDetector.RayCast(interactableLayer);
        Debug.Log($"Raycast hit: {contactInfo.contacted}, Collider: {contactInfo.collider?.name}");

        if (contactInfo.contacted)
        {
            bool isCar = contactInfo.collider.TryGetComponent(out Car _car);

            if (isCar && _car.route.isActive)
            {
                Debug.Log("🚗 Coche tocado");
                currentRoute = _car.route;
                currentLine = currentRoute.line;
                currentLine.Init();
            }
        }
    }


    private void ResetDrawer()
    {
        currentRoute = null;
        currentLine = null;
        isDrawingBlocked = false;
    }
}
