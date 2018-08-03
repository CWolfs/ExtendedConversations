using UnityEngine;
using System;
using Harmony;

using BattleTech;
using TScript;
using TScript.Ops;
using HBS.Logging;
using HBS.Collections;

using ExtendedConversations;

namespace ExtendedConversations.Core {
  public class ConversationUpgrades {
    public static void Declare(TsEnvironment env) {
      Main.Logger.Log("Declaring conversation updates");

      TsType intType = env.GetType("int");
      TsType boolType = env.GetType("bool");
      TsType stringType = env.GetType("string");

      TsType HasOrHasNotType = env.GetType("HasOrHasNot");
      TsType SenseTagListType = env.GetType("SenseTagList");
      TsType SimGameScopeType = env.GetType("SimGameScope");

      /*
      * CONDITIONS
      */

      // Evaluate Tag for Current System
      Main.Logger.Log("Declaring 'Evaluate Tag for Current System' condition");
      TsOp tsOp = env.DeclareOp("ConditionFunction", "Evaluate Tag for Current System", boolType, new TsOp.EvalDelegate(ConversationUpgrades.EvaluateTagForCurrentSystem));
      tsOp.DeclareInput("comp", HasOrHasNotType);
      tsOp.DeclareInput("tagName", SenseTagListType);

      // TODO: Add `Evaluate BattleTech Int` condition
      Main.Logger.Log("Declaring 'Evaluate BattleTech Int' condition");
      tsOp = env.DeclareOp("ConditionFunction", "Evaluate BattleTech Int", boolType, new TsOp.EvalDelegate(ConversationUpgrades.EvaluateBattleTechInt));
			tsOp.DeclareInput("scope", SimGameScopeType);
			tsOp.DeclareInput("param", stringType);
      tsOp.DeclareInput("operation", intType);
			tsOp.DeclareInput("value", intType);

      // TODO: Add `Evaluate BattleTech Float` condition

      // TODO: Add `Evaluate BattleTech String` condition

      // TODO: Add `Evaluate Money` condition

      /*
      * ACTIONS
      */

      // TODO: Add `TimeSkip` action

      // TODO: Add 'Dropship Location' action

      // TODO: Add 'Modify Money' action

      Main.Logger.Log("Finished declaring conversation upgrades");
    }

    /* CONDITIONS */
    public static object EvaluateTagForCurrentSystem(TsEnvironment env, object[] inputs) {
      Main.Logger.Log("EvaluateTagForCurrentSystem triggered");
      bool flag = env.ToBool(inputs[0]);
      string value = env.ToString(inputs[1]);
      TagSet currentSystemTags = UnityGameInstance.BattleTechGame.Simulation.CurSystem.Tags;
      bool flag2 = currentSystemTags.Contains(value) == flag;
      Main.Logger.Log("EvaluateTagForCurrentSystem finished with result of " + flag2);
      return flag2;
    }

    public static object EvaluateBattleTechInt(TsEnvironment env, object[] inputs) {
      int statScope = env.ToInt(inputs[0]);
      string statName = env.ToString(inputs[1]);
      int operation = env.ToInt(inputs[2]);
      int compareValue = env.ToInt(inputs[3]);
      Main.Logger.Log($"[EvaluateBattleTechInt] Triggered with scope '{statScope}', statName '{statName}', operation '{operation}', compareValue '{compareValue}");

      StatCollection statCollection;
      switch (statScope) {
        case 1:
          statCollection = UnityGameInstance.BattleTechGame.Simulation.CompanyStats;
          break;
        case 2:
          statCollection = UnityGameInstance.BattleTechGame.Simulation.CommanderStats;
          break;
        case 3:
          statCollection = UnityGameInstance.BattleTechGame.Simulation.CurSystem.Stats;
          break;
        default:
          return false;
      }

      if (statCollection.ContainsStatistic(statName)) {
        int stat = statCollection.GetValue<int>(statName);
        
        switch (operation) {
          case 1: // less than
            return (stat < compareValue);
          case 2: // equal to
            return (stat == compareValue);
          case 3: // greater than
            return (stat > compareValue);
          case 4: // less than or equal to
            return (stat <= compareValue);
          case 5: // greater than or equal to
            return (stat >= compareValue);
          default:
            return false;
        }
      }

      Main.Logger.Log($"[EvaluateBattleTechInt] Stat {statName} does not exist for conversation operation.");
      return false;
    }
  }
}