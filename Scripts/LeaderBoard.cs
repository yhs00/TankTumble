using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    public static LeaderBoard instance;
    public GameObject personalBoard;

    public List<GameObject> pbList;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pbList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        RemovePB();
    }

    public void InstantiatePB(GameObject master)
    {
        GameObject PB = Instantiate(personalBoard, transform);
        PB.GetComponent<PersonalBoard>().master = master;

        pbList.Add(PB);
        SortPB();
    }

    int compare(GameObject a, GameObject b)//포인트가 큰쪽이 -1 작은쪽이 1 반환
    {
        return a.GetComponent<PersonalBoard>().point > b.GetComponent<PersonalBoard>().point ? -1 : 1;
    }

    public void SortPB()
    {
        pbList.Sort(compare);
        for (int i = 0; i < pbList.Count; i++)
        {
            pbList[i].transform.SetSiblingIndex(i);
            //pbList[i].GetComponent<PersonalBoard>().rankText.text = "#" + (i+1);
        }
    }

    public void RemovePB()
    {
        for (int i = pbList.Count -1; i >= 0; i--)
        {
            if (pbList[i] == null)
            {
                pbList.RemoveAt(i);
            }

        }
    }

}
