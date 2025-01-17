using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class Bullet : MonoBehaviourPun
{
    public GameObject bulletMaster;
    public int speed;
    public int power;

    // Start is called before the first frame update
    void Start()
    {
        speed = 20;
        power = 50;

        this.GetComponent<MeshRenderer>().material = DataController.instance.mtMissile[bulletMaster.GetComponent<Player>().bulletSkinNum];
        this.gameObject.GetComponent<BoxCollider>().enabled = true;

        StartCoroutine(DestroyTimer());
    }


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);

        if (bulletMaster == null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == bulletMaster)//������ ������ �ȵǴϱ�
        {
            return;
        }

        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Barricade"))//�ٸ� �÷��̾ �°ų� �ٸ����̵忡 ������
        {
            bulletMaster.GetComponent<Player>().hitSFX.Play();
            Vector3 push = (other.transform.position - transform.position).normalized;
            int adPower = other.CompareTag("Player") ? power : power * 3;
            other.gameObject.GetComponent<Rigidbody>().AddForce(push * adPower, ForceMode.Impulse);//�ڷ� �и�AddRelativeForce

            if(other.TryGetComponent<Player>(out Player player))
            {
                other.gameObject.GetComponent<Player>().whoHitMe = bulletMaster;
            }
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
