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
    public Text unlockedBallCountText;
    public ScrollRect scrollRect;
    public Slider scrollSlider;
    public GameObject[] ballsAry;
    private GameObject curSelectedBall;
    #endregion

    #region Unity回调
    private void Awake()
    {
        updateNumberOfGemsUIText();
        initShop();
    }
    #endregion

    #region 点击事件
    public void onBackButtonClicked()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // Load the previous scene.
        SceneManager.LoadScene("GamePlay");
    }

    public void onBallSelected(GameObject btn)
    {
        string ballName = btn.name;
        int ballUnlockStatus = PlayerPrefs.GetInt(ballName, 0);
        bool isUnlocked = (ballUnlockStatus == 1 ? true : false);
        if (isUnlocked)
        {
            curSelectedBall = getCurSelectedBall(ballName);
        }
        else
        {
            // TODO:不同的球解锁方式不同
            switch (btn.name)
            {
                case "Ball1":
                    //buyBall("Ball1",50);
                    break;
            }
        }
    }
    #endregion

    #region 支持方法

    private void initShop()
    {
        shopMenu.SetActive(true);
        newBallMenu.SetActive(false);
    }

    //买球或者完成任时刷新
    void updateNumberOfGemsUIText()
    {
        int numberOfGems = PlayerPrefs.GetInt("NumberOfPickUps");
        numberOfGemsText.text = "" + numberOfGems;
    }

    void changeButtonSprites(Button button, Sprite unpressedImage, Sprite pressedImage)
    {
        button.GetComponent<Image>().sprite = unpressedImage;

        SpriteState st = new SpriteState();
        st.pressedSprite = pressedImage;
        button.spriteState = st;
    }

    private void checkAllBallsStatus()
    {
        for (int i =0;i<scrollRect.content.childCount;++i)
        {
            Transform child = scrollRect.content.GetChild(i);
            checkAllBallsStatus(child);
        }
    }


    private void checkAllBallsStatus(Transform ball)
    {
        string ballName = ball.name;
        GameObject ballImage = ball.Find("UIObject3D").gameObject;
        GameObject lockImage = ball.Find("Lock").gameObject;
        int ballstatus = PlayerPrefs.GetInt(ballName, 0);
        ballImage.SetActive(ballstatus == 1 ? true : false);
        lockImage.SetActive(ballstatus == 1 ? false : true);
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
        showGetNewBall();
        Transform ball = scrollRect.content.Find(ballName);
        Transform ballImage = ball.Find("UIObject3D");
        Transform lockImage = ball.Find("Lock");
        ballImage.gameObject.SetActive(true);
        lockImage.gameObject.SetActive(false);
    }

    private void showGetNewBall()
    {

    }
    #endregion
}
