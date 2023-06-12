using System;

using Harmony;

using BattleTech;
using isogame;

using ExtendedConversations.Core;
using ExtendedConversations.State;

using System.Threading.Tasks;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "EndConversation")]
  public class SimGameConversationManagerEndConversationPatch {
    static bool Prefix(SimGameConversationManager __instance) {
      try {
        if (ProcessSideloadConversationPatch(__instance)) {
          // Override vanilla method if processing a sideload to allow for the sideload mechanic to work
          return false;
        }

        ProcessForceNonFPConferenceRoomPatch(__instance);

        return true;
      } catch (Exception e) {
        Main.Logger.LogError("[Extended Conversations] An error occured in SimGameConversationManagerEndConversationPatch. Caught gracefully." + e.StackTrace.ToString());
        return true;
      }
    }

    static async void Postfix(SimGameConversationManager __instance) {
      await CheckAndRunDelayedMessages();
    }

    private static async System.Threading.Tasks.Task CheckAndRunDelayedMessages() {
      try {
        await System.Threading.Tasks.Task.Delay(1500);

        SimGameState simGame = UnityGameInstance.Instance.Game.Simulation;
        if (simGame.interruptQueue.HasQueue) {
          if (!simGame.interruptQueue.IsOpen) {
            simGame.interruptQueue.DisplayIfAvailable();
          }
        }
      } catch (Exception e) {
        Main.Logger.LogError("[Extended Conversations] An error occured in CheckAndRunDelayedMessages. Caught gracefully." + e.StackTrace.ToString());
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