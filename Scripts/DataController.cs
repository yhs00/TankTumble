using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public string playerName;

    public int[] skinNum = new int[4];//ũ��� �׻� MainMenu.tankSkin.Length�� ���ƾ���

    public int highScore;

    public float[] volumes = {1,1,1};//Master,BGM,SFX

    public bool isJstickTurn = false;
}

public class AIData
{
    public string[] aiNames = { "Ghost", "Wraith", "Shade", "Spectre", "Phantom", "Faker", "Prodigy", "Legend", "Ace", "Maverick", "Blitz", "Viper", "Seraph", "Valkyrie", "Phoenix", "Zenith", "Apex", "Summit", "Ascend", "Soar", "Enigma", "Cipher", "Oracle", "Mystic", "Prophet", "Shadow", "Nova", "Echo", "Titan", "Hydra", "Chimera", "Siren", "Tempest", "Mirage", "Reaper", "Havoc", "Sparky", "Bolt", "Zapper", "Volt", "Surge", "Shock", "Static", "Jolt", "Arc", "Flare", "Ignite", "Blaze", "Ember", "Cinder", "Pyro", "Magma", "Lava", "Comet", "Whisper" };
}


public class DataController : MonoBehaviour
{
    public static DataController instance;

    public PlayerData nowPlayer = new PlayerData();
    public AIData nowAI = new AIData();
    string path;
    string filename = "save";

    public Material[] mt;
    public Material[] mtMissile;

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
        DontDestroyOnLoad(this.gameObject);

        path = Application.persistentDataPath + "/";
    }

    public void Start()
    {
        if (!File.Exists(DataController.instance.path + filename))
        {
            SaveData();
            Debug.Log("������ ��� �Ѱ� ������");
        }
        LoadData();
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowPlayer);

        File.WriteAllText(path + filename, data);
        //print(path);
    }

    public void LoadData()
    {
        string data = File.ReadAllText(path + filename);
        nowPlayer = JsonUtility.FromJson<PlayerData>(data);
    }
}