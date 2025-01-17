using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlane : MonoBehaviour
{ 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<Player>(out Player player))
        {
            //player.point = 0;//점수 초기화

            if (player.whoHitMe != null)//쏜애한테 점수 추가
            {
                player.whoHitMe.GetComponent<Player>().point += 50;
                player.whoHitMe = null;
            }

        }


        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<EnemyAI>() == null)//ai가 아니라 플레이어가 떨어지면
        {
            other.GetComponent<Player>().isDie = true;

        }
        else if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Barricade"))//ai나 바리케이드 떨어지면 재생성
        {
            other.GetComponent<Rigidbody>().isKinematic = true;

            Transform targetPlane = MapCreator.planeTiles.transform.GetChild(Random.Range(0, MapCreator.planeTiles.transform.childCount)); // 타일중하나 뽑음
            Vector3 tp = new Vector3(targetPlane.position.x, targetPlane.position.y + 2f, targetPlane.position.z); //뽑은 타일위로 타겟을 옮긴다

            other.gameObject.transform.position = tp;
            other.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            other.GetComponent<Rigidbody>().isKinematic = false;//날아가는 힘을 초기화하기위해서 잠깐 켰다끔

            if (other.TryGetComponent<Player>(out Player ai))
            {
                ai.point = 0;//점수 초기화
            }
        }
    }

}
