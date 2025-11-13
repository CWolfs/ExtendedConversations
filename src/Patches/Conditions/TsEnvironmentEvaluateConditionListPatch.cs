using System.Collections.Generic;
using Harmony;
using TScript;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(TsEnvironment), "evaluateConditionList")]
  public class TsEnvironmentEvaluateConditionListPatch {
    static void Prefix(List<TsNode> conditions) {
      if (Main.Settings == null || !Main.Settings.EnableDebugLogging || !Main.Settings.DebugLogConditions) return;

      Main.Logger.Log($"[EvaluateConditionList] Evaluating {conditions.Count} condition(s) with AND logic:");
      for (int i = 0; i < conditions.Count; i++) {
        string opName = conditions[i].getOp()?.GetName() ?? "unknown";
        Main.Logger.Log($"  [{i}] {opName}");
      }
    }

    static void Postfix(bool __result) {
      if (Main.Settings == null || !Main.Settings.EnableDebugLogging || !Main.Settings.DebugLogConditions) return;

      Main.Logger.Log($"[EvaluateConditionList] FINAL RESULT: {__result}");
      if (!__result) {
        Main.Logger.Log($"  -> At least one condition failed (AND logic)");
      }
      Main.Logger.Log(""); // Add spacing after condition list evaluation
    }
  }
}
