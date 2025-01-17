using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviourPun
{
    public GameObject cannonBall;
    public GameObject cannonTarget;

    public GameObject gameManager;
    bool isSolo;

    RaycastHit hit;
    Vector3 tp;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Timer(false));
        if (gameManager.GetComponent<GameManager>().isSolo)
        {
            isSolo = true;
        }
        else
        {
            isSolo= false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        cannonTarget.transform.Rotate(new Vector3(0, 300f, 0) * Time.deltaTime);
    }

    IEnumerator Timer(bool isFire)
    {
        if (isSolo)
        {
            if (!isFire)
            {
                //aiming
                SpawnRandom();
                cannonTarget.transform.position = new Vector3(tp.x, tp.y + 0.01f, tp.z); //뽑은 타일위로 타겟을 옮긴다
                isFire = true;
            }
            else
            {
                //fire
                cannonBall.transform.position = cannonTarget.transform.position + new Vector3(0, 10, 0);
                isFire = false;
            }
        }
        else
        {
            if(PhotonNetwork.IsMasterClient)
            {
                
                if (!isFire)
                {
                    //aiming
                    SpawnRandom();
                    cannonTarget.transform.position = new Vector3(tp.x, tp.y + 0.01f, tp.z); //뽑은 타일위로 타겟을 옮긴다
                    photonView.RPC("MovePos", RpcTarget.All, false, cannonTarget.transform.position);
                    isFire = true;
                }
                else
                {
                    //fire
                    cannonBall.transform.position = cannonTarget.transform.position + new Vector3(0, 10, 0);
                    photonView.RPC("MovePos", RpcTarget.All, true, cannonBall.transform.position);
                    isFire = false;
                }
            }
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(Timer(isFire));
    }
    public void SpawnRandom()
    {
        Transform PlaneTp = MapCreator.planeTiles.transform.GetChild(Random.Range(0, MapCreator.planeTiles.transform.childCount)); // 타일중하나 뽑음
        tp = new Vector3(PlaneTp.position.x, PlaneTp.position.y, PlaneTp.position.z); //뽑은 타일위로 타겟을 옮긴다
    }

    [PunRPC]
    public void MovePos(bool isFire, Vector3 tp)
    {
        if(!isFire)
        {
            cannonTarget.transform.position = tp;
        }
        else
        {
            cannonBall.transform.position = tp;
        }
    }
}

