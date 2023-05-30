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
using ExtendedConversations.State;

namespace ExtendedConversations.Core {
  public class Actions {
    public static bool IsNodeAction { get; set; } = false;
    public static bool IsLinkAction { get; set; } = false; // Response or Root

    // TODO: Move all this into a state class
    public static bool MovedKameraInLeopardCommandCenter = false;
    public static bool ForceNextIsInFlashpointCheckFalse = false;
    public static Conversation ActiveConversation = null;
    public static string HardLockTarget = null;

    // Sideload Conversation state
    public static bool ReplaceLinkOnResponseIfNeeded { get; set; } = false;
    public static bool SideLoadCaptureNextResponseIndex { get; set; } = false;
    public static Dictionary<string, SideloadConversationState> SideLoadCachedState = new Dictionary<string, SideloadConversationState>();
    public static Dictionary<string, string> SideloadConversationMap = new Dictionary<string, string>(); // <sideloadedConvoId, previousConvoId>

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
      bool forceNonFPConferenceRoom = (inputs.Length == 4) ? env.ToBool(inputs[3]) : false;
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

      ForceNextIsInFlashpointCheckFalse = forceNonFPConferenceRoom;
      ActiveConversation = conversation;

      return null;
    }

    static IEnumerator WaitThenQueueConversation(SimGameState simulation, Conversation conversation, string groupHeader, string groupSubHeader) {
      yield return new WaitForSeconds(1);
      SimGameInterruptManager interruptManager = simulation.interruptQueue;
      interruptManager.QueueConversation(conversation, groupHeader, groupSubHeader, null, true);
    }

    public static object SideloadConversation(TsEnvironment env, object[] inputs) {
      string conversationId = env.ToString(inputs[0]);
      string nodeEntryId = env.ToString(inputs[1]);
      bool resumeHostOnFinish = env.ToBool(inputs[2]);
      Main.Logger.Log($"[SideloadConversation] Sideload conversation id: " + conversationId + " with nodeEntryId: " + nodeEntryId + " with resumeHostOnFinish: " + resumeHostOnFinish);
      if (IsNodeAction) Main.Logger.Log($"[SideloadConversation] Sideload conversation is a node action");
      if (IsLinkAction) Main.Logger.Log($"[SideloadConversation] Sideload conversation is a link action");
      ReplaceLinkOnResponseIfNeeded = true;

      Conversation conversation = null;
      SimGameState simGame = UnityGameInstance.Instance.Game.Simulation;
      SimGameConversationManager conversationManager = simGame.ConversationManager;
      Conversation currentConversation = conversationManager.thisConvoDef;

      try {
        conversation = simGame.DataManager.SimGameConversations.Get(conversationId);
      } catch (KeyNotFoundException) {
        Main.Logger.Log($"[SideloadConversation] Conversation with id '{conversationId}' not found. Check the conversation id is correct or/and if the conversation has loaded correctly.");
      }

      if (conversation == null) {
        Main.Logger.Log($"[SideloadConversation] Conversation is null for id '{conversationId}'");
      } else {
        if (resumeHostOnFinish) {
          SideloadConversationState cachedState = new SideloadConversationState();
          cachedState.convoDef = currentConversation;
          cachedState.currentLink = conversationManager.currentLink;
          cachedState.currentNode = conversationManager.currentNode;
          cachedState.state = conversationManager.thisState;
          cachedState.linkToAutoFollow = conversationManager.linkToAutoFollow;
          cachedState.onlyOnceLinks = conversationManager.onlyOnceLinks;

          // Handle action being on a Response
          if (IsLinkAction) {
            Main.Logger.Log($"[SideloadConversation] Is link action - use hydrate node instead");
            cachedState.useNodeOnHydrate = true;

            bool isNodeInAutofollowMode = false;
            bool forceEnd = conversationManager.EvaluateNode(conversationManager.currentNode, out isNodeInAutofollowMode);
            Main.Logger.Log("[SideloadConversation] Current node is in autofollow response mode? " + isNodeInAutofollowMode);

            if (isNodeInAutofollowMode) {
              // Handle action being on an 'Autofollow Response'
              for (int i = 0; i < conversationManager.currentNode.branches.Count; i++) {
                ConversationLink conversationLink = conversationManager.currentNode.branches[i];
                if (conversationLink.responseText == "") {
                  cachedState.nextNodeIndex = conversationLink.nextNodeIndex;
                }
              }
            } else {
              // Handle action being on a 'Text Response'
              cachedState.nextNodeIndex = conversationManager.currentLink.nextNodeIndex;
            }
          }

          cachedState.previousNodes = new List<ConversationNode>();
          foreach (ConversationNode prevNode in conversationManager.previousNodes) {
            cachedState.previousNodes.Add(prevNode);
          }

          Actions.SideLoadCachedState.Add(currentConversation.idRef.id, cachedState);
          Actions.SideloadConversationMap.Add(conversation.idRef.id, currentConversation.idRef.id);

          SideLoadCaptureNextResponseIndex = true;
        }

        conversationManager.thisConvoDef = conversation;
        conversationManager.currentNode = null;
        conversationManager.currentLink = null;
        conversationManager.previousNodes.Clear();

        ConversationNode conversationNode = new ConversationNode();
        conversationNode.index = -1;
        for (int i = 0; i < conversation.roots.Count; i++) {
          conversationNode.branches.Add(conversation.roots[i]);
        }
        conversationManager.currentNode = conversationNode;

        bool autoFollow = false;
        bool passedConditions = conversationManager.EvaluateNode(conversationNode, out autoFollow);
        if (autoFollow && passedConditions) {
          conversationManager.EndConversation();
        }

        // Find the entry node if provided
        if (nodeEntryId == "") {
          conversationManager.thisState = BattleTech.SimGameConversationManager.ConversationState.NODE;
          conversationManager.currentNode = conversationNode;
        } else {
          int entryNodeIndex = conversation.nodes.FindIndex((node => node.idRef.id == nodeEntryId));

          if (entryNodeIndex != -1) {
            conversationManager.thisState = BattleTech.SimGameConversationManager.ConversationState.RESPONSE;

            ConversationLink conversationLink = new ConversationLink();
            conversationLink.onlyOnce = false;
            conversationLink.idRef = new IDRef();
            conversationLink.idRef.id = Guid.NewGuid().ToString();
            conversationLink.nextNodeIndex = entryNodeIndex;

            conversationManager.currentLink = conversationLink;
          } else {
            conversationManager.currentNode = conversationNode;
            conversationManager.currentLink = conversation.roots[0];
            conversationManager.linkToAutoFollow = 0;
          }
        }
      }

      IsNodeAction = false;
      IsLinkAction = false;

      return null;
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

    /*
      Both flashpointId and systemId can be null/empty.
      Vanilla code for flashpoints handles by:
        - flashpointId = null/empty will pick a random non-completed flashpoint
        - systemId = null/empty will pick a random system
    */
    public static object AddFlashpoint(TsEnvironment env, object[] inputs) {
      string flashpointId = env.ToString(inputs[0]);
      string systemId = env.ToString(inputs[1]);

      Main.Logger.Log($"[AddFlashpoint] Received flashpointId '{flashpointId}' and systemId '{systemId}'");

      if (flashpointId == "0") flashpointId = null;
      if (systemId == "0") systemId = null;

      if (!string.IsNullOrEmpty(systemId) && !systemId.StartsWith("starsystemdef_") && !systemId.StartsWith("local")) {
        systemId = $"starsystemdef_{systemId}";
      }

      Main.Logger.Log($"[AddFlashpoint] Using flashpointId '{flashpointId}' and systemId '{systemId}'");

      SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
      simulation.GenerateFlashpointCommand(flashpointId, systemId);

      return null;
    }

    public static object SetCameraHardLock(TsEnvironment env, object[] inputs) {
      string key = env.ToString(inputs[0]);

      Main.Logger.Log($"[SetCameraHardLock] Received key '{key}'");
      HardLockTarget = key;

      UnityGameInstance.BattleTechGame.Simulation.ConversationManager.SetCameraLockTarget(key);

      return null;
    }
  }
}