using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;

using BattleTech;
using BattleTech.UI;
using TScript;
using TScript.Ops;
using HBS.Logging;
using HBS.Collections;
using isogame;

using ExtendedConversations;
using ExtendedConversations.Utils;

namespace ExtendedConversations.Core {
  public class Actions {
    public static bool MovedKameraInLeopardCommandCenter = false;

    public static object TimeSkip(TsEnvironment env, object[] inputs) {
      int daysToSkip = env.ToInt(inputs[0]);
      Main.Logger.Log($"[TimeSkip] Triggered with days to skip '{daysToSkip}'");

      SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

      for (int i = 0; i < daysToSkip; i++) {
        ReflectionHelper.InvokePrivateMethod(simulation, "OnDayPassed", new object[] { 0 });
      }

      Main.Logger.Log($"[TimeSkip] Skip complete");
      return null;
    }

    public static object SetCurrentSystem(TsEnvironment env, object[] inputs) {
      string systemName = env.ToString(inputs[0]);
      bool includeTravelTime = env.ToBool(inputs[1]);
      Main.Logger.Log($"[SetCurrentSystem] Travelling to '{systemName}' and includeTravelTime is '{includeTravelTime}'");

      SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

      if (includeTravelTime) {
        simulation.TravelToSystemByString(systemName, true);
      } else {
        StarSystemNode systemNode = simulation.Starmap.GetSystemByID(systemName);
        ReflectionHelper.SetReadOnlyProperty(simulation, "CurSystem", systemNode.System);
        simulation.SetCurrentSystem(systemNode.System, true, false);
      }

      Main.Logger.Log($"[SetCurrentSystem] Travel complete");
      return null;
    }

    public static object ModifyFunds(TsEnvironment env, object[] inputs) {
      int operation = env.ToInt(inputs[0]);
      int amount = env.ToInt(inputs[1]);
      Main.Logger.Log($"[ModifyFunds] Operation '{operation}' with amount '{amount}'.'");

      SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

      if (operation == 0) { // ADD
        simulation.AddFunds(amount);
      } else if (operation == 1) { // REMOVE
        simulation.AddFunds(-amount);
      } else {
        Main.Logger.LogError($"[ModifyFunds] Unknown operation type of '{operation}'");
        return null;
      }

      Main.Logger.Log($"[ModifyFunds] Funds modified.");
      return null;
    }

    public static object SetCharactersVisible(TsEnvironment env, object[] inputs) {
      bool isVisible = env.ToBool(inputs[0]);
      string crewNamesGrouped = env.ToString(inputs[1]);
      Main.Logger.Log($"[SetCharactersVisible] crewnames '{crewNamesGrouped}' will be visible status {isVisible}.");

      SimGameState simGameState = UnityGameInstance.Instance.Game.Simulation;
      string[] crewNames = crewNamesGrouped.Split(',');

      // Fix for Leopard Kamea/Alex structure being wrong
      if (!MovedKameraInLeopardCommandCenter) {
        if (simGameState.CurDropship == DropshipType.Leopard && simGameState.CurRoomState == DropshipLocation.CMD_CENTER) {
          if (crewNamesGrouped.Contains("Kamea") || crewNamesGrouped.Contains("Alexander")) {
            // Move Kamea GO out of Alex
            Main.Logger.Log("[SetCharactersVisible] Moving Kamera so she can be enabled on her own");
            Transform kameraTransform = GameObject.Find("LeopardHub").transform.Find("chrPrfCrew_alexander/chrPrfCrew_kamea (2)");

            GameObject kameaContainer = new GameObject("Kamea");
            kameaContainer.transform.parent = kameraTransform.parent.parent;

            kameraTransform.parent = kameaContainer.transform;
            MovedKameraInLeopardCommandCenter = true;
          }
        }
      }

      if (simGameState.CurRoomState == DropshipLocation.CONFERENCE) {
        if (simGameState.CameraController.flashPoint == simGameState.CameraController.CurrentRoomProps) {
          if (crewNamesGrouped.Contains("Kamea") || crewNamesGrouped.Contains("Alexander")) {
            // Check if Kamea and Alex exist, if not - copy them from another Conferernce Room
            // GameObject flashpointConferenceRoomGO = GameObject.Find("FlashpointConference");
            // bool charactersExist = flashpointConferenceRoomGO.transform.Find("Kamea") != null;

            // if (!charactersExist) {
            //   GameObject originalKamea = GameObject.Find("LeopardConference").transform.Find("Kamea").gameObject;
            //   GameObject originalAlexander = GameObject.Find("LeopardConference").transform.Find("Alexander").gameObject;

            //   GameObject copiedKamea = GameObject.Instantiate(originalKamea, flashpointConferenceRoomGO.transform);
            //   copiedKamea.name = "Kamea";
            //   copiedKamea.transform.position = new Vector3(copiedKamea.transform.position.x, -20.317f, copiedKamea.transform.position.z);
            //   foreach (Transform child in copiedKamea.transform) {
            //     child.gameObject.SetActive(true);
            //   }

            //   GameObject copiedAlexander = GameObject.Instantiate(originalAlexander, flashpointConferenceRoomGO.transform);
            //   copiedAlexander.name = "Alexander";
            //   copiedAlexander.transform.position = new Vector3(copiedAlexander.transform.position.x, -20.317f, copiedAlexander.transform.position.z);
            //   foreach (Transform child in copiedAlexander.transform) {
            //     child.gameObject.SetActive(true);
            //   }
            // }
          }
        }
      }

      foreach (string crewName in crewNames) {
        SimGameState.SimGameCharacterType character = (SimGameState.SimGameCharacterType)Enum.Parse(typeof(SimGameState.SimGameCharacterType), crewName, true);
        SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
        simulation.SetCharacterVisibility(character, isVisible);
      }

      Main.Logger.Log($"[SetCharactersVisible] Finished");
      return null;
    }

    public static object StartConversation(TsEnvironment env, object[] inputs) {
      string conversationId = env.ToString(inputs[0]);
      string groupHeader = env.ToString(inputs[1]);
      string groupSubHeader = env.ToString(inputs[2]);
      Main.Logger.Log($"[StartConversation] conversationId '{conversationId}' with groupHeader '{groupHeader}' and groupSubHeader '{groupSubHeader}'.");

      SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
      Conversation conversation = null;

      try {
        conversation = simulation.DataManager.SimGameConversations.Get(conversationId);
      } catch (KeyNotFoundException) {
        Main.Logger.Log($"[StartConversation] Conversation with id '{conversationId}' not found. Check the conversation id is correct or/and if the conversation has loaded correctly.");
      }

      if (conversation == null) {
        Main.Logger.Log($"[StartConversation] Conversation is null for id '{conversationId}'");
      } else {
        simulation.ConversationManager.OneOnOneDialogInterrupt();
        UnityGameInstance.Instance.StartCoroutine(WaitThenQueueConversation(simulation, conversation, groupHeader, groupSubHeader));
        Main.Logger.Log($"[StartConversation] Conversaton queued for immediate start.");
      }

      return null;
    }

    static IEnumerator WaitThenQueueConversation(SimGameState simulation, Conversation conversation, string groupHeader, string groupSubHeader) {
      yield return new WaitForSeconds(1);
      SimGameInterruptManager interruptManager = (SimGameInterruptManager)ReflectionHelper.GetPrivateField(simulation, "interruptQueue");
      interruptManager.QueueConversation(conversation, groupHeader, groupSubHeader, null, true);
    }

    public static object AddContract(TsEnvironment env, object[] inputs) {
      string contractId = env.ToString(inputs[0]);
      string target = env.ToString(inputs[1]);
      string employer = env.ToString(inputs[2]);
      string possibleLocation = env.ToString(inputs[3]);
      bool global = false;
      string location = null;

      SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
      StarSystem currentSystem = simulation.CurSystem;

      // Only global if the modder has entered in a location for the action, and it's not the same as the current system
      if ((possibleLocation != "0") && (location != currentSystem.ID)) {
        global = true;
        location = possibleLocation;
      }

      SimGameState.AddContractData contractData = new SimGameState.AddContractData();
      contractData.ContractName = contractId;   // "SimpleBattle_LastMechStanding"
      contractData.Target = target;             // "TaurianConcordat"
      contractData.Employer = employer;         // "AuriganRestoration"
      contractData.IsGlobal = global;           // true
      contractData.TargetSystem = location;     // "starsystemdef_Itrom"
      simulation.AddContract(contractData);

      return null;
    }
  }
}