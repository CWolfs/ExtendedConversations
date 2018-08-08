using UnityEngine;
using System;
using Harmony;

using BattleTech;
using TScript;
using TScript.Ops;
using HBS.Logging;
using HBS.Collections;

using ExtendedConversations;
using ExtendedConversations.Utils;

namespace ExtendedConversations.Core {
  public class ValueGetters {
    public static object GetBattleTechString(TsEnvironment env, object[] inputs) {
      int statScope = env.ToInt(inputs[0]);
      string statName = env.ToString(inputs[1]);
      Main.Logger.Log($"[GetBattleTechString] Triggered with scope {statScope} and statName {statName}.");

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
        string stat = statCollection.GetValue<string>(statName);
        Main.Logger.Log($"[GetBattleTechString] Stat {statName} found with value {stat}.");
        return stat;
      }
			
      Main.Logger.LogError($"[GetBattleTechString] Stat {statName} does not exist for conversation operation.");
      return null;
    }
  }
}