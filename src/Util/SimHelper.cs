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
  }
}