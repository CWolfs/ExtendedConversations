using UnityEngine;
using System;
using Harmony;

using BattleTech;
using TScript;
using TScript.Ops;
using HBS.Logging;
using HBS.Collections;

using ExtendedConversations;
using ExtendedConversations.Utils;

namespace ExtendedConversations.Core {
  public class Conditions {
    public static object EvaluateTagForCurrentSystem(TsEnvironment env, object[] inputs) {
      bool flag = env.ToBool(inputs[0]);
      string value = env.ToString(inputs[1]);
      Main.Logger.Log($"[EvaluateTagForCurrentSystem] Triggered with flag {flag} and value {value}.");

      TagSet currentSystemTags = UnityGameInstance.BattleTechGame.Simulation.CurSystem.Tags;
      bool flag2 = currentSystemTags.Contains(value) == flag;
      Main.Logger.Log("[EvaluateTagForCurrentSystem] Finished with result of " + flag2);
      return flag2;
    }

    public static object EvaluateBattleTechString(TsEnvironment env, object[] inputs) {
      int statScope = env.ToInt(inputs[0]);
      string statName = env.ToString(inputs[1]);
      int operation = env.ToInt(inputs[2]);
      string compareValue = env.ToString(inputs[3]);
      Main.Logger.Log($"[EvaluateBattleTechString] Triggered with scope '{statScope}', statName '{statName}', operation '{operation}', compareValue '{compareValue}");

      StatCollection statCollection = SimHelper.GetStatCollection(statScope);

      if (statCollection.ContainsStatistic(statName)) {
        string stat = statCollection.GetValue<string>(statName);

        switch (operation) {
          case 1: // equal to
            return (stat == compareValue);
          case 2: // not equal to
            return (stat != compareValue);
          default:
            return false;
        }
      }

      Main.Logger.Log($"[EvaluateBattleTechString] Stat {statName} does not exist for conversation operation.");
      return false;
    }

    public static object EvaluateBattleTechInt(TsEnvironment env, object[] inputs) {
      int statScope = env.ToInt(inputs[0]);
      string statName = env.ToString(inputs[1]);
      int operation = env.ToInt(inputs[2]);
      int compareValue = env.ToInt(inputs[3]);
      Main.Logger.Log($"[EvaluateBattleTechInt] Triggered with scope '{statScope}', statName '{statName}', operation '{operation}', compareValue '{compareValue}");

      StatCollection statCollection = SimHelper.GetStatCollection(statScope);

      if (statCollection.ContainsStatistic(statName)) {
        int stat = statCollection.GetValue<int>(statName);

        switch (operation) {
          case 1: // less than
            return (stat < compareValue);
          case 2: // equal to
            return (stat == compareValue);
          case 3: // greater than
            return (stat > compareValue);
          case 4: // less than or equal to
            return (stat <= compareValue);
          case 5: // greater than or equal to
            return (stat >= compareValue);
          default:
            return false;
        }
      }

      Main.Logger.Log($"[EvaluateBattleTechInt] Stat {statName} does not exist for conversation operation.");
      return false;
    }

    public static object EvaluateBattleTechFloat(TsEnvironment env, object[] inputs) {
      int statScope = env.ToInt(inputs[0]);
      string statName = env.ToString(inputs[1]);
      int operation = env.ToInt(inputs[2]);
      float compareValue = env.ToFloat(inputs[3]);
      Main.Logger.Log($"[EvaluateBattleTechFloat] Triggered with scope '{statScope}', statName '{statName}', operation '{operation}', compareValue '{compareValue}");

      StatCollection statCollection = SimHelper.GetStatCollection(statScope);

      if (statCollection.ContainsStatistic(statName)) {
        float stat = statCollection.GetValue<float>(statName);

        switch (operation) {
          case 1: // less than
            return (stat < compareValue);
          case 2: // equal to
            return (stat == compareValue);
          case 3: // greater than
            return (stat > compareValue);
          case 4: // less than or equal to
            return (stat <= compareValue);
          case 5: // greater than or equal to
            return (stat >= compareValue);
          default:
            return false;
        }
      }

      Main.Logger.Log($"[EvaluateBattleTechFloat] Stat {statName} does not exist for conversation operation.");
      return false;
    }

    public static object EvaluateFunds(TsEnvironment env, object[] inputs) {
      int operation = env.ToInt(inputs[0]);
      int moneyCheckValue = env.ToInt(inputs[1]);
      int funds = UnityGameInstance.BattleTechGame.Simulation.Funds;

      switch (operation) {
        case 1: // less than
          return (funds < moneyCheckValue);
        case 2: // equal to
          return (funds == moneyCheckValue);
        case 3: // greater than
          return (funds > moneyCheckValue);
        case 4: // less than or equal to
          return (funds <= moneyCheckValue);
        case 5: // greater than or equal to
          return (funds >= moneyCheckValue);
        default:
          return false;
      }
    }

    public static object EvaluateTimeline(TsEnvironment env, object[] inputs) {
      int operation = env.ToInt(inputs[0]);
      string date = env.ToString(inputs[1]); // format is like either 3050 (year only), 3050-02 (year and month) or 30-50-02-01 (year, month and day)
      DateTime currentDate = UnityGameInstance.BattleTechGame.Simulation.CurrentDate;
      DateTime parsedDate;
      DateTime endDate;

      // Analyse 'date' variable
      switch (date.Length) {
        case 4: // year only
          if (!int.TryParse(date, out int yearOnly)) {
            Main.Logger.LogError($"[EvaluateTimeline] Year is not a valid integer. Provided date: '{date}'");
            return false;
          }

          parsedDate = new DateTime(yearOnly, 1, 1);
          endDate = new DateTime(yearOnly, 12, 31);
          break;
        case 7: // year and month
          if (!int.TryParse(date.Substring(0, 4), out int year) || !int.TryParse(date.Substring(5, 2), out int month)) {
            Main.Logger.LogError($"[EvaluateTimeline] Year or month is not a valid integer. Provided date: '{date}'"); ;
            return false;
          }

          parsedDate = new DateTime(year, month, 1);
          endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
          break;
        case 10: // year, month and day
          if (!DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            Main.Logger.LogError($"[EvaluateTimeline] Date is not in a valid format (yyyy-MM-dd). Provided date: '{date}'");
            return false;
          }

          endDate = parsedDate;
          break;
        default: {
          Main.Logger.LogError($"Invalid date format: {date}");
          return false;
        }
      }

      // Check the date vs. the current date and return true/false based on the operation check
      switch (operation) {
        case 1: // On or Before
          return currentDate.Date <= endDate;
        case 2: // On or After
          return currentDate.Date >= parsedDate;
        case 3: // Before
          return currentDate.Date < parsedDate;
        case 4: // After
          return currentDate.Date > endDate;
        case 5: // On
          return currentDate.Date >= parsedDate && currentDate.Date <= endDate;
        case 6: // Not On
          return currentDate.Date < parsedDate || currentDate.Date > endDate;
        default: {
          Main.Logger.LogError($"[EvaluateTimeline] Invalid operation: {operation}");
          return false;
        }
      }
    }
  }
}