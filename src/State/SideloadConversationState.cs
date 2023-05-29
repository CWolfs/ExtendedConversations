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

namespace ExtendedConversations.State {
  public class SideloadConversationState {
    public Conversation convoDef { get; set; }
    public ConversationNode currentNode { get; set; }
    public ConversationLink currentLink { get; set; }
    public SimGameConversationManager.ConversationState state { get; set; }
    public int linkToAutoFollow { get; set; }
    public List<ConversationNode> previousNodes { get; set; }
  }
}