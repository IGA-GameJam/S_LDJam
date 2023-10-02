using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class M_ButtonManager : MonoBehaviour
{
    public O_ButtonBinder[] buttonSequance;
    public bool isHorizontal;
    private Vector2 dPadDirection;
    private bool isOnMove;
    private int currentButtonIndex = -1;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnPressA(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            buttonSequance[currentButtonIndex].TriggerButtonEffect();
    }

    public void OnPressB(InputAction.CallbackContext context)
    {

    }

    public void OnPressDPad(InputAction.CallbackContext context)
    {
        dPadDirection = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Started) DPadButtonSwitch();
    }

    private void DPadButtonSwitch()
    {
        if (!isOnMove && dPadDirection != Vector2.zero)
        {
            if (isHorizontal && dPadDirection.x != 0)
            {
                if (currentButtonIndex >= 0) buttonSequance[currentButtonIndex].TriggerOnDeselected();

                if (dPadDirection.x > 0) currentButtonIndex++;
                else currentButtonIndex--;

                if (currentButtonIndex >= buttonSequance.Length) currentButtonIndex = 0;
                if (currentButtonIndex < 0) currentButtonIndex = buttonSequance.Length - 1;

                buttonSequance[currentButtonIndex].TriggerOnHovering();
            }
            if(!isHorizontal && dPadDirection.y != 0)
            {
                if (currentButtonIndex >= 0) buttonSequance[currentButtonIndex].TriggerOnDeselected();

                if (dPadDirection.y > 0) currentButtonIndex++;
                else currentButtonIndex--;

                if (currentButtonIndex >= buttonSequance.Length) currentButtonIndex = 0;
                if (currentButtonIndex < 0) currentButtonIndex = buttonSequance.Length - 1;

                buttonSequance[currentButtonIndex].TriggerOnHovering();
            }
        }
    }
}
