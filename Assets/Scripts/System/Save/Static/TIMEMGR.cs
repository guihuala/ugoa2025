using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TIMEMGR
{
    private static float sessionStartTime;    // 当前运行开始时间戳
    private static float accumulatedTime;     // 累积游戏时间（不包括本次运行）

    // 游戏开始时调用
    public static void Init()
    {
        sessionStartTime = Time.realtimeSinceStartup;
        accumulatedTime = SaveManager.Instance.gameTime;
    }

    // 获取当前总游戏时间（单位：秒）
    public static float GetCurrentGameTime()
    {
        return accumulatedTime + (Time.realtimeSinceStartup - sessionStartTime);
    }

    // 保存当前总游戏时间
    public static void SaveCurrentGameTime()
    {
        SaveManager.Instance.gameTime = GetCurrentGameTime();
    }

    // 获取格式化时间字符串 00:00:00
    public static string GetFormatTime(int seconds)
    {
        TimeSpan ts = new TimeSpan(0, 0, seconds);
        return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
    }

    // 将 8 位数字日期转换为 YYYY/MM/DD 格式
    public static void SetDate(ref string date)
    {
        if (date.Length >= 8)
        {
            date = date.Insert(4, "/").Insert(7, "/");
        }
    }

    // 将 6 位数字时间转换为 HH:MM:SS 格式
    public static void SetTime(ref string time)
    {
        if (time.Length >= 6)
        {
            time = time.Insert(2, ":").Insert(5, ":");
        }
    }
}