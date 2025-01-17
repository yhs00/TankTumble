using System.Collections;
using UnityEngine;
using Photon.Pun;


public class Fire : MonoBehaviourPun, IPunObservable
{
    bool isAI;
    EnemyAI enemyAI;

    public GameObject tankHead;
    public GameObject bullet;
    public GameObject firePoint;
    public Animator tankCannon;
    public bool isCanFire;

    public Camera mainCamera;
    public AudioListener aL;
    RaycastHit hit;

    public Joystick joystick;
    public Vector3 JTargetTr;
    public GameObject joystickTarget;
    public Vector3 target;
    public float angle;


    Quaternion receiveRot;
    

    // Start is called before the first frame update
    void Start()
    {

        joystick = GameObject.FindWithTag("FireJoystick").GetComponent<Joystick>();

        if (GetComponent<EnemyAI>() == null)
        {
            isAI = false;
        }
        else
        {
            isAI = true;
            enemyAI= GetComponent<EnemyAI>();
        }

        StartCoroutine(FireDelay());

        if(photonView == null)
        {
            return;
        }

        if (photonView.IsMine)
        {
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(true);
            }
            if (aL != null)
            {
                aL.enabled = true;
            }
        }
        else
        {
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(false);
            }
            if (aL != null)
            {
                aL.enabled = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isAI)
        {
            JoystickUse();
        }

        if (photonView == null)
        {
            LooK();
            BulletFire();
        }
        else if (photonView.IsMine) 
        {
            LooK();
            BulletFire();
        }
        else 
        {
            float t = Mathf.Clamp(20f * Time.deltaTime, 0f, 0.99f);
            tankHead.transform.rotation = Quaternion.Lerp(tankHead.transform.rotation, receiveRot, t);

        }

    }

    public void JoystickUse()
    {
        JTargetTr.x = joystick.Horizontal * 2;
        JTargetTr.z = joystick.Vertical * 2;

        if (JTargetTr != Vector3.zero)
        {
            joystickTarget.transform.localPosition = JTargetTr;
        }

    }

    public void LooK()
    {
        if (!isAI)
        {
            target = joystickTarget.transform.position;
        }
        //인공지능이 조종중이면 EnemyAI에서 알아서넘김



        if (target != null)
        {
            Debug.DrawRay(tankHead.transform.position, target - tankHead.transform.position, Color.red);
            Vector3 direction = target - tankHead.transform.position;
            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

        tankHead.transform.localRotation = Quaternion.RotateTowards(tankHead.transform.localRotation, Quaternion.Euler(0, angle - transform.eulerAngles.y, 0), 200f * Time.deltaTime);
        //tankHead.transform.localRotation = Quaternion.Euler(0, angle - transform.eulerAngles.y, 0);

    }

    public void BulletFire()
    {
        if(!isCanFire)
        {
            return; 
        }

        if (isAI)
        {
            if(enemyAI.aiFire)
            {
                if (photonView == null)
                {
                    BulletCreate();
                }
                else if (photonView.IsMine)
                {
                    photonView.RPC("BulletCreate", RpcTarget.All);
                }
            }
        }
        else if(JTargetTr != Vector3.zero)//pc로플레이면 JTargetTr != Vector3.zero 대신 Input.GetMouseButtonDown(0) 넣기
        {
            if (photonView == null)
            {
                BulletCreate();
            }
            else if(photonView.IsMine)
            {
                photonView.RPC("BulletCreate", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void BulletCreate()
    {
        GameObject myBullet = Instantiate(bullet, firePoint.transform.position, tankHead.transform.rotation);
        myBullet.GetComponent<Bullet>().bulletMaster = gameObject;
        tankCannon.SetTrigger("animIsFire");
        StartCoroutine(FireDelay());
    }


    IEnumerator FireDelay()
    {
        isCanFire = false;
        yield return new WaitForSeconds(1f);
        isCanFire = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //데이터 내거는 보내고 다른건 받고
        if (stream.IsWriting)//isMine == true
        {

            stream.SendNext(tankHead.transform.rotation);

        }
        else if (stream.IsReading)//isMine == false
        {

            receiveRot = (Quaternion)stream.ReceiveNext();

        }

    }
}
