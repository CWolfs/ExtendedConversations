using BattleTech;

namespace ExtendedConversations.State {
  public class TimeSkipStateManager {
    private static TimeSkipStateManager _instance;
    private static readonly object _lock = new object();

    /// <summary>
    /// Singleton instance accessor
    /// </summary>
    public static TimeSkipStateManager Instance {
      get {
        if (_instance == null) {
          lock (_lock) {
            if (_instance == null) {
              _instance = new TimeSkipStateManager();
            }
          }
        }
        return _instance;
      }
    }

    /// <summary>
    /// Indicates if a time skip operation is currently active
    /// </summary>
    public bool IsTimeSkipActive { get; private set; }

    /// <summary>
    /// When true, costs (quarterly funds deduction) should be suppressed during time skip
    /// </summary>
    public bool DisableCost { get; private set; }

    /// <summary>
    /// When true, popups and notifications should be suppressed during time skip
    /// </summary>
    public bool DisablePopups { get; private set; }

    /// <summary>
    /// Private constructor for singleton pattern
    /// </summary>
    private TimeSkipStateManager() {
      Reset();
    }

    /// <summary>
    /// Sets the time skip state and options
    /// </summary>
    public void SetTimeSkipState(bool isActive, bool disableCost, bool disablePopups) {
      IsTimeSkipActive = isActive;
      DisableCost = disableCost;
      DisablePopups = disablePopups;

      Main.Logger.Log($"[TimeSkipStateManager] State updated: IsActive={isActive}, DisableCost={disableCost}, DisablePopups={disablePopups}");
    }

    /// <summary>
    /// Resets all time skip state to default values
    /// </summary>
    public void Reset() {
      IsTimeSkipActive = false;
      DisableCost = false;
      DisablePopups = false;
    }
  }
}
