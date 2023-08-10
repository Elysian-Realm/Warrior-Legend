using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class Sign : MonoBehaviour
{

    public Transform playerTrans;
    private PlayerInputControl playerInput;
    private Animator animator;
    public GameObject signSprite;
    private IInteractable targetItem;
    private bool canPress;

    private void Awake()
    {
        animator = signSprite.GetComponent<Animator>();
        playerInput = new PlayerInputControl();
        playerInput.Enable();
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (canPress)
        {
            targetItem.TriggerAction(OnInteractDone);
        }
    }

    private void OnInteractDone()
    {
        canPress = false;
        targetItem = null;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;
            switch (d.device)
            {
                case Keyboard:
                    animator.Play("keyboard");
                    break;
                case DualShockGamepad:
                    animator.Play("ps");
                    break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
        targetItem = null;
    }
}
