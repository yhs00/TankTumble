using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyAI : MonoBehaviourPun
{
    public Player player;
    public Fire fire;
    public Vector3 aiTarget;
    public float range;

    float aiRandomMoveX;
    float aiRandomMoveZ;
    public bool aiFire;
    RaycastHit hit;

    public GameObject[] enemys;

    private void Awake()
    {
        player = this.gameObject.GetComponent<Player>();
        fire = this.gameObject.GetComponent<Fire>();
    }


    // Start is called before the first frame update
    void Start()
    {
        range = 15f;
        aiFire = false;

        StartCoroutine(PickTimer());
        StartCoroutine(UpdateTarget());
        StartCoroutine(RandomNameSkin());
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Aim();

    }

    void Aim()
    {
        if (aiTarget != null) 
        {
            fire.target = aiTarget;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void Movement()
    {
        

        if (transform.position.y >= 1)//총알이나 대포에 맞아서 공중에 뜨면 가운데로 가려는 행동
        {
            if (transform.position.x > 0)
            {
                player.movement.x = -0.7f;
            }
            else
            {
                player.movement.x = 0.7f;
            }   

            if (transform.position.z > 0)
            {
                player.movement.z = -0.7f;
            }
            else
            {
                player.movement.z = 0.7f;
            }

        }
        else
        {
            player.movement.x = aiRandomMoveX;
            player.movement.z = aiRandomMoveZ;

            Debug.DrawRay(transform.position + transform.forward * 2, Vector3.down * 10, Color.blue);
            Ray ray = new Ray(transform.position + transform.forward * 2, Vector3.down * 10);
            if (!Physics.Raycast(ray, out hit) || !hit.transform.CompareTag("Plane"))//앞에 땅이없으면 새로운방향으로 이동
            {
                PickRandomDirection();
            }
        }
    }

    IEnumerator PickTimer()
    {
        PickRandomDirection();
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        StartCoroutine(PickTimer());
    }

    void PickRandomDirection()
    {
        aiRandomMoveX = Random.Range(-1f, 1f);
        aiRandomMoveZ = Random.Range(-1f, 1f);
        player.movement.x = aiRandomMoveX;
        player.movement.z = aiRandomMoveZ;
    }

    IEnumerator UpdateTarget()
    {
        float nowDistance = 999;
        int i = 0;
        enemys = GameObject.FindGameObjectsWithTag("Player");

        //일정시간마다 맵의 플레이어를 전부 찾아서 제일 가깝고 범위내에 있는놈으로 타겟을 설정
        for (i = 0; i < enemys.Length; i++)
        {
            //(&& Vector3.Distance(enemys[i].transform.position, transform.position) != 0) 안하면 항상 자기자신을 타겟설정
            if (Vector3.Distance(enemys[i].transform.position, transform.position) <= nowDistance && Vector3.Distance(enemys[i].transform.position, transform.position) != 0)
            {

                nowDistance = Vector3.Distance(enemys[i].transform.position, transform.position);
                aiTarget = enemys[i].transform.position;
            }
        }

        if (nowDistance >= range)//인지거리내에 없으면
        {
            aiTarget = new Vector3(0, 0, 0);
            aiFire = false;
        }
        else
        {
            aiFire = true;
        }

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UpdateTarget());
    }

    IEnumerator RandomNameSkin()
    {
        yield return new WaitForSeconds(Time.deltaTime);

        if (photonView == null)
        {
            player.name.text = DataController.instance.nowAI.aiNames[Random.Range(0, DataController.instance.nowAI.aiNames.Length)];

            for (int i = 0; i < player.tankSkins.Length; i++)
            {
                player.tankSkins[i].GetComponent<MeshRenderer>().material = DataController.instance.mt[Random.Range(0, DataController.instance.mt.Length)];

            }
            //미사일은 다른 마테리얼써야함 생성될때 바꿀거기 때문에 따로 int로 빼놓음
            player.bulletSkinNum = Random.Range(0, DataController.instance.mtMissile.Length);

        }
        else if (photonView.IsMine)
        {
            int a = Random.Range(0, DataController.instance.nowAI.aiNames.Length);

            int[] b = new int[3];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = Random.Range(0, DataController.instance.mt.Length);

            }

            int c = Random.Range(0, DataController.instance.mtMissile.Length);

            photonView.RPC("ChangeAi", RpcTarget.AllBuffered,a,b,c);

        }
        else
        {

        }
    }


    [PunRPC]
    public void ChangeAi(int a,int[] b,int c)
    {
        if(player != null)
        {
            player.name.text = DataController.instance.nowAI.aiNames[a];


            for (int i = 0; i < player.tankSkins.Length; i++)
            {
                player.tankSkins[i].GetComponent<MeshRenderer>().material = DataController.instance.mt[b[i]];

            }
            //미사일은 다른 마테리얼써야함 생성될때 바꿀거기 때문에 따로 int로 빼놓음
            player.bulletSkinNum = c;

        }
    }
}
