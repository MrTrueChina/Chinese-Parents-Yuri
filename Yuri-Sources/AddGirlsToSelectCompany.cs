using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace MtC.Mod.ChineseParents.Yuri
{
    // 这个功能制作难度太高了，先放弃吧

    //// FIXME：测试代码需要删除
    ///// <summary>
    ///// 开始选择羁绊的方法
    ///// </summary>
    //[HarmonyPatch(typeof(Panel_Boys), "SelectCompany")]
    //public static class Panel_Boys_SelectCompany
    //{
    //    private static bool Prefix(keyValueUpdate keyValue)
    //    {
    //        TipsManager.instance.AddTips("SelectCompany 方法即将调用", 1);

    //        Main.ModEntry.Logger.Log("SelectCompany 方法即将调用");

    //        Main.ModEntry.Logger.Log("key = " + keyValue.Key);
    //        Main.ModEntry.Logger.Log("value = " + keyValue.Values);
    //        if(keyValue.Values != null)
    //        {
    //            Main.ModEntry.Logger.Log("valueType = " + keyValue.Values.GetType().Name);
    //        }

    //        return true;
    //    }
    //}

    //// FIXME：测试代码需要删除
    ///// <summary>
    ///// 刷新男生列表的方法
    ///// </summary>
    //[HarmonyPatch(typeof(Panel_Boys), "Refresh")]
    //public static class Panel_Boys_Refresh
    //{
    //    private static bool Prefix()
    //    {
    //        TipsManager.instance.AddTips("Refresh 方法即将调用", 1);

    //        Main.ModEntry.Logger.Log("Refresh 方法即将调用");

    //        return true;
    //    }

    //    private static void Postfix(Panel_Boys __instance, Transform ___boysList, GameObject ___boyItem)
    //    {
    //        TipsManager.instance.AddTips("Refresh 方法调用完毕", 1);

    //        Main.ModEntry.Logger.Log("Refresh 方法调用完毕");


    //        // 把原来的方法复粘过来再执行一次看看会发生什么
    //        int num = 0;
    //        ___boysList.GetComponent<GridLayoutGroup>().enabled = true;
    //        foreach (KeyValuePair<int, BoyRecord> keyValuePair in BoysManager.Instance.BoysDictionary)
    //        {
    //            Main.ModEntry.Logger.Log("num = " + num);

    //            Item_Boys component = UnityEngine.Object.Instantiate<GameObject>(___boyItem, ___boysList).GetComponent<Item_Boys>();
    //            component.Init(keyValuePair.Key, keyValuePair.Value.loving, __instance);
    //            __instance.boys.Add(keyValuePair.Key + 1000, component);
    //            num++;
    //        }
    //    }
    //}
}
