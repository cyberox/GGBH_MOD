using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Il2Cpp;
using Il2CppEGameTypeData;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;

namespace GGBH_MOD;

public class ModMain : MelonMod
{
    private class ModItem
    {
        public string modNamespace;

        public List<Assembly> assembly = new List<Assembly>();

        public object modMainBase;

        public bool isLoad;
    }

    private static Dictionary<string, ModItem> allMod = new Dictionary<string, ModItem>();

    private static int initModMgrHashCode;

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        File.WriteAllText(System.Environment.CurrentDirectory + "/MelonLoader/GGBH_MOD_LOAD_COMPLETE.txt", "");
    }

    public static void OnGameCMDMelonLoader(ETypeData e)
    {
        CMDMelonLoader cMDMelonLoader = ((Il2CppObjectBase)(object)e).Cast<CMDMelonLoader>();
        if (cMDMelonLoader.cmd == "LoadDll")
        {
            string text = cMDMelonLoader.values[0].ToString();
            string text2 = cMDMelonLoader.values[1].ToString();
            try
            {
                MelonAssembly melonAssembly = MelonAssembly.LoadMelonAssembly(text, true);
                if (!allMod.ContainsKey(text2))
                {
                    ModItem modItem = new ModItem();
                    modItem.modNamespace = text2;
                    allMod[text2] = modItem;
                }
                allMod[text2].assembly.Add(melonAssembly.Assembly);
                return;
            }
            catch (System.Exception)
            {
                return;
            }
        }
        if (cMDMelonLoader.cmd == "InitModMain")
        {
            if (initModMgrHashCode == ((Il2CppSystem.Object)(object)g.mod).GetHashCode())
            {
                return;
            }
            initModMgrHashCode = ((Il2CppSystem.Object)(object)g.mod).GetHashCode();
            foreach (KeyValuePair<string, ModItem> item2 in allMod)
            {
                if (item2.Value.modMainBase != null && item2.Value.isLoad)
                {
                    try
                    {
                        item2.Value.isLoad = false;
                        System.Console.WriteLine("DestroyModMain: " + item2.Key);
                        item2.Value.modMainBase.GetType().GetMethod("Destroy").Invoke(item2.Value.modMainBase, new object[0]);
                        item2.Value.modMainBase = null;
                    }
                    catch (System.Exception ex2)
                    {
                        System.Console.WriteLine(ex2.ToString());
                    }
                }
            }
            List<string> list = new List<string>();
            list.Add(cMDMelonLoader.values.Count.ToString());
            for (int i = 0; i < cMDMelonLoader.values.Count; i++)
            {
                if (cMDMelonLoader.values[i] != null)
                {
                    list.Add(cMDMelonLoader.values[i].ToString());
                }
            }
            {
                foreach (KeyValuePair<string, ModItem> item3 in allMod)
                {
                    if (!list.Contains(item3.Value.modNamespace))
                    {
                        continue;
                    }
                    if (item3.Value.modMainBase == null && !item3.Value.isLoad)
                    {
                        item3.Value.isLoad = true;
                        for (int j = 0; j < item3.Value.assembly.Count; j++)
                        {
                            if (item3.Value.assembly[j].GetType(item3.Key + ".ModMain") != null)
                            {
                                object obj = item3.Value.assembly[j].CreateInstance(item3.Key + ".ModMain");
                                if (obj == null)
                                {
                                    System.Console.WriteLine("Failed to initialize class \"" + item3.Key + ".ModMain\"");
                                }
                                else if (obj == null)
                                {
                                    System.Console.WriteLine("Class \"" + item3.Key + ".ModMain\" does not inherit from ModMainBase");
                                }
                                else if (obj != null)
                                {
                                    item3.Value.modMainBase = obj;
                                    break;
                                }
                            }
                        }
                        if (item3.Value.modMainBase == null)
                        {
                            System.Console.WriteLine("Type \"" + item3.Key + ".ModMain\" not found");
                        }
                    }
                    if (item3.Value.modMainBase != null)
                    {
                        try
                        {
                            System.Console.WriteLine("InitModMain: " + item3.Key);
                            item3.Value.modMainBase.GetType().GetMethod("Init").Invoke(item3.Value.modMainBase, new object[0]);
                        }
                        catch (System.Exception ex3)
                        {
                            System.Console.WriteLine(ex3.ToString());
                        }
                    }
                }
                return;
            }
        }
        _ = cMDMelonLoader.cmd == "DestroyModMain";
    }
}
