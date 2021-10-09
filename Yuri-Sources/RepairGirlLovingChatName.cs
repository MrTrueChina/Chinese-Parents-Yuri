using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 获取多语种字符串的方法，几乎所有显示的文本都来自这个方法。女儿版获取儿子版的女同学名称时会获取错误，在这里进行修复
    /// </summary>
    [HarmonyPatch(typeof(XmlData), "GetStringLanguage", new Type[] { typeof(string), typeof(bool) })]
    public static class XmlData_GetStringLanguage_name_sex
    {
        /// <summary>
        /// 在前后缀之间传递参数的类
        /// </summary>
        internal class GetStringLanguageParams
        {
            public string name;
            public bool sex;

            public GetStringLanguageParams(string name, bool sex = false)
            {
                this.name = name;
                this.sex = sex;
            }
        }

        private static void Prefix(out GetStringLanguageParams __state, string name, ref bool sex)
        {
            // 向后缀传递参数，虽然后缀也可以像前缀一样用同名参数获取方法参数，但这个方法内部修改了参数，所以需要在前缀备份一份直接发给后缀
            __state = new GetStringLanguageParams(name, sex);
        }

        private static void Postfix(XmlData __instance, GetStringLanguageParams __state, ref string __result)
        {
            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                return;
            }

            // 这一代是儿子则不处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                return;
            }

            // 不是区分性别的获取对话人物名称，不作处理
            if (!"player".Equals(__state.name) || !__state.sex)
            {
                return;
            }

            // 如果返回的是错误的名称，使用无视性别的方式再获取一次
            List<string> wrongNames = new List<string>() { "女孩1", "女孩2", "女孩3", "女孩4", "女孩5", "女孩6", "女孩7", "女孩8", "女孩9" };
            if (wrongNames.Contains(__result))
            {
                __result = __instance.GetStringLanguage(__state.name, false);
                return;
            }
        }
    }
}
