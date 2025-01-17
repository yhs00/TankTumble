using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject target;

    Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        targetPos = new Vector3(target.transform.position.x, target.transform.position.y + 20, target.transform.position.z - 10);
        transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * 100);
        transform.rotation = Quaternion.Euler(70, 0, 0);
    }
}