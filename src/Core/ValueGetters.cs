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

      StatCollection statCollection = SimHelper.GetStatCollection(statScope);
      if (statCollection == null) { // GUARD
        Main.Logger.LogError($"[GetBattleTechFloat] StatCollection is null for {statScope}");
        return null;
      }

      if (statCollection.ContainsStatistic(statName)) {
        string stat = statCollection.GetValue<string>(statName);
        Main.Logger.Log($"[GetBattleTechString] Stat {statName} found with value {stat}.");
        return stat;
      }
			
      Main.Logger.LogError($"[GetBattleTechString] Stat {statName} does not exist for conversation operation.");
      return null;
    }

    public static object GetBattleTechInt(TsEnvironment env, object[] inputs) {
      int statScope = env.ToInt(inputs[0]);
      string statName = env.ToString(inputs[1]);
      Main.Logger.Log($"[GetBattleTechInt] Triggered with scope {statScope} and statName {statName}.");

      StatCollection statCollection = SimHelper.GetStatCollection(statScope);
      if (statCollection == null) { // GUARD
        Main.Logger.LogError($"[GetBattleTechFloat] StatCollection is null for {statScope}");
        return null;
      }

      if (statCollection.ContainsStatistic(statName)) {
        int stat = statCollection.GetValue<int>(statName);
        Main.Logger.Log($"[GetBattleTechInt] Stat {statName} found with value {stat}.");
        return stat;
      }
			
      Main.Logger.LogError($"[GetBattleTechInt] Stat {statName} does not exist for conversation operation.");
      return null;
    }

    public static object GetBattleTechFloat(TsEnvironment env, object[] inputs) {
      int statScope = env.ToInt(inputs[0]);
      string statName = env.ToString(inputs[1]);
      Main.Logger.Log($"[GetBattleTechFloat] Triggered with scope {statScope} and statName {statName}.");

      StatCollection statCollection = SimHelper.GetStatCollection(statScope);
      if (statCollection == null) { // GUARD
        Main.Logger.LogError($"[GetBattleTechFloat] StatCollection is null for {statScope}");
        return null;
      }

      if (statCollection.ContainsStatistic(statName)) {
        float stat = statCollection.GetValue<float>(statName);
        Main.Logger.Log($"[GetBattleTechFloat] Stat {statName} found with value {stat}.");
        return stat;
      }
			
      Main.Logger.LogError($"[GetBattleTechFloat] Stat {statName} does not exist for conversation operation.");
      return null;
    }
  }
}