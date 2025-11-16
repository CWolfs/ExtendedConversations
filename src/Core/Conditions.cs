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
    // Helper method to parse flexible date formats (yyyy, yyyy-MM, yyyy-M, yyyy-MM-dd, etc.)
    private static bool TryParseFlexibleDate(string dateString, out DateTime parsedDate) {
      parsedDate = DateTime.MinValue;

      if (string.IsNullOrEmpty(dateString)) {
        return false;
      }

      switch (dateString.Length) {
        case 4: // year only
          if (int.TryParse(dateString, out int yearOnly)) {
            parsedDate = new DateTime(yearOnly, 1, 1).Date;
            return true;
          }
          return false;

        case 6: // year and month (yyyy-M format like "3050-6")
        case 7: // year and month (yyyy-MM format like "3050-06")
          string[] yearMonthFormats = { "yyyy-MM", "yyyy-M" };
          if (DateTime.TryParseExact(dateString, yearMonthFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            parsedDate = parsedDate.Date; // Strip time component
            return true;
          }
          return false;

        case 8: // year, month and day (yyyy-M-d format like "3050-6-5")
        case 9: // year, month and day (yyyy-MM-d or yyyy-M-dd format)
        case 10: // year, month and day (yyyy-MM-dd format like "3050-06-05")
          string[] fullDateFormats = { "yyyy-MM-dd", "yyyy-M-d", "yyyy-MM-d", "yyyy-M-dd" };
          if (DateTime.TryParseExact(dateString, fullDateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            parsedDate = parsedDate.Date; // Strip time component
            return true;
          }
          return false;

        default:
          return false;
      }
    }

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
      Main.Logger.Log($"[EvaluateBattleTechString] Triggered with scope '{statScope}', statName '{statName}', operation '{operation}', compareValue '{compareValue}'");

      StatCollection statCollection = SimHelper.GetStatCollection(statScope);

      if (statCollection.ContainsStatistic(statName)) {
        string stat = statCollection.GetValue<string>(statName);

        switch (operation) {
          case 1: // equal to (string comparison)
            Main.Logger.Log($"[EvaluateBattleTechString] String comparison: '{stat}' == '{compareValue}' = {stat == compareValue}");
            return stat == compareValue;
          case 2: // not equal to (string comparison)
            Main.Logger.Log($"[EvaluateBattleTechString] String comparison: '{stat}' != '{compareValue}' = {stat != compareValue}");
            return stat != compareValue;
          case 3: // less than (date comparison)
          case 4: // less than or equal to (date comparison)
          case 5: // greater than (date comparison)
          case 6: // greater than or equal to (date comparison)
            // Attempt to parse both values as dates
            if (!TryParseFlexibleDate(stat, out DateTime statDate)) {
              Main.Logger.LogError($"[EvaluateBattleTechString] Failed to parse stat value '{stat}' as a date for operation {operation}. Expected format: yyyy, yyyy-MM, or yyyy-MM-dd");
              return false;
            }
            if (!TryParseFlexibleDate(compareValue, out DateTime compareDate)) {
              Main.Logger.LogError($"[EvaluateBattleTechString] Failed to parse compare value '{compareValue}' as a date for operation {operation}. Expected format: yyyy, yyyy-MM, or yyyy-MM-dd");
              return false;
            }

            // Perform date comparison
            bool result;
            string operationName;
            switch (operation) {
              case 3: // less than
                operationName = "Less Than";
                result = statDate < compareDate;
                break;
              case 4: // less than or equal to
                operationName = "Less Than Or Equal To";
                result = statDate <= compareDate;
                break;
              case 5: // greater than
                operationName = "Greater Than";
                result = statDate > compareDate;
                break;
              case 6: // greater than or equal to
                operationName = "Greater Than Or Equal To";
                result = statDate >= compareDate;
                break;
              default:
                Main.Logger.LogError($"[EvaluateBattleTechString] Invalid operation: {operation}");
                return false;
            }

            Main.Logger.Log($"[EvaluateBattleTechString] Date comparison: '{stat}' ({statDate:yyyy-MM-dd}) {operationName} '{compareValue}' ({compareDate:yyyy-MM-dd}) = {result}");
            return result;
          default:
            Main.Logger.LogError($"[EvaluateBattleTechString] Invalid operation: {operation}");
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
            return stat < compareValue;
          case 2: // equal to
            return stat == compareValue;
          case 3: // greater than
            return stat > compareValue;
          case 4: // less than or equal to
            return stat <= compareValue;
          case 5: // greater than or equal to
            return stat >= compareValue;
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
            return stat < compareValue;
          case 2: // equal to
            return stat == compareValue;
          case 3: // greater than
            return stat > compareValue;
          case 4: // less than or equal to
            return stat <= compareValue;
          case 5: // greater than or equal to
            return stat >= compareValue;
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
          return funds < moneyCheckValue;
        case 2: // equal to
          return funds == moneyCheckValue;
        case 3: // greater than
          return funds > moneyCheckValue;
        case 4: // less than or equal to
          return funds <= moneyCheckValue;
        case 5: // greater than or equal to
          return funds >= moneyCheckValue;
        default:
          return false;
      }
    }

    public static object EvaluateTimeline(TsEnvironment env, object[] inputs) {
      Main.Logger.Log($"[EvaluateTimeline] Entered EvaluateTimeline condition.");

      int operation = env.ToInt(inputs[0]);
      string date = env.ToString(inputs[1]); // format is like either 3050 (year only), 3050-02 (year and month) or 3050-02-01 (year, month and day)
      DateTime currentDate = UnityGameInstance.BattleTechGame.Simulation.CurrentDate;
      DateTime parsedDate;
      DateTime endDate;

      // Debug logging - Log all inputs
      Main.Logger.Log($"[EvaluateTimeline] TRIGGERED: operation={operation}, dateString='{date}', currentDate={currentDate.Date.ToString("yyyy-MM-dd")}");

      // Analyse 'date' variable
      switch (date.Length) {
        case 4: // year only
          if (!int.TryParse(date, out int yearOnly)) {
            Main.Logger.LogError($"[EvaluateTimeline] Year is not a valid integer. Provided date: '{date}'");
            return false;
          }

          parsedDate = new DateTime(yearOnly, 1, 1).Date;
          endDate = new DateTime(yearOnly, 12, 31).Date;
          Main.Logger.Log($"[EvaluateTimeline] Parsed year-only: parsedDate={parsedDate:yyyy-MM-dd}, endDate={endDate:yyyy-MM-dd}");
          break;
        case 6: // year and month (yyyy-M format like "3050-6")
        case 7: // year and month (yyyy-MM format like "3050-06")
          // Support both zero-padded (yyyy-MM) and non-zero-padded (yyyy-M) month formats
          string[] yearMonthFormats = { "yyyy-MM", "yyyy-M" };
          if (!DateTime.TryParseExact(date, yearMonthFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            Main.Logger.LogError($"[EvaluateTimeline] Year-month date is not in a valid format (yyyy-MM or yyyy-M). Provided date: '{date}'");
            return false;
          }

          parsedDate = parsedDate.Date; // Strip time component
          endDate = new DateTime(parsedDate.Year, parsedDate.Month, DateTime.DaysInMonth(parsedDate.Year, parsedDate.Month)).Date;
          Main.Logger.Log($"[EvaluateTimeline] Parsed year-month: parsedDate={parsedDate:yyyy-MM-dd}, endDate={endDate:yyyy-MM-dd}");
          break;
        case 8: // year, month and day (yyyy-M-d format like "3050-6-5")
        case 9: // year, month and day (yyyy-MM-d or yyyy-M-dd format like "3050-06-5" or "3050-6-05")
        case 10: // year, month and day (yyyy-MM-dd format like "3050-06-05")
          // Support multiple full date formats (zero-padded and non-zero-padded)
          string[] fullDateFormats = { "yyyy-MM-dd", "yyyy-M-d", "yyyy-MM-d", "yyyy-M-dd" };
          if (!DateTime.TryParseExact(date, fullDateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            Main.Logger.LogError($"[EvaluateTimeline] Date is not in a valid format (yyyy-MM-dd, yyyy-M-d, etc.). Provided date: '{date}'");
            return false;
          }

          parsedDate = parsedDate.Date; // Strip time component
          endDate = parsedDate;
          Main.Logger.Log($"[EvaluateTimeline] Parsed full date: parsedDate={parsedDate:yyyy-MM-dd}, endDate={endDate:yyyy-MM-dd}");
          break;
        default: {
          Main.Logger.LogError($"Invalid date format: {date}");
          return false;
        }
      }

      // Check the date vs. the current date and return true/false based on the operation check
      bool result;
      string operationName;

      switch (operation) {
        case 1: // On or Before
          operationName = "On or Before";
          result = currentDate.Date <= endDate.Date;
          break;
        case 2: // On or After
          operationName = "On or After";
          result = currentDate.Date >= parsedDate.Date;
          break;
        case 3: // Before
          operationName = "Before";
          result = currentDate.Date < parsedDate.Date;
          break;
        case 4: // After
          operationName = "After";
          result = currentDate.Date > endDate.Date;
          break;
        case 5: // On
          operationName = "On";
          result = currentDate.Date >= parsedDate.Date && currentDate.Date <= endDate.Date;
          break;
        case 6: // Not On
          operationName = "Not On";
          result = currentDate.Date < parsedDate.Date || currentDate.Date > endDate.Date;
          break;
        default:
          Main.Logger.LogError($"[EvaluateTimeline] Invalid operation: {operation}");
          return false;
      }

      Main.Logger.Log($"[EvaluateTimeline] RESULT: Operation '{operationName}' (op={operation}) returned {result}");
      return result;
    }

    public static object EvaluateDaysSinceDate(TsEnvironment env, object[] inputs) {
      Main.Logger.Log($"[EvaluateDaysSinceDate] Entered EvaluateDaysSinceDate condition.");

      string date = env.ToString(inputs[0]); // format is like either 3050 (year only), 3050-02 (year and month) or 3050-02-01 (year, month and day)
      int operation = env.ToInt(inputs[1]);
      int compareDays = env.ToInt(inputs[2]);
      DateTime currentDate = UnityGameInstance.BattleTechGame.Simulation.CurrentDate;
      DateTime parsedDate;

      // Debug logging - Log all inputs
      Main.Logger.Log($"[EvaluateDaysSinceDate] TRIGGERED: dateString='{date}', operation={operation}, compareDays={compareDays}, currentDate={currentDate.Date.ToString("yyyy-MM-dd")}");

      // Parse date with flexible format support
      switch (date.Length) {
        case 4: // year only
          if (!int.TryParse(date, out int yearOnly)) {
            Main.Logger.LogError($"[EvaluateDaysSinceDate] Year is not a valid integer. Provided date: '{date}'");
            return false;
          }

          parsedDate = new DateTime(yearOnly, 1, 1).Date;
          Main.Logger.Log($"[EvaluateDaysSinceDate] Parsed year-only: parsedDate={parsedDate:yyyy-MM-dd}");
          break;
        case 6: // year and month (yyyy-M format like "3050-6")
        case 7: // year and month (yyyy-MM format like "3050-06")
          // Support both zero-padded (yyyy-MM) and non-zero-padded (yyyy-M) month formats
          string[] yearMonthFormats = { "yyyy-MM", "yyyy-M" };
          if (!DateTime.TryParseExact(date, yearMonthFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            Main.Logger.LogError($"[EvaluateDaysSinceDate] Year-month date is not in a valid format (yyyy-MM or yyyy-M). Provided date: '{date}'");
            return false;
          }

          parsedDate = parsedDate.Date; // Strip time component
          Main.Logger.Log($"[EvaluateDaysSinceDate] Parsed year-month: parsedDate={parsedDate:yyyy-MM-dd}");
          break;
        case 8: // year, month and day (yyyy-M-d format like "3050-6-5")
        case 9: // year, month and day (yyyy-MM-d or yyyy-M-dd format like "3050-06-5" or "3050-6-05")
        case 10: // year, month and day (yyyy-MM-dd format like "3050-06-05")
          // Support multiple full date formats (zero-padded and non-zero-padded)
          string[] fullDateFormats = { "yyyy-MM-dd", "yyyy-M-d", "yyyy-MM-d", "yyyy-M-dd" };
          if (!DateTime.TryParseExact(date, fullDateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            Main.Logger.LogError($"[EvaluateDaysSinceDate] Date is not in a valid format (yyyy-MM-dd, yyyy-M-d, etc.). Provided date: '{date}'");
            return false;
          }

          parsedDate = parsedDate.Date; // Strip time component
          Main.Logger.Log($"[EvaluateDaysSinceDate] Parsed full date: parsedDate={parsedDate:yyyy-MM-dd}");
          break;
        default: {
          Main.Logger.LogError($"[EvaluateDaysSinceDate] Invalid date format: {date}");
          return false;
        }
      }

      // Calculate days since the parsed date
      int daysSince = (int)(currentDate.Date - parsedDate.Date).TotalDays;
      Main.Logger.Log($"[EvaluateDaysSinceDate] Days since {parsedDate:yyyy-MM-dd}: {daysSince}");

      // Perform comparison based on operation
      bool result;
      string operationName;

      switch (operation) {
        case 1: // less than
          operationName = "Less Than";
          result = daysSince < compareDays;
          break;
        case 2: // equal to
          operationName = "Equal To";
          result = daysSince == compareDays;
          break;
        case 3: // greater than
          operationName = "Greater Than";
          result = daysSince > compareDays;
          break;
        case 4: // less than or equal to
          operationName = "Less Than Or Equal To";
          result = daysSince <= compareDays;
          break;
        case 5: // greater than or equal to
          operationName = "Greater Than Or Equal To";
          result = daysSince >= compareDays;
          break;
        default:
          Main.Logger.LogError($"[EvaluateDaysSinceDate] Invalid operation: {operation}");
          return false;
      }

      Main.Logger.Log($"[EvaluateDaysSinceDate] RESULT: Operation '{operationName}' (op={operation}) comparing {daysSince} days against {compareDays} returned {result}");
      return result;
    }
  }
}