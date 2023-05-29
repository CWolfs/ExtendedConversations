using System.Collections.Generic;

using BattleTech;

using isogame;


namespace ExtendedConversations.State {
  public class SideloadConversationState {
    public Conversation convoDef { get; set; }
    public ConversationNode currentNode { get; set; }
    public ConversationLink currentLink { get; set; }
    public SimGameConversationManager.ConversationState state { get; set; }
    public int linkToAutoFollow { get; set; }
    public List<ConversationNode> previousNodes { get; set; }
    public int ResponseIndexClicked { get; set; }
  }
}