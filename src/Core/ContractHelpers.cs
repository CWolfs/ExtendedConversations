using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using BattleTech;
using BattleTech.Data;
using BattleTech.Framework;

namespace ExtendedConversations.Core {
  // Struct for map and encounter selection results
  public struct MapSelection {
    public string MapName;
    public string MapPath;
    public Biome.BIOMESKIN BiomeSkin;
    public string EncounterGuid;
  }

  // Struct for target faction selection results
  public struct TargetSelection {
    public FactionValue Target;
    public FactionValue TargetAlly;
    public FactionValue EmployerAlly;
  }

  public static class ContractHelpers {
    // Get random valid employer for a system
    public static FactionValue GetRandomEmployerForSystem(SimGameState simulation, StarSystem system) {
      List<string> validEmployers = system.Def.ContractEmployerIDList
        .Where(e => !simulation.ignoredContractEmployers.Contains(e))
        .ToList();

      if (validEmployers.Count == 0) {
        Main.Logger.LogWarning($"[GetRandomEmployerForSystem] No valid employers found for system {system.Name}, using first faction");
        return FactionEnumeration.FactionList[0];
      }

      string randomEmployerId = validEmployers[UnityEngine.Random.Range(0, validEmployers.Count)];
      return FactionEnumeration.GetFactionByName(randomEmployerId);
    }

    // Get random valid target for an employer
    public static TargetSelection GetRandomTargetForEmployer(
      SimGameState simulation, StarSystem system, FactionValue employer) {

      FactionDef employerDef = employer.FactionDef;

      // Get valid targets: must be in system's target list AND be enemies of the employer
      List<string> validTargets = employerDef.Enemies
        .Where(t => system.Def.ContractTargetIDList.Contains(t) &&
                    !simulation.IgnoredContractTargets.Contains(t))
        .ToList();

      if (validTargets.Count == 0) {
        Main.Logger.LogWarning($"[GetRandomTargetForEmployer] No valid targets found for employer {employer.Name}, using first available target");
        return new TargetSelection {
          Target = system.Def.ContractTargetIDList.Count > 0
            ? FactionEnumeration.GetFactionByName(system.Def.ContractTargetIDList[0])
            : FactionEnumeration.FactionList[1],
          TargetAlly = FactionEnumeration.GetInvalidUnsetFactionValue(),
          EmployerAlly = FactionEnumeration.GetInvalidUnsetFactionValue()
        };
      }

      string randomTargetId = validTargets[UnityEngine.Random.Range(0, validTargets.Count)];
      FactionValue target = FactionEnumeration.GetFactionByName(randomTargetId);

      // For simplicity, not setting allies - the game will handle it
      return new TargetSelection {
        Target = target,
        TargetAlly = FactionEnumeration.GetInvalidUnsetFactionValue(),
        EmployerAlly = FactionEnumeration.GetInvalidUnsetFactionValue()
      };
    }

    // Get random map for system and contract type
    public static MapSelection GetRandomMapForSystem(
      StarSystem system, ContractTypeValue contractType) {

      // Get valid maps for the system
      List<MapAndEncounters> playableMaps = MetadataDatabase.Instance
        .GetReleasedMapsAndEncountersBySinglePlayerProceduralContractTypeAndTags(
          system.Def.MapRequiredTags,
          system.Def.MapExcludedTags,
          system.Def.SupportedBiomes,
          includeOwnershipCheck: true
        );

      if (playableMaps.Count == 0) {
        Main.Logger.LogWarning($"[GetRandomMapForSystem] No playable maps found for system {system.Name}, using fallback");
        // Fallback to any map for this contract type
        playableMaps = MetadataDatabase.Instance.GetReleasedMapsAndEncountersByContractTypeAndOwnership(
          contractType.ID,
          includeUnpublishedContractTypes: false
        );
      }

      if (playableMaps.Count == 0) {
        throw new Exception($"No maps available for contract type {contractType.Name}");
      }

      MapAndEncounters selectedMap = playableMaps[UnityEngine.Random.Range(0, playableMaps.Count)];

      // Get random encounter for this contract type
      string encounterGuid = GetRandomEncounterForMap(selectedMap, contractType);

      return new MapSelection {
        MapName = selectedMap.Map.MapName,
        MapPath = selectedMap.Map.MapPath,
        BiomeSkin = selectedMap.Map.BiomeSkinEntry.BiomeSkin,
        EncounterGuid = encounterGuid
      };
    }

    // Get random encounter for a map and contract type
    public static string GetRandomEncounterForMap(MapAndEncounters mapAndEncounters, ContractTypeValue contractType) {
      List<EncounterLayer_MDD> validEncounters = new List<EncounterLayer_MDD>();

      foreach (EncounterLayer_MDD encounter in mapAndEncounters.Encounters) {
        if (encounter.ContractTypeRow.ContractTypeID == contractType.ID) {
          validEncounters.Add(encounter);
        }
      }

      if (validEncounters.Count == 0) {
        Main.Logger.LogWarning($"[GetRandomEncounterForMap] No encounters found for contract type {contractType.Name} on map, using first available");
        return mapAndEncounters.Encounters[0].EncounterLayerGUID;
      }

      return validEncounters[UnityEngine.Random.Range(0, validEncounters.Count)].EncounterLayerGUID;
    }

    // Select map for contract with optional specific map ID and fallback to random
    public static MapSelection SelectMapForContract(
      string mapIdInput, StarSystem targetSystem, ContractTypeValue contractType) {

      string mapName;
      string mapPath;
      string encounterGuid;
      Biome.BIOMESKIN biomeSkin;

      if (!string.IsNullOrEmpty(mapIdInput)) {
        // Use specified map
        Map_MDD specifiedMap = MetadataDatabase.Instance.GetMapByPath(mapIdInput, checkOwnership: true);
        if (specifiedMap != null) {
          mapName = specifiedMap.MapName;
          mapPath = specifiedMap.MapPath;
          biomeSkin = specifiedMap.BiomeSkinEntry.BiomeSkin;

          // Get encounters for this map - need to fetch MapAndEncounters
          List<MapAndEncounters> mapsWithEncounters = MetadataDatabase.Instance
            .GetReleasedMapsAndEncountersByContractTypeAndOwnership(
              contractType.ID,
              includeUnpublishedContractTypes: false);

          MapAndEncounters matchingMap = mapsWithEncounters.FirstOrDefault(m => m.Map.MapPath == mapPath);
          if (matchingMap != null) {
            encounterGuid = GetRandomEncounterForMap(matchingMap, contractType);
            Main.Logger.Log($"[SelectMapForContract] Selected specified map: {mapName}");
          } else {
            Main.Logger.LogWarning($"[SelectMapForContract] No encounters found for specified map, using random map");
            MapSelection randomMap = GetRandomMapForSystem(targetSystem, contractType);
            mapName = randomMap.MapName;
            mapPath = randomMap.MapPath;
            biomeSkin = randomMap.BiomeSkin;
            encounterGuid = randomMap.EncounterGuid;
          }
        } else {
          Main.Logger.LogWarning($"[SelectMapForContract] Map '{mapIdInput}' not found, using random map");
          MapSelection randomMap = GetRandomMapForSystem(targetSystem, contractType);
          mapName = randomMap.MapName;
          mapPath = randomMap.MapPath;
          biomeSkin = randomMap.BiomeSkin;
          encounterGuid = randomMap.EncounterGuid;
        }
      } else {
        // Random map for the system
        MapSelection randomMap = GetRandomMapForSystem(targetSystem, contractType);
        mapName = randomMap.MapName;
        mapPath = randomMap.MapPath;
        biomeSkin = randomMap.BiomeSkin;
        encounterGuid = randomMap.EncounterGuid;
        Main.Logger.Log($"[SelectMapForContract] Selected random map: {mapName}");
      }

      return new MapSelection {
        MapName = mapName,
        MapPath = mapPath,
        BiomeSkin = biomeSkin,
        EncounterGuid = encounterGuid
      };
    }

    // Calculate player-appropriate difficulty
    public static int GetPlayerAppropriateDifficulty(SimGameState simulation, StarSystem system) {
      // Use the same logic as vanilla game's GetContractRangeDifficultyRange
      int baseDiff = system.Def.GetDifficulty(simulation.SimGameMode) + Mathf.FloorToInt(simulation.GlobalDifficulty);

      // Get variance from constants (typically 1)
      int variance = simulation.Constants.Story.ContractDifficultyVariance;

      int minDiff = Mathf.Max(1, baseDiff - variance);
      int maxDiff = Mathf.Max(1, baseDiff + variance);

      // Random difficulty in the player-appropriate range
      return UnityEngine.Random.Range(minDiff, maxDiff + 1);
    }
  }
}
