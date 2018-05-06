﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsElevatorController : Photon.PunBehaviour, IPunObservable
{
    public GameObject BottomLeftDoor;
    public GameObject BottomRightDoor;
    public GameObject TopLeftDoor;
    public GameObject TopRightDoor;
    public GameObject BoxObject;
    public float Height = 4.4f;
    public float MaximumAngle = 500.0f;
    public float DoorsOpeningAngle = 90.0f; // Dotyczy obrotu korbki
    public float OpenedDoorsAngle = 120.0f;  // Dotyczy obrotu drzwi
    //private float boxStartingY;
    private float previousAngle;
    private float boxMovingAngle;
    private float topDoorsRotationStartingAngle;

	// Use this for initialization
	void Start ()
    {
        //boxStartingY = BoxObject.transform.position.y;
        boxMovingAngle = MaximumAngle - 2 * DoorsOpeningAngle;
        topDoorsRotationStartingAngle = MaximumAngle - DoorsOpeningAngle;
    }
	
	// Update is called once per frame
	void Update () {}

    public void UpdatePosition(float angle)
    {
        float angleDiff = Mathf.Abs(angle - previousAngle);
        float direction = (angle > previousAngle ? 1.0f : -1.0f);

        if (angle > topDoorsRotationStartingAngle)
        {
            if (previousAngle < topDoorsRotationStartingAngle)
            {
                float boxAngleDiff = topDoorsRotationStartingAngle - previousAngle;
                angleDiff -= boxAngleDiff;
                moveBox(boxAngleDiff, direction);
            }
            rotateDoors(TopLeftDoor, TopRightDoor, angleDiff, direction);
        }
        else if (angle > DoorsOpeningAngle)
        {
            if (previousAngle < DoorsOpeningAngle)
            {
                float doorsAngleDiff = DoorsOpeningAngle - previousAngle;
                angleDiff -= doorsAngleDiff;
                rotateDoors(BottomLeftDoor, BottomRightDoor, doorsAngleDiff, -direction);
            }
            else if (previousAngle > topDoorsRotationStartingAngle)
            {
                float doorsAngleDiff = previousAngle - topDoorsRotationStartingAngle;
                angleDiff -= doorsAngleDiff;
                rotateDoors(TopLeftDoor, TopRightDoor, doorsAngleDiff, direction);
            }
            moveBox(angleDiff, direction);
        }
        else if (angle > 0.0f)
        {
            if (previousAngle > DoorsOpeningAngle)
            {
                float boxAngleDiff = previousAngle - DoorsOpeningAngle;
                angleDiff -= boxAngleDiff;
                moveBox(boxAngleDiff, direction);
            }
            rotateDoors(BottomLeftDoor, BottomRightDoor, angleDiff, -direction);
        }

        previousAngle = angle;
    }

    private void rotateDoors(GameObject leftDoor, GameObject rightDoor, float angleDiff, float direction)
    {
        float angleIncrement = (angleDiff * OpenedDoorsAngle / DoorsOpeningAngle) * direction;
        Vector3 currentRot = leftDoor.transform.rotation.eulerAngles;
        float y = currentRot.y + angleIncrement;
        leftDoor.transform.rotation = Quaternion.Euler(currentRot.x, y, currentRot.z);
        currentRot = rightDoor.transform.rotation.eulerAngles;
        y = currentRot.y + (angleIncrement * (-1.0f));
        rightDoor.transform.rotation = Quaternion.Euler(currentRot.x, y, currentRot.z);
    }

    private void moveBox(float angleDiff, float direction)
    {
        float distanceIncrement = (angleDiff * Height / boxMovingAngle) * direction;
        Vector3 currentPos = BoxObject.transform.position;
        float y = currentPos.y + distanceIncrement;
        BoxObject.transform.position = new Vector3(currentPos.x, y, currentPos.z);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(previousAngle);
        }
        else
        {
            previousAngle = (float)stream.ReceiveNext();
        }
    }
}
