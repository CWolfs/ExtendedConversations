using Harmony;
using TScript;
using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(TsNode), "Eval")]
  public class TsNodeEvalPatch {
    static void Prefix(TsNode __instance) {
      if (Main.Settings == null || !Main.Settings.EnableDebugLogging) return;

      string opName = __instance.getOp()?.GetName() ?? "unknown";

      // Check if we're evaluating an action or a condition
      bool isAction = Actions.IsLinkAction || Actions.IsNodeAction;

      if (isAction && Main.Settings.DebugLogActions) {
        Main.Logger.Log($"    [TsNode.Eval] Action: {opName}");
      } else if (!isAction && Main.Settings.DebugLogConditions) {
        Main.Logger.Log($"    [TsNode.Eval] Condition: {opName}");
      }
    }

    static void Postfix(TsNode __instance, object __result, TsEnvironment env) {
      if (Main.Settings == null || !Main.Settings.EnableDebugLogging) return;

      // Only log condition results (actions return void or other types)
      bool isAction = Actions.IsLinkAction || Actions.IsNodeAction;

      if (!isAction && Main.Settings.DebugLogConditions) {
        bool boolResult = env.ToBool(__result);
        Main.Logger.Log($"    [TsNode.Eval] Condition result: {boolResult}");
      }
    }
  }
}
