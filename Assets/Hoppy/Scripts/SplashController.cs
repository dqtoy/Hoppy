using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SplashController : MonoBehaviour {

    #region 变量定义

    // 加载下一个场景前的等待时间
    private float setTime = 3;
    // 闪屏显示计时器
    float timer = 0.0f;                   

	#endregion

	#region Unity回调

	void Start ()
	{
        //android 和ios的值不同
		#if UNITY_ANDROID
		setTime = 3;
		#elif UNITY_IPHONE
		setTime = 6;
		#endif

		checkLanucheStatus ();
	}
		
	void Update ()
	{
		timer += Time.deltaTime;
		if (timer >= setTime)
			loadNextScene ();
	}

	#endregion

	#region 支持
    
	void checkLanucheStatus()
	{
		string hasLauncedBefore = PlayerPrefs.GetString("HasLauncedBefore");
		if (hasLauncedBefore != "Yes")
		{
			// 第一次运行游戏
			// 初始化所有玩家首选项

			// 下面的一些玩家偏好可能对分析数据很有用

			PlayerPrefs.SetString("HasLauncedBefore", "Yes");

			// 启动游戏次数
			PlayerPrefs.SetInt("LaunchCounter", 1);
			// 玩了多少次
			PlayerPrefs.SetInt("GamesPlayed", 0);
			// 是否启用声音
			PlayerPrefs.SetString("sound", "On");
			// 最高得分
			PlayerPrefs.SetInt("Best Score", 0);
			// 碰撞钻石次数
			PlayerPrefs.SetInt("NumberOfPickUps", 0);
		}
		else
		{
			// 不是第一次启动游戏

			// 增加启动次数
			int launchCounter = PlayerPrefs.GetInt("LaunchCounter");
			launchCounter++;
			PlayerPrefs.SetInt("LaunchCounter", launchCounter);
		}
	}

	void loadNextScene ()
	{
		SceneManager.LoadScene ("GamePlay");
	}

	#endregion
}
