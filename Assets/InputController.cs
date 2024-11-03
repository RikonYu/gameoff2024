using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    private bool isRightMouseDown = false; 
    private void Update()
    {
        HandleMovementInput();

        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;

            UIController.Instance.ShowCone();
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
            UIController.Instance.HideCone();
        }
    }

    private void HandleMovementInput()
    {
        if (isRightMouseDown)
            return;

        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveZ = Input.GetAxisRaw("Vertical");   

        Vector3 moveDirection = new Vector3(moveX, moveZ,0).normalized;

        if (moveDirection.magnitude == 0)
            return;

        float currentSpeed = Consts.PlayerMoveSpeed;

        if (Input.GetKey(KeyCode.Z))
            currentSpeed *= Consts.PlayerCrouchSpeedFactor;

        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
    }

}