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
        default:
          return null;
      }
    }
  }
}