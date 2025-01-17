using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public AudioSource hitTankSFX;
    public AudioSource planeBreakSFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * Time.deltaTime * 20;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Plane"))
        {
            planeBreakSFX.Play();
            other.gameObject.GetComponent<PlaneTile>().isBreak = true;
        }
        else if(other.gameObject.CompareTag("Player"))//플레이어가 맞으면 날라감
        {
            hitTankSFX.Play();

            other.gameObject.GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * 50, ForceMode.Impulse);//뒤로 밀림
        }
    }
}
