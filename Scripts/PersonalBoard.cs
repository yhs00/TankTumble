using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersonalBoard : MonoBehaviour
{
    public GameObject master;
    Player player;

    public int point;
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        player = master.GetComponent<Player>();

        Color color = new Color(Random.Range(0.000f,1f), Random.Range(0.000f, 1f), Random.Range(0.000f, 1f), 1);
        
        rankText.color = color;
        nameText.color = color;
        scoreText.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (master == null)
        {
            Destroy(this.gameObject);
        }

        if(point != player.point)
        {
            point = player.point;
            LeaderBoard.instance.SortPB();
        }
        nameText.text = player.name.text;
        scoreText.text = player.score.text;

        rankText.text = "#" + (gameObject.transform.GetSiblingIndex() + 1);

    }

}
