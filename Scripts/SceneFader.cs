using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;

    public static SceneFader sceneFader;

    bool doubleClick = false; //��Ÿ�����ڵ�

    public void Start()
    {
        var a = FindObjectsOfType<SceneFader>();//�̰��ϴ� ���� �����ߴٰ� ���θ޴��� ���ư��� 2�� ����⶧��
        if (a.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
            sceneFader = this;

            SceneManager.sceneLoaded += Loadedscene;//�� ��ȯ���� Loadedscene()����

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Loadedscene(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string scene)//1: �ٸ� ��ũ��Ʈ���� ������ �ϸ� �ش��ϴ� ���� �޾ƿ�
    {
        if (doubleClick)//�̰�������: �������� Ŭ���� �ϸ� �Է��� �������Ǹ鼭 �ڷ�ƾ�� �ߺ����� �����
        {
            return;
        }
        StartCoroutine(FadeOut(scene));//2: �װ� ���̵�ƿ��� �ִ´�
        doubleClick = true;
    }

    IEnumerator FadeIn()//�����Ҷ� ���̵���
    {
        if (MapCreator.planeTiles == null)
        {
            for (float t = 1; t > 0; t -= Time.deltaTime)//�������Ӹ��� ���
            {
                float a = curve.Evaluate(t);//�ش�Ǵ� �ð��� Ŀ�� ��ġ�� a�� �Ҵ�
                img.color = new Color(0f, 0f, 0f, a);//a�� ���� ��
                yield return 0;//������ ���� �ۿ��� ������ �� �����ӿ� ������ ����Ǿ ȿ���� ���� 
            }
        }
        else
        {
            //MapCreator�� �ٴ��� �� �����ɶ����� ���̴�����
            while (1600 > GameObject.FindGameObjectsWithTag("Plane").Length)
            {
                yield return null;
            }


            for (float t = 1; t > 0; t -= Time.deltaTime)//�������Ӹ��� ���
            {
                float a = curve.Evaluate(t);//�ش�Ǵ� �ð��� Ŀ�� ��ġ�� a�� �Ҵ�
                img.color = new Color(0f, 0f, 0f, a);//a�� ���� ��
                yield return 0;//������ ���� �ۿ��� ������ �� �����ӿ� ������ ����Ǿ ȿ���� ���� 
            }
        }
        
    }

    IEnumerator FadeOut(string scene)//3: ������ sceneNum�� �Ҵ�Ǵµ�
    {

        for (float t = 0; t < 1; t += Time.deltaTime)//�������Ӹ��� ��Ӱ�
        {
            float a = curve.Evaluate(t);//�ش�Ǵ� �ð��� Ŀ�� ��ġ�� a�� �Ҵ�
            img.color = new Color(0f, 0f, 0f, a);//a�� ���� ��
            Time.timeScale = 1f;//������ �������� ������ ���ߴ� ���׹�����
            yield return 0;//������ ���� �ۿ��� ������ �� �����ӿ� ������ ����Ǿ ȿ���� ���� 
        }

        if(scene == "MultiScene")
        {
            PhotonNetwork.LoadLevel("MultiScene");//�Ǵ°����ȵǴ°����𸣰���
        }
        else
        {
            SceneManager.LoadScene(scene);//4: ���̵�ƿ��� �ϰ� �� �̸��� �´� ���� �ε��Ų��
        }

        doubleClick = false;
    }


}
