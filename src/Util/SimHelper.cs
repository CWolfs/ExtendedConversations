using UnityEngine;
using System;

using BattleTech;

namespace ExtendedConversations.Utils {
  public class SimHelper {
    public static StatCollection GetStatCollection(int scope) {
      switch (scope) {
        case 1:
          return UnityGameInstance.BattleTechGame.Simulation.CompanyStats;
        case 2:
          return UnityGameInstance.BattleTechGame.Simulation.CommanderStats;
        case 3:
          return UnityGameInstance.BattleTechGame.Simulation.CurSystem.Stats;
        case 4:
          if (UnityGameInstance.BattleTechGame.Simulation.ActiveFlashpoint == null) {
            return null;
          }
          return UnityGameInstance.BattleTechGame.Simulation.ActiveFlashpoint.Stats;
        default:
          return null;
      }
    }

    public static StatCollection GetStatCollection(string scope) {
      string lowerCaseScope = scope.ToLower();

      switch (lowerCaseScope) {
        case "company":
          return GetStatCollection(1);
        case "commander":
          return GetStatCollection(2);
        case "currentsystem":
          return GetStatCollection(3);
        case "flashpoint":
          return GetStatCollection(4);
        default:
          return null;
      }
    }
  }
}