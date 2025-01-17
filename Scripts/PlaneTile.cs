using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlaneTile : MonoBehaviourPun
{
    public bool isBreak;
    bool isSpin;
    Rigidbody rb;
    MeshCollider mc;
    public Vector3 originVector3;


    // Start is called before the first frame update
    void Awake()
    {
        if(transform.parent == null)
        {
            transform.parent = MapCreator.planeTiles.transform;
        }
        rb = GetComponent<Rigidbody>();
        mc = GetComponent<MeshCollider>();
        originVector3 = transform.position;
        isBreak = false;
        isSpin = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpin)
        {
            rb.AddForce(new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10)), ForceMode.Impulse);//흩날리는 이펙트
            transform.Rotate(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100));
        }

        if (isBreak)
        {
            mc.enabled = false;
            rb.isKinematic = false;
            gameObject.transform.parent = null;//부모에서빼고 비활성화
            StartCoroutine(ReBuildTile());
            isSpin = true;
            
            if (photonView != null)
            {
                if (photonView.IsMine)
                {
                    photonView.RPC("BreakBuffer", RpcTarget.OthersBuffered);
                }
            }
            isBreak = false;
        }



    }

    IEnumerator ReBuildTile()
    {
        yield return new WaitForSeconds(40f);
        ReBuild();

        if (photonView != null)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID);
                photonView.RPC("ReBuild", RpcTarget.OthersBuffered);
            }
        }
    }

    [PunRPC]
    public void BreakBuffer()
    {
        isBreak = true;
    }

    [PunRPC]
    public void ReBuild()
    {
        rb.isKinematic = true;
        mc.enabled = true;
        transform.position = originVector3;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.parent = MapCreator.planeTiles.transform;
        isSpin = false;
        StopAllCoroutines();

    }
}
