using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [Header("............[ Core ]")]
    public int score;
    public int maxLevel;
    public bool isOver;


    [Header("............[ Object pool ]")]
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public List<Dongle> donglepool;

    [Range(1, 30)]
    public int poolSize;
    public int poolCursor;


    [Header("............[ UI ]")]
    public GameObject startGroup;
    public GameObject endGroup;
    public Text scoreText;
    public Text maxScoreText;
    public Text subScoreText;

    [Header("............[etc]")]
    public GameObject line;
    public GameObject bottom;


    void Awake()
    {
        Application.targetFrameRate = 60;

        donglepool = new List<Dongle>();
        for(int index=0; index< poolSize; index++ )
        {
            MakeDongle();
        }

        if(!PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
        
        maxScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();
    }
     public void GameStart()
    {
        // ������Ʈ Ȱ��ȭ
        line.SetActive(true);
        bottom.SetActive(true);
        scoreText.gameObject.SetActive(true);
        maxScoreText.gameObject.SetActive(true);
        startGroup.SetActive(false);


        Invoke("NextDongle", 1.5f);
    }

   
    Dongle MakeDongle()
    {
        GameObject instant = Instantiate(donglePrefab, dongleGroup);
        instant.name = "Dongle" + donglepool.Count;
        Dongle instantDongle = instant.GetComponent<Dongle>();
        instantDongle.manager = this;
        donglepool.Add(instantDongle);
        return instantDongle;
    }
   
    Dongle GetDongle()
    {
        for(int index=0; index < donglepool.Count; index++)
        {
            poolCursor = (poolCursor + 1) % donglepool.Count;
            if (!donglepool[poolCursor].gameObject.activeSelf)
            {
                return donglepool[poolCursor];
            }
        }

        return MakeDongle();
    }
    void NextDongle()
    {
        if(isOver)
        {
            return;
        }
        
        
        lastDongle = GetDongle();
        lastDongle.Level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);

        StartCoroutine("WaitNext");
    }

    IEnumerator WaitNext()
    {
        while (lastDongle != null)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(2.5f);

       

        NextDongle();
    }

    
    
    public void TouchDown()
    {
        if (lastDongle == null)
            return;
        
        lastDongle.Drag();
    }

    public void TouchUp()
    {
        if (lastDongle == null)
            return;

        lastDongle.Drop();
        lastDongle = null;
    }

    public void GameOver()
    {
        if (isOver)
        {
            return;
        }
        
        isOver = true;

        StartCoroutine("GameOverRoutine");
       
    }

    IEnumerator GameOverRoutine()
    {
        // 1. ��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� ���� ��������
        Dongle[] dongles = FindObjectsOfType<Dongle>();

        // 2. ����� ���� ��� ������ ����ȿ�� ��Ȱ��ȭ
        for (int index = 0; index < dongles.Length; index++)
        {
            dongles[index].rigid.simulated = false;
        }

        // 3. 1���� ����� �ϳ��� �����
        for (int index = 0; index < dongles.Length; index++)
        {
            dongles[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);

        }

        // �ְ� ���� ����
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);
        // ���ӿ��� UI ǥ��
        subScoreText.text = "���� : " + scoreText.text;
        endGroup.SetActive(true);

    }

    public void Reset()
    {
        StartCoroutine("ResetCoroutine");
    }
    
    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    void LateUpdate()
    {
        scoreText.text = score.ToString();
    }


}

