using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;

public static class Utilities
{
    public static IEnumerator CRWaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    public static void ButtonClickSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
    }

    public static void RateApp()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:
                //Application.OpenURL(AppInfo.Instance.APPSTORE_LINK);
                break;

            case RuntimePlatform.Android:
                //Application.OpenURL(AppInfo.Instance.PLAYSTORE_LINK);
                break;
        }
    }

    public static void ShowMoreGames()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:
                //Application.OpenURL(AppInfo.Instance.APPSTORE_HOMEPAGE);
                break;

            case RuntimePlatform.Android:
                //Application.OpenURL(AppInfo.Instance.PLAYSTORE_HOMEPAGE);
                break;
        }
    }

    public static void OpenFacebookPage()
    {
        //Application.OpenURL(AppInfo.Instance.FACEBOOK_LINK);
    }

    public static void OpenTwitterPage()
    {
        //Application.OpenURL(AppInfo.Instance.TWITTER_LINK);
    }

    public static string EscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    /// <summary>
    /// 把DateTime作为字符串存储在PlayerPrefs中
    /// </summary>
    /// <param name="time">时间.</param>
    /// <param name="ppkey">Ppkey.</param>
    public static void StoreTime(string ppkey, DateTime time)
    {
        PlayerPrefs.SetString(ppkey, time.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 从PlayerPrefs中读取字符串并转换为DateTime，如果以前没有存储过返回默认时间
    /// </summary>
    /// <returns>The time.</returns>
    /// <param name="ppkey">Ppkey.</param>
    public static DateTime GetTime(string ppkey, DateTime defaultTime)
    {
        string storedTime = PlayerPrefs.GetString(ppkey, string.Empty);

        if (!string.IsNullOrEmpty(storedTime))
            return DateTime.FromBinary(Convert.ToInt64(storedTime));
        else
            return defaultTime;
    }
}