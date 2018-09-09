using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopController : MonoBehaviour {

    #region 变量定义
    public GameObject shopMenu;
    public GameObject newBallMenu;
    public Text numberOfGemsText;
    public GameObject player;
    public Text unlockedBallsCountText;
    public ScrollRect scrollRect;
    public Slider scrollSlider;
    public GameObject newBall;
    public GameObject giftImage;
    public GameObject giftFx;
    public Sprite selectedSprite;
    public Sprite unSelectedSprite;
    public GameObject[] ballsAry;
    private GameObject curSelectedBall = null;
    private int unlockedBallsCount = 0;
    private bool isNewBalling = false;
    #endregion

    #region Unity回调
    private void Awake()
    {
        updateNumberOfGemsUIText();
        initShop();
        initCurBall();
        updateUnlockBallsText();
    }

    private void Start()
    {
        scrollRect.onValueChanged.AddListener(onScrollRectValueChanged);
    }
    #endregion

    #region 点击事件
    public void onBackButtonClicked()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        SceneManager.LoadScene("GamePlay");
    }

    public void onBallSelected(Button btn)
    {
        string ballName = btn.name;
        int ballUnlockStatus = PlayerPrefs.GetInt(ballName, 0);
        bool isUnlocked = (ballUnlockStatus == 1 ? true : false);
        if (isUnlocked)
        {
            PlayerPrefs.SetString("CurBall", ballName);
            if (curSelectedBall == null)
            {
                curSelectedBall = getCurSelectedBall(ballName);
                Material curSelectedBallMat = curSelectedBall.GetComponent<MeshRenderer>().sharedMaterial;
                player.GetComponent<MeshRenderer>().material = curSelectedBallMat;
                btn.GetComponent<Image>().sprite = selectedSprite;
            }
            else
            {
                if (!btn.name.Equals(curSelectedBall.name))
                {
                    Image oldBallBg = scrollRect.content.Find(curSelectedBall.name).GetComponent<Image>();
                    oldBallBg.sprite = unSelectedSprite;
                    curSelectedBall = getCurSelectedBall(ballName);
                    Material curSelectedBallMat = curSelectedBall.GetComponent<MeshRenderer>().sharedMaterial;
                    player.GetComponent<MeshRenderer>().material = curSelectedBallMat;
                    btn.GetComponent<Image>().sprite = selectedSprite;
                }
            }
        }
        else
        {
            // TODO:不同的球解锁方式不同
            switch (btn.name)
            {
                case "Ball1":
                    //buyBall("Ball1",50);
                    break;
                case "Ball10":
                    //Follow Twitter
                    unLockBall("Ball10");
                    break;
            }
        }
    }


    public void onNewBallDoneButtonClicked()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        newBallMenu.SetActive(false);
        shopMenu.SetActive(true);
        newBall.SetActive(false);
        giftImage.SetActive(true);
        giftFx.SetActive(false);
    }
    #endregion

    #region 支持方法

    private void initCurBall()
    {
        string curBallName = PlayerPrefs.GetString("CurBall","Ball1");
        Button ballBtn = scrollRect.content.Find(curBallName).GetComponent<Button>();
        onBallSelected(ballBtn);
    }

    private void initShop()
    {
        shopMenu.SetActive(true);
        newBallMenu.SetActive(false);
        checkAllBallsStatus();
    }

    private void updateUnlockBallsText()
    {
        unlockedBallsCountText.text = "" + unlockedBallsCount + "/" + scrollRect.content.childCount;
    }

    //买球或者完成任时刷新
    void updateNumberOfGemsUIText()
    {
        int numberOfGems = PlayerPrefs.GetInt("NumberOfPickUps");
        numberOfGemsText.text = "" + numberOfGems;
    }

    private void checkAllBallsStatus()
    {
        for (int i =0;i<scrollRect.content.childCount;++i)
        {
            Transform child = scrollRect.content.GetChild(i);
            checkBallStatus(child);
        }
    }


    private void checkBallStatus(Transform ball)
    {
        //是否解锁
        string ballName = ball.name;
        GameObject ballImage = ball.Find("UIObject3D").gameObject;
        GameObject lockImage = ball.Find("Lock").gameObject;
        int ballstatus = PlayerPrefs.GetInt(ballName, 0);
        ballImage.SetActive(ballstatus == 1 ? true : false);
        lockImage.SetActive(ballstatus == 1 ? false : true);
        unlockedBallsCount += (ballstatus == 1 ? 1 : 0);
    }

    private GameObject getCurSelectedBall(string ballName)
    {
        GameObject curBall = null;
        for (int i = 0; i <ballsAry.Length;++i)
        {
            if (ballsAry[i].name.Equals(ballName))
            {
                curBall = ballsAry[i];
                return curBall;
            }
        }
        return null;
    }

    private void buyBall(string ballName, int gemsPrice)
    {
        unLockBall(ballName);
    }

    private void unLockBall(string ballName)
    {
        Transform ball = scrollRect.content.Find(ballName);
        Transform ballImage = ball.Find("UIObject3D");
        Transform lockImage = ball.Find("Lock");
        ballImage.gameObject.SetActive(true);
        lockImage.gameObject.SetActive(false);
        PlayerPrefs.SetInt(ballName, 1);
        unlockedBallsCount++;
        showGetNewBall();
        updateUnlockBallsText();
    }

    private void showGetNewBall()
    {
        shopMenu.SetActive(false);
        newBallMenu.SetActive(true);
        newBall.SetActive(false);
        giftImage.SetActive(true);
        giftFx.SetActive(false); 
        StartCoroutine(PlayNewBallAnim());
    }

    private void onScrollRectValueChanged(Vector2 value)
    {
        scrollSlider.value = value.x;
    }

    IEnumerator PlayNewBallAnim()
    {
        isNewBalling = true;

        float start = Time.time;

        while (Time.time - start < 2f)
        {
            giftImage.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-10f, 10f));
            giftImage.transform.localScale = new Vector3(UnityEngine.Random.Range(0.9f, 1.3f), UnityEngine.Random.Range(0.9f, 1.3f), UnityEngine.Random.Range(0.9f, 1.3f));
            yield return new WaitForSeconds(0.07f);
        }

        start = Time.time;
        Vector3 startScale = giftImage.transform.localScale;

        while (Time.time - start < 0.15f)
        {
            giftImage.transform.localScale = Vector3.Lerp(startScale, Vector3.one * 20f, (Time.time - start) / 0.2f);
            yield return null;
        }

        giftImage.gameObject.SetActive(false);

        // Show newBall
        newBall.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        SoundManager.Instance.PlaySound(SoundManager.Instance.rewarded);

        isNewBalling = false;

        giftFx.SetActive(true);
    }
    #endregion
}
