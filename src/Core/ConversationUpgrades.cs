using UnityEngine;
using System;
using Harmony;
using TScript;
using TScript.Ops;
using HBS.Logging;

namespace TestBattletechMod.Core {
  public class ConversationUpgrades {
    private static ILog Log = HBS.Logging.Logger.GetLogger(typeof(ConversationUpgrades).Name, LogLevel.Log);

    public static void Declare(TsEnvironment env) {
      Log.Log("Testing ConversationUpgrades Declare");
    }
  }
}