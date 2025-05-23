using UnityEngine;
using UnityEngine.Events;

public class UserInput : MonoBehaviour
{
    public UnityAction OnMouseDown;
    public UnityAction OnMouseMove;
    public UnityAction OnMouseUp;
    private bool isMouseDown;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            OnMouseDown?.Invoke();
        }

        if(isMouseDown)
        {
            OnMouseMove?.Invoke();
        }

        if(Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            OnMouseUp?.Invoke();
        }
    }

}
