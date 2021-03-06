﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MRRoomController : MonoBehaviour {

    public DoorController hiddenDoor;
    public DoorController bookDoor;
    public DoorController studyDoor;
    public DoorController exitDoor;
    public LeverController elevatorController;
    private bool[] gallerySwitchesDown = { false, false, false, false, false, false };
    private bool[] platesPressed = { false, false, false, false, false, false, false };

    // Use this for initialization
    void Start () {
        if (!FindObjectOfType<LoadingScreenCanvas>())
        {
            SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        }
        else
        {
            FindObjectOfType<LoadingScreenCanvas>().FinishLoading();
        }

    }

    public void GallerySwitchDown(int id)
    {
        gallerySwitchesDown[id] = true;
        CheckSwitches();
    }

    public void GallerySwitchUp(int id)
    {
        gallerySwitchesDown[id] = false;
        CheckSwitches();
    }

    private void CheckSwitches()
    {
        if (!gallerySwitchesDown[0] && gallerySwitchesDown[1] && !gallerySwitchesDown[2] && gallerySwitchesDown[3] && gallerySwitchesDown[4] && !gallerySwitchesDown[5])
        {
            bookDoor.Open();
            studyDoor.Close();
        }
        else if (!gallerySwitchesDown[0] && gallerySwitchesDown[1] && gallerySwitchesDown[2] && !gallerySwitchesDown[3] && gallerySwitchesDown[4] && !gallerySwitchesDown[5])
        {
            bookDoor.Close();
            studyDoor.Open();
        }
        else
        {
            bookDoor.Close();
            studyDoor.Close();
        }
    }

    public void GalleryPlatePressed()
    {
        hiddenDoor.Open();
    }

    public void GalleryPlateReleased()
    {
        hiddenDoor.Close();
    }

    public void PlatePressed(int id)
    {
        platesPressed[id] = true;
        if (!platesPressed[0] && platesPressed[1] && !platesPressed[2] && !platesPressed[3] && platesPressed[4] && !platesPressed[5] && platesPressed[6])
        {
            exitDoor.Open();
        }
        else
        {
            exitDoor.Close();
        }
    }

    public void PlateReleased(int id)
    {
        platesPressed[id] = false;
        if (!platesPressed[0] && platesPressed[1] && !platesPressed[2] && !platesPressed[3] && platesPressed[4] && !platesPressed[5] && platesPressed[6])
        {
            exitDoor.Open();
        }
        else
        {
            exitDoor.Close();
        }
    }

    public void UnlockElevator()
    {
        elevatorController.SetCanUse(true);
    }
}
