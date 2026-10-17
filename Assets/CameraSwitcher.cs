using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Transform viewPoint1;  // 視点1
    [SerializeField] private Transform viewPoint2;  // 視点2

    private int currentView = 0;

    void Update()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            SwitchView();
        }
    }

    void SwitchView()
    {
        currentView = 1 - currentView; // 0→1、1→0 に切り替え

        if (currentView == 0)
        {
            MoveToView(viewPoint1);
        }
        else
        {
            MoveToView(viewPoint2);
        }
    }

    void MoveToView(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
