using System.Collections.Generic;

namespace ExtendedConversations {
    public class Settings {
        // Debug logging flags
        public bool EnableDebugLogging = false;
        public bool DebugLogConditions = false;
        public bool DebugLogActions = false;
        public bool DebugLogNodeText = false;

        /// <summary>
        /// List of SimGameInterruptManager.InterruptType names that are allowed to display during Time Skip operations when DisablePopups is enabled.
        /// </summary>
        public List<string> TimeSkipAllowedInterrupts = new List<string> { };
    }
}