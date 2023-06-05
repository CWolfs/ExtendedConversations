using System.Collections.Generic;

using isogame;

using TScript;

namespace ExtendedConversations.Utils {
  public static class ConversationHelper {
    public static bool DoesLinkContainAction(string actionName, ConversationLink link) {
      List<TsNode> list = new List<TsNode>();
      if (link.actions != null) {
        for (int i = 0; i < link.actions.ops.Count; i++) {
          TsCall call = link.actions.ops[i];
          if (call.functionName == actionName) return true;
        }
      }

      return false;
    }
  }
}
