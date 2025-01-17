using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    public GameObject planeTile;
    public static GameObject planeTiles;
    public int mapSize;

    public GameObject barricade;

    private void Awake()
    {
        mapSize = 20;
        planeTiles = transform.GetChild(0).gameObject;

        if (GetComponent<GameManager>().isSolo)
        {
            mapCreate(true);
        }
        else 
        {
            mapCreate(false);
        }

    }

    public void mapCreate(bool isSolo)
    {

        for (int i = -mapSize; i < mapSize; i++)
        {
            for (int e = -mapSize; e < mapSize; e++)
            {
                if(isSolo)
                {
                    Instantiate(planeTile, new Vector3(i * 2, 0, e * 2), Quaternion.identity);//Plane크기가 0.2라 2곱함
                }
                else
                {
                    if(PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Instantiate("PlaneTile", new Vector3(i * 2, 0, e * 2), Quaternion.identity);//Plane크기가 0.2라 2곱함
                    }
                }
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            if (isSolo)
            {
                Instantiate(barricade, new Vector3(Random.Range(-mapSize * 2, mapSize * 2 + 1), 1f, Random.Range(-mapSize * 2, mapSize * 2 + 1)), Quaternion.identity);
            }
            else
            {

                
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate("Barricade", new Vector3(Random.Range(-mapSize * 2, mapSize * 2 + 1), 1f, Random.Range(-mapSize * 2, mapSize * 2 + 1)), Quaternion.identity);
                }
                
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
