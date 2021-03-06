﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingDissapear : Photon.PunBehaviour, IPunObservable
{

    public List<GameObject> pingElements;
    bool dissapear;
    float timer;
    public float dissapearTime = 1f;
    public float scaleFactor = 0.25f;
    public PingIndicator indicator;
    public Color firstPlayerColor;
    public Material firstPlayerMaterial;
    public Color secondPlayerColor;
    public LineRenderer line;
    public Material secondPlayerMaterial;
    public bool debugSecondPlayer;
    public Vector3 lineStartPosition;

    public void OnEnable()
    {
        if (photonView.isMine)
        {
            photonView.RPC("RpcShowPing", PhotonTargets.All);
        }
    }

    private void SetColor()
    {
        if (!PhotonNetwork.isMasterClient && !debugSecondPlayer)
        {
            debugSecondPlayer = true;
        }
        dissapear = true;
    }

    void Update()
    {
        if (dissapear)
        {
            timer += Time.deltaTime;
            foreach (GameObject go in pingElements)
            {
                go.GetComponent<Renderer>().material.SetFloat("_Alpha", 1 - timer / dissapearTime);
            }
            line.material.SetFloat("_Alpha", 1 - timer / dissapearTime);
            transform.localScale = new Vector3(timer / dissapearTime * scaleFactor, timer / dissapearTime * scaleFactor, timer / dissapearTime * scaleFactor);
            if (timer / dissapearTime >= 1)
            {
                foreach (GameObject go in pingElements)
                {
                    go.GetComponent<Renderer>().material.SetFloat("_Alpha", 0);
                }
                dissapear = false;
                if (photonView.isMine)
                    PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    [PunRPC]
    void RpcShowPing()
    {
        Show();
    }

    public void Show()
    {
        if (!photonView.isMine)
        {
            line.SetPosition(0, lineStartPosition);
            line.SetPosition(1, transform.position);
        }
        if (photonView.owner.IsMasterClient)
        {
            foreach (GameObject go in pingElements)
            {
                go.GetComponent<Renderer>().material = firstPlayerMaterial;
            }
            line.material = firstPlayerMaterial;
        }
        else
        {
            foreach (GameObject go in pingElements)
            {
                go.GetComponent<Renderer>().material = secondPlayerMaterial;
            }
            line.material = secondPlayerMaterial;
        }
        foreach (PingIndicator pi in FindObjectsOfType<PingIndicator>())
        {
            if (pi.GetComponent<PhotonView>().owner != photonView.owner)
            {
                pi.ping = transform;
                pi.StartShowing(dissapearTime, line.material.color);
            }
        }
        timer = 0;
        foreach (GameObject go in pingElements)
        {
            go.GetComponent<Renderer>().material.SetFloat("_Alpha", 1);
        }
        line.material.SetFloat("_Alpha", 1);
        transform.localScale = Vector3.zero;
        dissapear = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(lineStartPosition);
        }
        else
        {
            lineStartPosition = (Vector3)stream.ReceiveNext();
            if (lineStartPosition != Vector3.zero && !photonView.isMine)
            {
                if (line.GetPosition(0) != lineStartPosition)
                {
                    line.SetPosition(0, lineStartPosition);
                    line.enabled = true;
                }
            }
        }
    }
}
