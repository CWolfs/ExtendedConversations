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

      TsType voidType = env.GetType("void");
      TsType intType = env.GetType("int");
      TsType boolType = env.GetType("bool");
      TsType stringType = env.GetType("string");

      TsType HasOrHasNotType = env.GetType("HasOrHasNot");
      TsType SenseTagListType = env.GetType("SenseTagList");
      TsType SimGameScopeType = env.GetType("SimGameScope");

      /*
      * CONDITIONS
      */

      // Evaluate Tag for Current System
      Main.Logger.Log("Declaring 'Evaluate Tag for Current System' condition");
      TsOp tsOp = env.DeclareOp("ConditionFunction", "Evaluate Tag for Current System", boolType, new TsOp.EvalDelegate(Conditions.EvaluateTagForCurrentSystem));
      tsOp.DeclareInput("comp", HasOrHasNotType);
      tsOp.DeclareInput("tagName", SenseTagListType);

      // `Evaluate BattleTech Int` condition
      Main.Logger.Log("Declaring 'Evaluate BattleTech Int' condition");
      tsOp = env.DeclareOp("ConditionFunction", "Evaluate BattleTech Int", boolType, new TsOp.EvalDelegate(Conditions.EvaluateBattleTechInt));
			tsOp.DeclareInput("scope", SimGameScopeType);
			tsOp.DeclareInput("param", stringType);
      tsOp.DeclareInput("operation", intType);
			tsOp.DeclareInput("value", intType);

      // TODO: Add `Evaluate BattleTech Float` condition

      // TODO: Add `Evaluate BattleTech String` condition

      // Evaluate Funds
      Main.Logger.Log("Declaring 'Evaluate Funds' condition");
      tsOp = env.DeclareOp("ConditionFunction", "Evaluate Funds", boolType, new TsOp.EvalDelegate(Conditions.EvaluateFunds));
      tsOp.DeclareInput("operation", intType);
			tsOp.DeclareInput("value", intType);

      /*
      * ACTIONS
      */

      // TODO: Add `TimeSkip` action
      Main.Logger.Log("Declaring 'Time Skip' action");
      tsOp = env.DeclareOp("EffectFunctions", "Time Skip", voidType, new TsOp.EvalDelegate(Actions.TimeSkip));
      tsOp.DeclareInput("days", intType);

      // TODO: Add 'Dropship Location' action

      // TODO: Add 'Modify Funds' action

      Main.Logger.Log("Finished declaring conversation upgrades");
    }
  }
}