using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using UnityEngine.Scripting;
using JetBrains.Annotations;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject enemyAI;
    public GameObject player;

    public Vector3 tp;
    public bool isSolo;
    public int maxUnit;
    public List<GameObject> ais;

    public GameObject myPlayer;
    public static bool isDiePlayer;
    public GameObject gameOverUI;


    public GameObject joyStick1;
    public GameObject joyStick2;

    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {

        ais = new List<GameObject>();

        if(DataController.instance.nowPlayer.isJstickTurn)
        {
            Vector3 jsTp = joyStick1.transform.position;
            joyStick1.transform.position = joyStick2.transform.position;
            joyStick2.transform.position = jsTp;
        }
        
        isDiePlayer = false;

        if(isSolo)
        {
            //�÷��̾� ����
            SpawnRandom();
            myPlayer = Instantiate(player, tp + new Vector3(0, 1, 0), Quaternion.identity);

            AiSpawn(1);
        }
        else
        {

            //OnPhotonSerializeView ȣ��� //�������� ���涧 ���
            PhotonNetwork.SerializationRate = 60;

            //RPCȣ��� //���Ҷ��ѹ�
            PhotonNetwork.SendRate = 60;

            //��Ƽ�÷��̾����
            StartCoroutine(WaitSpawn());

            if (PhotonNetwork.IsMasterClient)
            {            
                AiSpawn(PhotonNetwork.CountOfPlayers);
            }
        }

        
    }


    // Update is called once per frame
    void Update()
    {
        if (isDiePlayer)
        {
            gameOverUI.SetActive(true);
            isDiePlayer=false;
        }
    }

    public void SpawnRandom()
    {
        Transform PlaneTp = MapCreator.planeTiles.transform.GetChild(Random.Range(0, MapCreator.planeTiles.transform.childCount)); // Ÿ�����ϳ� ����
        tp = new Vector3(PlaneTp.position.x, PlaneTp.position.y + 2f, PlaneTp.position.z); //���� Ÿ������ Ÿ���� �ű��
    }

    void AiSpawn(int playerCount)
    {
        SpawnRandom();
        if (isSolo)
        {
            if(playerCount + ais.Count < maxUnit)
            {
                ais.Add(Instantiate(enemyAI, tp + Vector3.up, Quaternion.identity));
                AiSpawn(1);
            }
        }
        else
        {
            if (playerCount + ais.Count < maxUnit)
            {
                ais.Add(PhotonNetwork.Instantiate("EnemyAI", tp + Vector3.up, Quaternion.identity));
                AiSpawn(PhotonNetwork.CountOfPlayers);
            }
            
        }
    }
    void AiDespawn()
    {
        if (isSolo && ais.Count > 0)
        {
            Destroy(ais.Last());
            ais.RemoveAt(ais.Count - 1);
        }
        else if(!isSolo && ais.Count > 0)
        {
            PhotonNetwork.Destroy(ais.Last());
            
            ais.RemoveAt(ais.Count - 1);
        }
    }

    IEnumerator WaitSpawn()
    {
        while(1600 > GameObject.FindGameObjectsWithTag("Plane").Length)
        {
            Debug.Log(GameObject.FindGameObjectsWithTag("Plane").Length);
            yield return null;
        }

        //�÷��̾� ����
        SpawnRandom();
        myPlayer = PhotonNetwork.Instantiate("Player", tp + new Vector3(0, 1, 0), Quaternion.identity);

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (PhotonNetwork.IsMasterClient)
        {
            AiDespawn();
        }
        LeaderBoard.instance.SortPB();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (PhotonNetwork.IsMasterClient)
        {
            AiSpawn(PhotonNetwork.CountOfPlayers);
        }
        LeaderBoard.instance.SortPB();
    }

    
    public void ReStart()
    {
        GameObject mycar = myPlayer.transform.GetChild(0).gameObject;
        mycar.GetComponent<Rigidbody>().isKinematic = true;

        SpawnRandom();

        mycar.gameObject.transform.position = tp;
        mycar.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        mycar.GetComponent<Rigidbody>().isKinematic = false;//���ư��� ���� �ʱ�ȭ�ϱ����ؼ� ��� �״ٲ�

        mycar.GetComponent<Fire>().isCanFire = true;
        mycar.GetComponent<Player>().moveSpeed = 10;
        mycar.GetComponent<Player>().point = 0;//���� �ʱ�ȭ

    }

    public void QuitMenu()
    {
        SceneFader.sceneFader.FadeTo("MainMenu");
        
    }
}
