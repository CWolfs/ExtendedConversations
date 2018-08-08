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
  public class Conditions {
    public static object EvaluateTagForCurrentSystem(TsEnvironment env, object[] inputs) {
      bool flag = env.ToBool(inputs[0]);
      string value = env.ToString(inputs[1]);
      Main.Logger.Log($"[EvaluateTagForCurrentSystem] Triggered with flag {flag} and value {value}.");

      TagSet currentSystemTags = UnityGameInstance.BattleTechGame.Simulation.CurSystem.Tags;
      bool flag2 = currentSystemTags.Contains(value) == flag;
      Main.Logger.Log("[EvaluateTagForCurrentSystem] Finished with result of " + flag2);
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

    public static object EvaluateFunds(TsEnvironment env, object[] inputs) {
      int operation = env.ToInt(inputs[0]);
      int moneyCheckValue = env.ToInt(inputs[1]);
      int funds = UnityGameInstance.BattleTechGame.Simulation.Funds;

      switch (operation) {
        case 1: // less than
          return (funds < moneyCheckValue);
        case 2: // equal to
          return (funds == moneyCheckValue);
        case 3: // greater than
          return (funds > moneyCheckValue);
        case 4: // less than or equal to
          return (funds <= moneyCheckValue);
        case 5: // greater than or equal to
          return (funds >= moneyCheckValue);
        default:
          return false;
      }
    }
  }
}