using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using Photon.Realtime;


public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    bool isAI;

    public Vector3 movement;
    public int moveSpeed;
    public Animator[] wheel;
    public Joystick joystick;
    public bool isWheelMove;

    public new TMP_Text name;
    public GameObject[] tankSkins = new GameObject[3];//크기는 항상 MainMenu.tankSkin.Length -1 와 같아야함(bullet은 따로 해줄거기 때문에)
    public int bulletSkinNum;

    public GameObject whoHitMe;
    public int point;
    public TMP_Text score;

    Vector3 receivePos;
    Quaternion receiveRot;
    int receivePoint;

    public AudioSource hitSFX;
    public bool isDie;



    public Photon.Realtime.Player lastOwnerName;

    // Start is called before the first frame update
    void Start()
    {
        joystick = GameObject.FindWithTag("MoveJoystick").GetComponent<Joystick>();
        isDie  = false;
        moveSpeed = 10;
        if (GetComponent<EnemyAI>() == null)
        {
            isAI = false;
        }
        else
        {
            isAI = true;
        }

        int[] a = new int[3];
        for (int i = 0; i < a.Length; i++)
        {
            a[i] = DataController.instance.nowPlayer.skinNum[i];
        }
        //미사일은 다른 마테리얼써야함 생성될때 바꿀거기 때문에 따로 int로 빼놓음
        int bSnum = DataController.instance.nowPlayer.skinNum[3];

        
        if (photonView == null)
        {
            ChangeSkin(a, bSnum);
            name.text = DataController.instance.nowPlayer.playerName;

        }
        else if (photonView.IsMine)
        {
            if (!isAI)
            {
                photonView.RPC("ChangeSkin", RpcTarget.AllBuffered, a, bSnum);
                name.text = DataController.instance.nowPlayer.playerName;
            }
            lastOwnerName = photonView.Owner;
        }
        else
        {
            if (!isAI)
            {
                name.text = photonView.Owner.NickName;
            }
            lastOwnerName = photonView.Owner;
        }

        //퍼스널보드 생성
        LeaderBoard.instance.InstantiatePB(this.gameObject);
        LeaderBoard.instance.SortPB();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "" + point;

        if (photonView == null)
        {
            if (isDie)
            {
                StartCoroutine(Die());
                isDie = false;
            }

            Moving();
            return;
        }

        if (photonView.IsMine)
        {
            if (isDie)
            {
                StartCoroutine(Die());
                isDie = false;
            }

            Moving();
        }
        else
        {

            float t = Mathf.Clamp(20f * Time.deltaTime, 0f, 0.99f);
            transform.position = Vector3.Lerp(transform.position, receivePos, 20 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, t);
            point = receivePoint;
        }

    }

    public void Moving()
    {

        if (!isAI)
        {
            movement.x = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
            movement.z = Input.GetAxisRaw("Vertical") + joystick.Vertical;
        }
        //ai조종중일땐 EnemyAi에서 알아서 넘김

        transform.position += movement * Time.deltaTime * moveSpeed;

        if (movement != Vector3.zero && moveSpeed > 0)//움직일 때 실행
        {
            //Debug.Log(movement);
            isWheelMove = true;
            for (int i = 0; i < 4; i++)
            {
                wheel[i].speed = 1;

            }

            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(movement.x, 0, movement.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
        else
        {
            isWheelMove= false;
            for (int i = 0; i < 4; i++)
            {
                wheel[i].speed = 0;

            }
        }
    }

    IEnumerator Die()
    {
        //파괴이펙트 활성화
        GetComponent<Fire>().isCanFire = false;
        moveSpeed = 0;
        if(DataController.instance.nowPlayer.highScore < point)//최고기록 달성
        {
            DataController.instance.nowPlayer.highScore = point;
            DataController.instance.SaveData();
        }

        yield return new WaitForSeconds(1f);
        GameManager.isDiePlayer = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //데이터 내거는 보내고 다른건 받고
        if (stream.IsWriting)//isMine == true
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(point);
            stream.SendNext(isWheelMove);
        }
        else if (stream.IsReading)//isMine == false
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
            receivePoint = (int)stream.ReceiveNext();
            if((bool)stream.ReceiveNext() == true)
            {
                for (int i = 0; i < 4; i++)
                {
                    wheel[i].speed = 1;

                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    wheel[i].speed = 0;

                }
            }
            
        }
        
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (!isAI && otherPlayer == lastOwnerName)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }


    [PunRPC]
    public void ChangeSkin(int[] a, int bSN)
    {

        for (int i = 0; i < tankSkins.Length; i++)
        {
            tankSkins[i].GetComponent<MeshRenderer>().material = DataController.instance.mt[a[i]];
        }
        //미사일은 다른 마테리얼써야함 생성될때 바꿀거기 때문에 따로 int로 빼놓음
        bulletSkinNum = bSN;

    }

}
