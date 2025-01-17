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
            //player.point = 0;//���� �ʱ�ȭ

            if (player.whoHitMe != null)//������� ���� �߰�
            {
                player.whoHitMe.GetComponent<Player>().point += 50;
                player.whoHitMe = null;
            }

        }


        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<EnemyAI>() == null)//ai�� �ƴ϶� �÷��̾ ��������
        {
            other.GetComponent<Player>().isDie = true;

        }
        else if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Barricade"))//ai�� �ٸ����̵� �������� �����
        {
            other.GetComponent<Rigidbody>().isKinematic = true;

            Transform targetPlane = MapCreator.planeTiles.transform.GetChild(Random.Range(0, MapCreator.planeTiles.transform.childCount)); // Ÿ�����ϳ� ����
            Vector3 tp = new Vector3(targetPlane.position.x, targetPlane.position.y + 2f, targetPlane.position.z); //���� Ÿ������ Ÿ���� �ű��

            other.gameObject.transform.position = tp;
            other.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            other.GetComponent<Rigidbody>().isKinematic = false;//���ư��� ���� �ʱ�ȭ�ϱ����ؼ� ��� �״ٲ�

            if (other.TryGetComponent<Player>(out Player ai))
            {
                ai.point = 0;//���� �ʱ�ȭ
            }
        }
    }

}
