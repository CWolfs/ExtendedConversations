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
      TsType floatType = env.GetType("float");
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

      // `Evaluate BattleTech String` condition
      Main.Logger.Log("Declaring 'Evaluate BattleTech String' condition");
      tsOp = env.DeclareOp("ConditionFunction", "Evaluate BattleTech String", boolType, new TsOp.EvalDelegate(Conditions.EvaluateBattleTechString));
			tsOp.DeclareInput("scope", SimGameScopeType);
			tsOp.DeclareInput("param", stringType);
      tsOp.DeclareInput("operation", intType);
			tsOp.DeclareInput("value", stringType);

      // `Evaluate BattleTech Int` condition
      Main.Logger.Log("Declaring 'Evaluate BattleTech Int' condition");
      tsOp = env.DeclareOp("ConditionFunction", "Evaluate BattleTech Int", boolType, new TsOp.EvalDelegate(Conditions.EvaluateBattleTechInt));
			tsOp.DeclareInput("scope", SimGameScopeType);
			tsOp.DeclareInput("param", stringType);
      tsOp.DeclareInput("operation", intType);
			tsOp.DeclareInput("value", intType);

      // `Evaluate BattleTech Float` condition
      Main.Logger.Log("Declaring 'Evaluate BattleTech Float' condition");
      tsOp = env.DeclareOp("ConditionFunction", "Evaluate BattleTech Float", boolType, new TsOp.EvalDelegate(Conditions.EvaluateBattleTechFloat));
			tsOp.DeclareInput("scope", SimGameScopeType);
			tsOp.DeclareInput("param", stringType);
      tsOp.DeclareInput("operation", intType);
			tsOp.DeclareInput("value", floatType);

      // Evaluate Funds
      Main.Logger.Log("Declaring 'Evaluate Funds' condition");
      tsOp = env.DeclareOp("ConditionFunction", "Evaluate Funds", boolType, new TsOp.EvalDelegate(Conditions.EvaluateFunds));
      tsOp.DeclareInput("operation", intType);
			tsOp.DeclareInput("value", intType);

      /*
      * ACTIONS
      */
      
      // `TimeSkip` action
      Main.Logger.Log("Declaring 'Time Skip' action");
      tsOp = env.DeclareOp("EffectFunctions", "Time Skip", voidType, new TsOp.EvalDelegate(Actions.TimeSkip));
      tsOp.DeclareInput("days", intType);

      // `Set Current System` action
      Main.Logger.Log("Declaring 'Set Current System' action");
      tsOp = env.DeclareOp("EffectFunctions", "Set Current System", voidType, new TsOp.EvalDelegate(Actions.SetCurrentSystem));
      tsOp.DeclareInput("systemName", stringType);
      tsOp.DeclareInput("includeTravelTime", intType);

      // 'Modify Funds' action
      Main.Logger.Log("Declaring 'Modify Funds' action");
      tsOp = env.DeclareOp("EffectFunctions", "Modify Funds", voidType, new TsOp.EvalDelegate(Actions.ModifyFunds));
      tsOp.DeclareInput("operation", intType);
      tsOp.DeclareInput("amount", intType);

      // 'Set Character Visible' action
      Main.Logger.Log("Declaring 'Set Characters Visible' action");
      tsOp = env.DeclareOp("EffectFunctions", "Set Characters Visible", voidType, new TsOp.EvalDelegate(Actions.SetCharactersVisible));
      tsOp.DeclareInput("isVisible", intType);
      tsOp.DeclareInput("crewNames", stringType);
      
      // 'Start Conversation' action
      Main.Logger.Log("Declaring 'Start Conversation' action");
      tsOp = env.DeclareOp("EffectFunctions", "Start Conversation Custom", voidType, new TsOp.EvalDelegate(Actions.StartConversation));
      tsOp.DeclareInput("conversationId", stringType);
      tsOp.DeclareInput("groupHeader", stringType);
      tsOp.DeclareInput("groupSubHeader", stringType);

      // 'Add Contract' action
      Main.Logger.Log("Declaring 'Add Contract' action");
      tsOp = env.DeclareOp("EffectFunctions", "Add Contract", voidType, new TsOp.EvalDelegate(Actions.AddContract));
      tsOp.DeclareInput("contractId", stringType);
      tsOp.DeclareInput("target", stringType);
      tsOp.DeclareInput("employer", stringType);
      tsOp.DeclareInput("possibleLocation", stringType);

      /*
      * VALUE GETTERS
      */
      
      // `Get BattleTech String` value getter
      Main.Logger.Log("Declaring 'Get BattleTech String' value getter");
      tsOp = env.DeclareOp("ValueGetterFunctions", "Get BattleTech String", stringType, new TsOp.EvalDelegate(ValueGetters.GetBattleTechString));
      tsOp.DeclareInput("scope", intType);
      tsOp.DeclareInput("statName", stringType);

      // `Get BattleTech Int` value getter
      Main.Logger.Log("Declaring 'Get BattleTech Int' value getter");
      tsOp = env.DeclareOp("ValueGetterFunctions", "Get BattleTech Int", intType, new TsOp.EvalDelegate(ValueGetters.GetBattleTechInt));
      tsOp.DeclareInput("scope", intType);
      tsOp.DeclareInput("statName", stringType);

      // `Get BattleTech Float` value getter
      Main.Logger.Log("Declaring 'Get BattleTech Float' value getter");
      tsOp = env.DeclareOp("ValueGetterFunctions", "Get BattleTech Float", floatType, new TsOp.EvalDelegate(ValueGetters.GetBattleTechFloat));
      tsOp.DeclareInput("scope", intType);
      tsOp.DeclareInput("statName", stringType);

      Main.Logger.Log("Finished declaring conversation upgrades");
    }
  }
}