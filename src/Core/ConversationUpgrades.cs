using UnityEngine;
using System;
using Harmony;

using BattleTech;
using TScript;
using TScript.Ops;
using HBS.Logging;
using HBS.Collections;

using ExtendedConversations;

namespace ExtendedConversations.Core {
  public class ConversationUpgrades {
    public static void Declare(TsEnvironment env) {
      Main.Logger.Log("Declaring conversation updates");

      TsType boolType = env.GetType("bool");
      TsType HasOrHasNotType = env.GetType("HasOrHasNot");
      TsType SenseTagListType = env.GetType("SenseTagList");

      Main.Logger.Log("Declaring 'Evaluate Tag for Current System' condition operation");
      TsOp tsOp = env.DeclareOp("ConditionFunction", "Evaluate Tag for Current System", boolType, new TsOp.EvalDelegate(ConversationUpgrades.EvaluateTagForCurrentSystem));
      tsOp.DeclareInput("comp", HasOrHasNotType);
      tsOp.DeclareInput("tagName", SenseTagListType);

      Main.Logger.Log("Finished declaring conversation upgrades");
    }

    /* OPERATIONS */
    public static object EvaluateTagForCurrentSystem(TsEnvironment env, object[] inputs) {
      Main.Logger.Log("EvaluateTagForCurrentSystem triggered");
      bool flag = env.ToBool(inputs[0]);
      string value = env.ToString(inputs[1]);
      TagSet currentSystemTags = UnityGameInstance.BattleTechGame.Simulation.CurSystem.Tags;
      bool flag2 = currentSystemTags.Contains(value) == flag;
      Main.Logger.Log("EvaluateTagForCurrentSystem finished with result of " + flag2);
      return flag2;
    }
  }
}