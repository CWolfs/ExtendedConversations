using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

using BattleTech;

using ExtendedConversations.Utils;

namespace ExtendedConversations.Core {
  public class ConversationInterpolator {
    private static ConversationInterpolator ci;

    public static ConversationInterpolator GetInstance() {
      if (ci == null) ci = new ConversationInterpolator();
      return ci;
    }

    public ConversationInterpolator() {
      Main.Logger.Log("Initialising conversation interpolator");
    }

    public string Interpolate(string text) {
      Main.Logger.Log($"[InterpolateStats] text '{text}'");

			Regex regex = new Regex("\\<.*?\\>");
      MatchCollection matchCollection = regex.Matches(text);
			while (matchCollection.Count > 0) {
				Match match = matchCollection[0];
				string value = match.Value;
				string expression = null;
				if (value.Length > 2) {
					expression = value.Substring(1, value.Length - 2).Trim();
				}
				int index = match.Index;
				int length = match.Length;

        string[] lookups = expression.Split('.');
        string actionType = lookups[0].ToLower();

        string interpolatedText = "";
        if (actionType.ToLower() == "stats") {
          interpolatedText = this.InterpolateStats(text, lookups);
        } else {
          Main.Logger.LogError($"[InterpolateStats] Unknown interpolation action of {actionType}");
        }

        text = text.Remove(index, length).Insert(index, interpolatedText);
        matchCollection = regex.Matches(text);
      }

      return text;
    }

    /*
      Looks up any stats element and replaces that over the placeholder
    */
    private string InterpolateStats(string text, string[] lookups) {
      if (lookups.Length < 3) {
        Main.Logger.LogError($"[InterpolateStats] {{Expression}} needs to be in the format of {{CI.StatType.StatKey}}");
        return $"[InterpolateStats] {{Expression}} needs to be in the format of {{CI.StatType.StatKey}}";
      }

      string statType = lookups[1];
      string statName = lookups[2];
      StatCollection statCollection = SimHelper.GetStatCollection(statType);

      if (statCollection.ContainsStatistic(statName)) {
        string statValue = statCollection.GetValue<string>(statName);
        return statValue;
      } else {
        Main.Logger.LogError($"[InterpolateStats] Stat '{statName}' does not exist for stat collection '{statType}'");
        return $"[InterpolateStats] Stat {statName} does not exist for stat collection {statType}";
      }	
    }
  }
}