using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class GamePlayUIController : MonoBehaviour {

	#region 变量声明

	public GameObject player;

	// 每步计分
	[HideInInspector]
	public int score;

	// 钻石数量Text
	public Text numberOfGemsText;

	// 开始UI
	public GameObject startMenu;
	// 游戏UI
	public GameObject gameMenu;
	// 游戏结束UI
	public GameObject gameOverMenu;
	// 奖励UI
	public GameObject prizeMenu;

	// 当前得分Text（游戏中和游戏结束UI上）
	public Text[] scoreTexts;
	// 最高得分Text（开始和游戏结束UI上）
	public Text[] bestScoreTexts;

    // 游戏结束UI上的Panels(奖励视频，免费礼品，新模型，评价我们和提升)
    public RectTransform[] gameOverPanels;

	// 奖励的钻石Image
	public Image newGemsImage;
	// 奖励的钻石Text
	public Text newGemsText;
	// 奖励的新球
	public Text newBallText;

	#endregion

	#region Unity回调
		
	void Start ()
	{
		startMenu.SetActive (true);
		gameMenu.SetActive (false);
		gameOverMenu.SetActive (false);
		prizeMenu.SetActive (false);

		int bestScore = PlayerPrefs.GetInt("Best Score");
		updateBestScoreUITexts (bestScore);

		// 更新显示的钻石数量
		updateNumberOfGemsUITexts ();
	}

	#endregion

	#region 游戏状态方法

	public void onGameStarted ()
	{
		startMenu.SetActive (false);
		gameMenu.SetActive (true);
		gameOverMenu.SetActive (false);
		prizeMenu.SetActive (false);

		// 增加游戏次数，用于决定什么时候弹出点赞面板
		int gamesPlayed = PlayerPrefs.GetInt ("GamesPlayed");
		gamesPlayed++;
		PlayerPrefs.SetInt ("GamesPlayed", gamesPlayed);
	}

	public void onGameOver ()
	{
		// 是否更新最高记录
		int bestScore = PlayerPrefs.GetInt("Best Score");
		if (score > bestScore)
		{
			PlayerPrefs.SetInt ("Best Score", score);
			updateBestScoreUITexts (score);
		}
		else
		{
			updateBestScoreUITexts (bestScore);
		}

		
		startMenu.SetActive (false);
		gameMenu.SetActive (false);
		gameOverMenu.SetActive (true);
		prizeMenu.SetActive (false);

		// 检测游戏结束UI状态
		checkGameOverPanelsStatus ();

		// 游戏结束UI上的面板动画
		for (int i = 0; i< gameOverPanels.Length; i++)
		{
			StartCoroutine (animatePanel(gameOverPanels[i]));
		}
	}

	#endregion

	#region 更新数值

	public void updateScoreUITexts ()
	{
		// 更新所有的当前得分
		for (int i = 0; i < scoreTexts.Length; i++)
		{
			scoreTexts [i].text = "" + score;
		}
	}

	public void updateBestScoreUITexts (int bestScore)
	{
		// 更新所有的最高记录
		for (int i = 0; i < bestScoreTexts.Length; i++)
		{
			bestScoreTexts[i].text = "记录: " + bestScore;
		}
	}

	public void updateNumberOfGemsUITexts ()
	{
		// 更新钻石数量
		int numberOfGems = PlayerPrefs.GetInt ("NumberOfPickUps");
		numberOfGemsText.text = "" + numberOfGems;
	}

	

	#endregion

	#region UI动画

	IEnumerator animatePanel (RectTransform animatingPanel)
	{
		Vector2 canvasSize = GetComponent<RectTransform> ().sizeDelta;

		// 移动Panel到屏幕左边
		animatingPanel.anchoredPosition = new Vector3 (-canvasSize.x, animatingPanel.anchoredPosition.y, 0);

		// 动画结束位置
		Vector3 targetPosition = new Vector3 (0, animatingPanel.anchoredPosition.y, 0);

		// 每帧在X轴上移动的大小
		float xPosStep = 20;

		while (true)
		{
			// 每帧在x方向加一个步进
			animatingPanel.anchoredPosition = new Vector3 (animatingPanel.anchoredPosition.x + xPosStep, animatingPanel.anchoredPosition.y, 0);
	
			if (animatingPanel.anchoredPosition.x >= targetPosition.x)
			{
				// 如果到达或者超出目标位置

				// 就用目标位置直接设置面板的位置
				animatingPanel.anchoredPosition = targetPosition;

				yield break;
			}

			// 否则下一帧继续执行
			yield return null;
		}
	}

	#endregion

	#region 按钮点击

	public void onSettingsButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
		SceneManager.LoadScene ("Settings");
	}

	public void onModelShopButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // TODO:加载商店场景
    }

	public void onLeaderboardButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // TODO:显示排行榜
    }

	public void onRestartButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        SceneManager.LoadScene ("GamePlay");
	}

	public void onShareButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // TODO:显示分享
    }

	public void onRateUsButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // TODO:打开AppStore的好评界面
#if UNITY_ANDROID
        //Application.OpenURL ("market://details?id=games.fa.rollingcolor"); 
#elif UNITY_IPHONE
		//Application.OpenURL ("itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id=1146256287&onlyLatestVersion=true&pageNumber=0&sortOrdering=1&type=Purple+Software");
#endif
    }

	public void onPromotionButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // TODO:打开推广界面
#if UNITY_ANDROID
        //Application.OpenURL ("market://details?id=games.fa.rollingcolor"); 
#elif UNITY_IPHONE
		//Application.OpenURL ("https://itunes.apple.com/app/id1146256287");
#endif
    }

	public void onFreeGiftButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // TODO:免费礼物
    }

	public void onPrizeDoneButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // 检查游戏结束UI的状态
        checkGameOverPanelsStatus ();

		startMenu.SetActive (false);
		gameMenu.SetActive (false);
		gameOverMenu.SetActive (true);
		prizeMenu.SetActive (false);
	}

	#endregion

	#region 更新游戏结束面板的状态

	void checkGameOverPanelsStatus()
	{
		// 更新免费礼物面板状态
		gameOverPanels [0].gameObject.SetActive (true);

		// 好评&&推广面板
		int gamesPlayed = PlayerPrefs.GetInt ("GamesPlayed");
		if (gamesPlayed % 14 == 0)
		{
			// 每玩14次就显示好评面板
			gameOverPanels [0].gameObject.SetActive (false);
			gameOverPanels [1].gameObject.SetActive (true);
			gameOverPanels [2].gameObject.SetActive (false);
		}
		else if (gamesPlayed % 5 == 0)
		{
			// 每玩5次显示推广面板（广告）
			gameOverPanels [0].gameObject.SetActive (false);
			gameOverPanels [1].gameObject.SetActive (false);
			gameOverPanels [2].gameObject.SetActive (true);
		}
	}

    private void CheckFreeGiftStatus()
    {
        //TimeSpan 
        string oldTimeStr = PlayerPrefs.HasKey("TimeStamp")?PlayerPrefs.GetString("TimeStamp"): "0000-00-00 00:00:00";
        DateTime oldTime = Convert.ToDateTime(oldTimeStr);
    }

	#endregion
}
