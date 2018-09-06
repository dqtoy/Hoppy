using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SettingsController : MonoBehaviour {


	#region 变量定义

	public Button soundButton;                     
	public Button restorePurchasesButton;          // 恢复购买按钮(android上禁用)

	public Sprite soundOnUnPressed;                
	public Sprite soundOnPressed;                  
	public Sprite soundOffUnPressed;               
	public Sprite soundOffPressed;                 

	public Text numberOfGemsText;                 

	#endregion

	#region Unity回调

	void Awake ()
	{
		#if UNITY_ANDROID
		restorePurchasesButton.gameObject.SetActive (false);
#endif

        bool isMuted = SoundManager.Instance.IsMuted();
		if (!isMuted)
		{
			changeButtonSprites (soundButton, soundOnUnPressed, soundOnPressed);
		}
		else
		{
			changeButtonSprites (soundButton, soundOffUnPressed, soundOffPressed);
		}

		updateNumberOfGemsUIText ();
	}

	#endregion

	#region 按钮点击

	public void onBackButtonClicked ()
	{
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // Load the previous scene.
        SceneManager.LoadScene ("GamePlay");
	}

	public void onSoundButtonClicked ()
	{
        bool isMuted = SoundManager.Instance.IsMuted();
        SoundManager.Instance.ToggleMute();
        if (isMuted)
		{
			changeButtonSprites (soundButton, soundOnUnPressed, soundOnPressed);
            SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        }
		else
		{		
			changeButtonSprites (soundButton, soundOffUnPressed, soundOffPressed);
		}
    }

	public void onRemoveAdsButtonClicked ()
	{
		// TODO:购买移除广告产品
	}

	public void onRestorePurchasesButtonClicked ()
	{
		// 只对ios有效
		#if UNITY_IPHONE
        //TODO: 
		#endif
	}

	public void onPromotionButtonClicked ()
	{
        //推广链接
		#if UNITY_ANDROID
		//Application.OpenURL ("market://details?id=games.fa.rollingcolor"); 
		#elif UNITY_IPHONE
		//Application.OpenURL ("https://itunes.apple.com/app/id1146256287");
		#endif
	}

	#endregion

	#region 支持方法

	void updateNumberOfGemsUIText()
	{
		int numberOfGems = PlayerPrefs.GetInt ("NumberOfPickUps");
		numberOfGemsText.text = "" + numberOfGems;
	}

	void changeButtonSprites(Button button, Sprite unpressedImage, Sprite pressedImage)
	{
		button.GetComponent<Image>().sprite = unpressedImage;

		SpriteState st = new SpriteState();
		st.pressedSprite = pressedImage;
		button.spriteState = st;
	}

	#endregion
}
