using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClear : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            Destroy(this.gameObject);
        }
    }
}
