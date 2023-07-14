using System.Linq;
using System.Collections.Generic;

using BattleTech.Data;
using BattleTech;

public static class MetaDatabaseExtensions {
  public static List<EventDef_MDD> GetEventDefMDD(this MetadataDatabase mdd, SimGameEventDef simGameEventDef) {
    string str = "SELECT ed.* from EventDef ed ";
    str += "WHERE ed.EventDefId = @EventDefId";

    return mdd.Query<EventDef_MDD>(str, new {
      EventDefId = simGameEventDef.Description.Id,
    }).ToList();
  }
}