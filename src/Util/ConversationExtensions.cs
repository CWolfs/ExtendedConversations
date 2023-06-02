using BattleTech;

using isogame;

public static class ConversationExtensions {
  public static ConversationLink GetRootToFollow(this Conversation conversation) {
    SimGameConversationManager conversationManager = UnityGameInstance.Instance.Game.Simulation.ConversationManager;

    for (int i = 0; i < conversation.roots.Count; i++) {
      ConversationLink conversationLink = conversation.roots[i];
      if (conversationManager.EvaluateLink(conversationLink)) {
        if (conversationLink.responseText == "") {
          return conversationLink;
        }
      }
    }

    return null;
  }
}