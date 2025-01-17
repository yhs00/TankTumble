using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public DataController data;
    public TMP_InputField nameField;
    public TMP_Text highScoreText;

    public GameObject[] tankSkin;//����

    public AudioMixer audioMixer;
    public Slider[] audioSliders;
    public Toggle jstickTurn;

    public GameObject quitMenu;
    bool isOtherMenuOpen;

    public TMP_Text noInternetText;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Disconnect();//���θ޴��ε��ƿ��� ��������
        data = DataController.instance;

        nameField.text = data.nowPlayer.playerName;
        highScoreText.text = "HighScore :" + data.nowPlayer.highScore;

        for (int i = 0; i < tankSkin.Length; i++)
        {

            tankSkin[i].GetComponent<MeshRenderer>().material = data.mt[data.nowPlayer.skinNum[i]];

            if (i == 3) //�̻����� �ٸ� ���׸�������
            {
                tankSkin[i].GetComponent<MeshRenderer>().material = data.mtMissile[data.nowPlayer.skinNum[i]];
            }
        }

        for(int i = 0;i < audioSliders.Length; i++)
        {
            audioSliders[i].value = data.nowPlayer.volumes[i];
        }

        jstickTurn.isOn = data.nowPlayer.isJstickTurn;

        isOtherMenuOpen = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isOtherMenuOpen)
        {
            QuitMenu();
        }
    }

    public void SoloButton()
    {
        //SceneManager.LoadScene("SoloScene");
        SceneFader.sceneFader.FadeTo("SoloScene");
    }

    public void MultiButton()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // ���ͳ� ���� ����
            Debug.Log("���ͳ� ���ῡ ������� �ʾҽ��ϴ�.");
            StartCoroutine(NoInetnetConnetced());
        }
        else
        {
            // ���ͳ� �����
            Debug.Log("���ͳ� ���ῡ ����Ǿ� �ֽ��ϴ�.");
            SceneFader.sceneFader.FadeTo("ConnectionScene");
        }
    }

    public void ChangeSkinUp(int skinNum)//0=body,1=cannon,2=tower,3=missile
    {
        if (data.nowPlayer.skinNum[skinNum] >= data.mt.Length - 1)//��Ų �ִ밹��(4)���� ������ �ȵǴϱ� ����
        {
            return;
        }
        data.nowPlayer.skinNum[skinNum]++;
        tankSkin[skinNum].GetComponent<MeshRenderer>().material = data.mt[data.nowPlayer.skinNum[skinNum]];
        if (skinNum == 3)
        {
            tankSkin[skinNum].GetComponent<MeshRenderer>().material = data.mtMissile[data.nowPlayer.skinNum[skinNum]];
        }

    }

    public void ChangeSkinDown(int skinNum)//0=body,1=cannon,2=tower,3=missile
    {
        if (data.nowPlayer.skinNum[skinNum] <= 0)//��Ų ��������(0)���� ������ �ȵǴϱ� ����
        {
            return;
        }
        data.nowPlayer.skinNum[skinNum]--;
        tankSkin[skinNum].GetComponent<MeshRenderer>().material = data.mt[data.nowPlayer.skinNum[skinNum]];
        if (skinNum == 3)
        {
            tankSkin[skinNum].GetComponent<MeshRenderer>().material = data.mtMissile[data.nowPlayer.skinNum[skinNum]];
        }

    }

    public void ConfirmButton()
    {
        data.SaveData();
        isOtherMenuOpen = false;
    }

    public void NameChanged()
    {
        data.nowPlayer.playerName = nameField.text;
        data.SaveData();
        Debug.Log(data.nowPlayer.playerName);
    }

    public void VolumeChanged(int volume)
    {
        string[] audioNames = {"Master","BGM","SFX"};
        data.nowPlayer.volumes[volume] = audioSliders[volume].value;
        audioMixer.SetFloat(audioNames[volume], Mathf.Log10(audioSliders[volume].value) * 20);
    }

    public void JoystickToggle(Toggle toggle)
    {
        data.nowPlayer.isJstickTurn = toggle.isOn;
    }

    public void QuitMenu()
    {
        if(!quitMenu.activeInHierarchy)
        {
            quitMenu.SetActive(true);
        }
        else if (quitMenu.activeInHierarchy)
        {
            quitMenu.SetActive(false);
        }
    }

    public void OtherMenuOpen()
    {
        isOtherMenuOpen = true;
    }

    public void YesQuit()
    {
        Application.Quit();
    }

    IEnumerator NoInetnetConnetced()
    {
        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            noInternetText.color = new Color(1f, 0f, 0f, t);//a�� ���� ��
            yield return 0;
        }
    }
}
