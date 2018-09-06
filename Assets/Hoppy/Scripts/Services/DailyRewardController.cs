using UnityEngine;
using System.Collections;
using System;

public class DailyRewardController : MonoBehaviour
{
    public static DailyRewardController Instance { get; private set; }

    public DateTime NextRewardTime
    {
        get
        {
            return GetNextRewardTime();
        }
    }

    public TimeSpan TimeUntilReward
    {
        get
        {
            return NextRewardTime.Subtract(DateTime.Now);
        }
    }

    [Header("检查是否禁用每日奖励")]
    public bool disable;

    [Header("每日奖励配置")]
    [Tooltip("每日奖励间隔小时")]
    public int rewardIntervalHours = 6;
    [Tooltip("每日奖励间隔分钟")]
    public int rewardIntervalMinutes = 0;
    [Tooltip("每日奖励间隔秒数")]
    public int rewardIntervalSeconds = 0;
    public int minRewardValue = 20;
    public int maxRewardValue = 50;

    private const string NextRewardTimePPK = "SGLIB_NEXT_DAILY_REWARD_TIME";

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// 确定是否可以领取每日奖励.
    /// </summary>
    public bool CanRewardNow()
    {
        return TimeUntilReward <= TimeSpan.Zero;
    }

    /// <summary>
    /// 得到每日奖励奖励随机数
    /// </summary>
    /// <returns>随机奖励.</returns>
    public int GetRandomReward()
    {
        return UnityEngine.Random.Range(minRewardValue, maxRewardValue + 1);
    }

    /// <summary>
    /// 根据跟定的小时分钟秒设置下个每日奖励的时间
    /// </summary>
    public void ResetNextRewardTime()
    {
        DateTime next = DateTime.Now.Add(new TimeSpan(rewardIntervalHours, rewardIntervalMinutes, rewardIntervalSeconds));
        StoreNextRewardTime(next);
    }

    void StoreNextRewardTime(DateTime time)
    {
        PlayerPrefs.SetString(NextRewardTimePPK, time.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    DateTime GetNextRewardTime()
    {
        string storedTime = PlayerPrefs.GetString(NextRewardTimePPK, string.Empty);

        if (!string.IsNullOrEmpty(storedTime))
            return DateTime.FromBinary(Convert.ToInt64(storedTime));
        else
            return DateTime.Now;
    }
}
