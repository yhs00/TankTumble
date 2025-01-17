using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallowTank : MonoBehaviour
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
        targetPos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        transform.position = targetPos;
    }
}
