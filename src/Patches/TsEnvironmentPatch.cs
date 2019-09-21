using Harmony;

using TScript;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(TScript.Ops.BattleTech), "Declare")]
  public class TsEnvironmentPatch {
    static void Postfix(TScript.Ops.BattleTech __instance, TsEnvironment env) {
      ConversationUpgrades.Declare(env);
    }
  }
}