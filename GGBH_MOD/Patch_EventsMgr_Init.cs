using HarmonyLib;
using Il2Cpp;

namespace GGBH_MOD;

[HarmonyPatch(typeof(EventsMgr), "Init")]
public class Patch_EventsMgr_Init
{
    private static Il2CppSystem.Action<ETypeData> onGameCMDMelonLoaderCall = (System.Action<ETypeData>)OnGameCMDMelonLoader;

    private static int lastEventHashCode;

    [HarmonyPostfix]
    private static void Postfix()
    {
        if (lastEventHashCode != ((Il2CppSystem.Object)(object)g.events).GetHashCode())
        {
            lastEventHashCode = ((Il2CppSystem.Object)(object)g.events).GetHashCode();
            g.events.Off("GameCMDMelonLoader", onGameCMDMelonLoaderCall);
            g.events.On("GameCMDMelonLoader", onGameCMDMelonLoaderCall, 0, false);
        }
    }

    private static void OnGameCMDMelonLoader(ETypeData e)
    {
        ModMain.OnGameCMDMelonLoader(e);
    }
}
