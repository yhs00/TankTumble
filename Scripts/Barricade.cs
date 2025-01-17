using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Barricade : MonoBehaviourPun, IPunObservable
{
    Vector3 receivePos;
    Quaternion receiveRot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView == null)
        {
            return;
        }

        if (photonView.IsMine)
        {
        }
        else
        {
            float t = Mathf.Clamp(20f * Time.deltaTime, 0f, 0.99f);
            transform.position = Vector3.Lerp(transform.position, receivePos, 20 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, t);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //데이터 내거는 보내고 다른건 받고
        if (stream.IsWriting)//isMine == true
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)//isMine == false
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }

    }
}
