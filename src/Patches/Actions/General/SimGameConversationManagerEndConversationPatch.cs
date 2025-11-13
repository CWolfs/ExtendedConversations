using System;

using Harmony;

using BattleTech;
using isogame;

using ExtendedConversations.Core;
using ExtendedConversations.State;

using BattleTech.UI;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "EndConversation")]
  public class SimGameConversationManagerEndConversationPatch {
    private static string cachedConversationName = "unknown";

    static bool Prefix(SimGameConversationManager __instance) {
      try {
        // Capture conversation name before it gets cleared
        if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogConditions) {
          cachedConversationName = __instance?.thisConvoDef?.ui_name ?? "unknown";
        }

        if (ProcessSideloadConversationPatch(__instance)) {
          // Override vanilla method if processing a sideload to allow for the sideload mechanic to work
          return false;
        }

        ProcessForceNonFPConferenceRoomPatch(__instance);

        return true;
      } catch (Exception e) {
        Main.Logger.LogError("[SimGameConversationManagerEndConversationPatch] An error occured in SimGameConversationManagerEndConversationPatch. Caught gracefully." + e.StackTrace.ToString());
        return true;
      }
    }

    static async void Postfix(SimGameConversationManager __instance) {
      // Debug logging for conversation end (using cached name from Prefix)
      if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogConditions) {
        Main.Logger.Log($"\n========================================");
        Main.Logger.Log($"[EndConversation] Ended: {cachedConversationName}");
        Main.Logger.Log($"========================================\n");
      }

      if (SimGameConversationManagerEndConversationPatch.IsCustomRoomActive) {
        Main.Logger.Log("[SimGameConversationManagerEndConversationPatch.Postfix] About to check and run delayed messages");
        await CheckAndRunDelayedMessages();
      } else {
        Main.Logger.Log("[SimGameConversationManagerEndConversationPatch.Postfix] Skipping check and run of delayed messages as NOT in a custom room.");
      }
    }

    public static bool IsCustomRoomActive {
      get {
        SGRoomManager roomManager = UnityGameInstance.Instance.Game.Simulation.RoomManager;
        DropshipLocation dropshipLocation = roomManager.currRoomDropshipLocation;

        return !EnumUtils.IsDefined<DropshipLocation>((int)dropshipLocation);
      }
    }

    private static async System.Threading.Tasks.Task CheckAndRunDelayedMessages() {
      try {
        await System.Threading.Tasks.Task.Delay(1500);

        SimGameState simGame = UnityGameInstance.Instance.Game.Simulation;
        if (simGame.interruptQueue.HasQueue) {
          if (!simGame.interruptQueue.IsOpen && !simGame.ConversationManager.IsOn) {
            simGame.interruptQueue.DisplayIfAvailable();
          } else {
            if (simGame.interruptQueue.IsOpen) Main.Logger.Log("[SimGameConversationManagerEndConversationPatch.CheckAndRunDelayedMessages] Waiting for interrupt popup to close.");
            if (simGame.ConversationManager.IsOn) Main.Logger.Log("[SimGameConversationManagerEndConversationPatch.CheckAndRunDelayedMessages] Waiting for conversation manager to finish.");
            await CheckAndRunDelayedMessages();
          }
        }
      } catch (Exception e) {
        Main.Logger.LogError("[SimGameConversationManagerEndConversationPatch.CheckAndRunDelayedMessages] An error occured in CheckAndRunDelayedMessages. Caught gracefully." + e.StackTrace.ToString());
      }
    }

    private static void ProcessForceNonFPConferenceRoomPatch(SimGameConversationManager simGameConversationManager) {
      Conversation conversation = simGameConversationManager.thisConvoDef;

      if (Actions.ActiveConversation == conversation) {
        Actions.ForceNextIsInFlashpointCheckFalse = false;
        Actions.ActiveConversation = null;
      }

      Actions.HardLockTarget = null;
    }

    private static bool ProcessSideloadConversationPatch(SimGameConversationManager simGameConversationManager) {

      Conversation conversation = simGameConversationManager.thisConvoDef;
      if (conversation == null) return false;

      string conversationId = conversation.idRef.id;

      if (Actions.SideloadConversationMap.ContainsKey(conversationId)) {
        string previousConversationId = Actions.SideloadConversationMap[conversationId];
        SideloadConversationState cachedState = Actions.SideLoadCachedState[previousConversationId];

        simGameConversationManager.thisConvoDef = cachedState.convoDef;
        simGameConversationManager.currentLink = cachedState.currentLink;
        simGameConversationManager.currentNode = cachedState.currentNode;
        simGameConversationManager.thisState = cachedState.state;
        simGameConversationManager.linkToAutoFollow = cachedState.linkToAutoFollow;
        simGameConversationManager.onlyOnceLinks = cachedState.onlyOnceLinks;
        simGameConversationManager.previousNodes.Clear();

        foreach (ConversationNode prevNode in cachedState.previousNodes) {
          simGameConversationManager.previousNodes.Add(prevNode);
        }

        Actions.SideLoadCachedState.Remove(previousConversationId);
        Actions.SideloadConversationMap.Remove(conversationId);

        if (cachedState.useNodeOnHydrate) {
          Main.Logger.Log("[ProcessSideloadConversationPatch] useNodeOnHydrate so attempting to use node instead");
          Main.Logger.Log("[SideloadConversation] cachedState.convoDef.nodes count" + cachedState.convoDef.nodes.Count);
          Main.Logger.Log("[SideloadConversation] Using nextNodeIndex " + cachedState.nextNodeIndex);
          if (cachedState.nextNodeIndex > -1) {
            simGameConversationManager.currentNode = cachedState.convoDef.nodes[cachedState.nextNodeIndex];
            simGameConversationManager.ShowNodeText(simGameConversationManager.currentNode);
          } else {
            return false;
          }
        } else {
          if (simGameConversationManager.currentLink.responseText == "") {
            simGameConversationManager.Continue(true);
          } else {
            simGameConversationManager.SelectResponse(cachedState.ResponseIndexClicked);
          }
        }

        return true;
      }

      return false;
    }
  }
}