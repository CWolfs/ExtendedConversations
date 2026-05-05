# Extended Conversations - BattleTech Harmony Mod

## Project Overview

This is a BattleTech Harmony mod that extends the game's conversation system with enhanced functionality. The mod adds custom **actions**, **conditions**, and **value getters** to the TScript conversation scripting system, enabling richer and more dynamic conversations.

**Philosophy**: Bite-sized bits of functionality - each operation does one thing well.

### Key Features
- 15 custom actions (time skip, travel, funds management, conversation control, etc.)
- 7 custom conditions (stat evaluation, date comparison, funds checks, etc.)
- 4 custom value getters (retrieve stats and game state)
- Text interpolation with `<Stats.*>` and `<Game.*>` tags in dialog
- Integration with ConverseTek conversation editor

## Development Environment

### Publicizer Usage
**IMPORTANT**: This project uses `Krafs.Publicizer` package to access internal/private members of BattleTech assemblies. **No reflection is needed for accessing private classes or methods** - the publicizer makes them accessible at compile time.

Configuration in .csproj:
```xml
<ItemGroup>
  <Publicize Include="Assembly-CSharp" />
  <Publicize Include="DOTween" />
  <Publicize Include="DOTweenPro" />
  <Publicize Include="UnityEngine.CoreModule" />
</ItemGroup>
<PublicizerRuntimeStrategies>Unsafe</PublicizerRuntimeStrategies>
```

Only use ReflectionHelper.cs for legacy code or edge cases.

### Decompiled BattleTech Source Code
Decompiled game source available at: `F:\ProtonDrive\My files\Battletech Modding\Source\1.9.1\Assembly-CSharp`

Use this reference to:
- Understand game class structure and methods
- Find patch points for new features
- Verify method signatures and parameter types
- Understand game mechanics and state management

## Project Structure

```
src/
├── Core/                          # Core functionality
│   ├── Actions.cs                 # All action implementations
│   ├── Conditions.cs              # All condition implementations
│   ├── ValueGetters.cs            # All value getter implementations
│   ├── ConversationUpgrades.cs    # TScript operation registration
│   ├── ConversationInterpolator.cs # Dialog tag interpolation
│   ├── DateHelper.cs              # Date parsing and fuzzy comparison
│   └── ContractHelpers.cs         # Contract generation helpers
├── Patches/                       # Harmony patches (organized by feature)
│   ├── Actions/                   # Action-specific patches
│   │   ├── General/
│   │   ├── TimeSkip/
│   │   ├── SideloadConversation/
│   │   └── SetCameraHardLock/
│   ├── Conditions/               # Condition-specific patches
│   ├── TsEnvironmentPatch.cs     # Main TScript integration hook
│   └── DialogPanel.cs            # Text interpolation patch
├── State/                         # State management
│   ├── TimeSkipStateManager.cs   # Singleton for time skip state
│   └── SideloadConversationState.cs # Conversation sideload state
├── Util/                          # Utility classes
│   ├── Logger.cs                 # Custom logging infrastructure
│   ├── SimHelper.cs              # SimGameState helpers
│   ├── ReflectionHelper.cs       # Reflection utilities (legacy)
│   ├── ConversationUtils.cs      # Conversation helpers
│   ├── ConversationExtensions.cs # Conversation extension methods
│   ├── MetaDatabaseExtensions.cs # MetadataDatabase extensions
│   └── EnumUtils.cs              # Enum utilities
├── Main.cs                        # Mod entry point
└── Settings.cs                    # Mod settings/configuration

operations/                        # ConverseTek editor definitions
└── *.json                        # Operation UI/parameter definitions
```

## Coding Conventions

### Naming Conventions

**Classes**
- PascalCase: `TimeSkipStateManager`, `DateHelper`, `Actions`
- Patch classes: Suffix with "Patch" - `SimGameConversationManagerDoNodeActionsPatch`
- Helper/Utility classes: Suffix with "Helper", "Manager", "Extensions"
- State classes: Suffix with "State" - `SideloadConversationState`

**Methods**
- PascalCase for all methods: `TimeSkip`, `ProcessSideloadConversationPatch`
- Action/Condition/ValueGetter names match TScript operation names exactly

**Fields**
- Private static: camelCase with underscore prefix - `_instance`, `_lock`
- Public static: PascalCase - `IsNodeAction`, `ActiveConversation`
- Constants: ALL_CAPS with underscores - `MAX_RECURSION_DEPTH`

**Properties**
- PascalCase: `IsTimeSkipActive`, `DisableCost`, `Instance`
- Explicit get/set: `{ get; set; }` or `{ get; private set; }`

**Local Variables and Parameters**
- camelCase: `daysToSkip`, `targetDateRaw`, `simulation`, `env`, `inputs`

### TScript Integration Pattern

All actions, conditions, and value getters follow this pattern:

**Registration in ConversationUpgrades.cs:**
```csharp
// Get type references
var voidType = env.GetType("void");
var intType = env.GetType("int");
var stringType = env.GetType("string");

// Declare operation
Main.Logger.Log("Declaring 'Time Skip' action");
tsOp = env.DeclareOp("EffectFunctions", "Time Skip", voidType,
    new TsOp.EvalDelegate(Actions.TimeSkip));

// Declare inputs (parameters)
tsOp.DeclareInput("days", intType);
tsOp.DeclareInput("disableCost", intType);
tsOp.DeclareInput("disablePopups", intType);
```

**Implementation in Actions.cs/Conditions.cs/ValueGetters.cs:**
```csharp
public static object OperationName(TsEnvironment env, object[] inputs) {
    // 1. Extract and convert parameters
    int param1 = env.ToInt(inputs[0]);
    string param2 = env.ToString(inputs[1]);
    bool param3 = (inputs.Length >= 3) ? env.ToBool(inputs[2]) : false; // Optional

    // 2. Log entry with parameters
    Main.Logger.Log($"[OperationName] Triggered with param1 '{param1}', param2 '{param2}'");

    // 3. Get game objects
    SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

    // 4. Validate
    if (/* validation fails */) {
        Main.Logger.LogError($"[OperationName] Error message");
        return false; // or null for actions
    }

    // 5. Implementation
    try {
        // Core logic here
    } catch (Exception ex) {
        Main.Logger.LogError($"[OperationName] Exception: {ex.Message}");
        Main.Logger.LogError($"[OperationName] Stack trace: {ex.StackTrace}");
    } finally {
        // Cleanup if needed
    }

    // 6. Log completion
    Main.Logger.Log($"[OperationName] Complete");

    // 7. Return
    return result; // bool for conditions, typed value for getters, null for actions
}
```

**Operation Categories:**
- `"ConditionFunction"` - Conditions (return bool)
- `"EffectFunctions"` - Actions (return null)
- `"ValueGetterFunctions"` - Value getters (return typed value)

### Harmony Patch Pattern

```csharp
[HarmonyPatch(typeof(TargetClass), "MethodName")]
public class TargetClassMethodNamePatch {
    // Prefix - runs before original, can skip original by returning false
    static bool Prefix(TargetClass __instance, ref paramType param1) {
        try {
            // Patch logic
            if (shouldSkipOriginal) return false;
            return true; // Run original method
        } catch (Exception e) {
            Main.Logger.LogError($"[ClassName] Error: {e.StackTrace}");
            return true; // Allow original to run on error
        }
    }

    // Postfix - runs after original (can't prevent original execution)
    static void Postfix(TargetClass __instance, ReturnType __result) {
        // Post-processing logic
    }
}
```

**Patch Naming**: `{TargetClass}{MethodName}Patch`
- Example: `SimGameConversationManagerDoNodeActionsPatch`
- Organized in folders by feature: `Patches/Actions/TimeSkip/`

### State Management Patterns

**Singleton Pattern (Thread-Safe Double-Check Locking):**
```csharp
public class StateManager {
    private static StateManager _instance;
    private static readonly object _lock = new object();

    public static StateManager Instance {
        get {
            if (_instance == null) {
                lock (_lock) {
                    if (_instance == null) {
                        _instance = new StateManager();
                    }
                }
            }
            return _instance;
        }
    }

    private StateManager() {
        Reset();
    }
}
```

**Cross-Patch State Sharing:**
Use static fields in Actions.cs for coordination:
```csharp
public static bool IsNodeAction { get; set; } = false;
public static bool IsLinkAction { get; set; } = false;
public static Conversation ActiveConversation = null;
```

Multiple patches read/write these to coordinate behavior.

### Error Handling

**Try-Catch-Finally for Actions:**
```csharp
try {
    stateManager.SetTimeSkipState(true, disableCost, disablePopups);
    // Implementation
    Main.Logger.Log($"[TimeSkip] Skip complete");
} catch (Exception ex) {
    Main.Logger.LogError($"[TimeSkip] Exception: {ex.Message}");
    Main.Logger.LogError($"[TimeSkip] Stack trace: {ex.StackTrace}");
} finally {
    stateManager.Reset();
}
```

**Validation and Early Return:**
```csharp
if (string.IsNullOrEmpty(eventDefId)) {
    Main.Logger.LogError("[TriggerEvent] No eventDefId provided");
    return null;
}

if (!simulation.DataManager.SimGameEventDefs.Exists(eventDefId)) {
    Main.Logger.LogError($"[TriggerEvent] Invalid eventDefId '{eventDefId}'");
    return null;
}
```

**Graceful Degradation in Patches:**
```csharp
static bool Prefix() {
    try {
        // Patch logic
        return true;
    } catch (Exception e) {
        Main.Logger.LogError("[Patch] Error caught: " + e.StackTrace);
        return true; // Allow original method to run
    }
}
```

**Null Coalescing:**
```csharp
int nodeIndex = node?.index ?? -1;
string nodeText = node?.text ?? "";
int actionCount = node?.actions?.ops?.Count ?? 0;
```

### Logging Conventions

**Consistent Log Tags:**
- All messages prefixed with `[MethodName]` in square brackets
- Example: `Main.Logger.Log($"[TimeSkip] Triggered with days '{daysToSkip}'");`

**Logging Levels:**
- `Main.Logger.Log()` - Info/debug messages
- `Main.Logger.LogWarning()` - Non-critical warnings
- `Main.Logger.LogError()` - Critical errors

**Entry/Exit Logging:**
```csharp
// Entry with parameters
Main.Logger.Log($"[MethodName] Triggered with param1 '{param1}', param2 '{param2}'");

// Exit
Main.Logger.Log($"[MethodName] Complete");
Main.Logger.Log($"[MethodName] RESULT: {result}");
```

**Conditional Debug Logging:**
```csharp
if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogActions) {
    Main.Logger.Log($"[DoNodeActions] Executing {actionCount} actions");
}
```

**Structured Output:**
```csharp
Main.Logger.Log($"\n========================================");
Main.Logger.Log($"[EndConversation] Ended: {conversationName}");
Main.Logger.Log($"========================================\n");
```

### Documentation Style

**XML Comments:**
```csharp
/// <summary>
/// Indicates if a time skip operation is currently active
/// </summary>
public bool IsTimeSkipActive { get; private set; }
```

**Inline Comments:**
- Use `// Comment` format (space after slashes)
- Explain complex logic liberally
- Mark TODOs: `// TODO: Description`
- Label guard clauses: `// GUARD`

### Common Patterns and Gotchas

**TScript "0" String Convention:**
TScript converts empty strings to "0", requiring special handling:
```csharp
if (employerInput == "0") employerInput = null;
if (string.IsNullOrEmpty(employerInput)) employerInput = null;
```

**Optional Parameters:**
Check length before accessing:
```csharp
bool optionalParam = (inputs.Length >= 3) ? env.ToBool(inputs[2]) : false;
```

**Extension Methods:**
Use for adding functionality to game classes:
```csharp
public static class ConversationExtensions {
    public static ConversationLink GetRootToFollow(this Conversation conversation) {
        // Implementation
    }
}
```

**Unity Coroutines:**
For delayed/async operations:
```csharp
UnityGameInstance.Instance.StartCoroutine(WaitThenExecute(...));

static IEnumerator WaitThenExecute(...) {
    yield return new WaitForSeconds(1);
    // Execute after delay
}
```

## Key Game Interfaces

### Primary Game Classes

**SimGameState** - Main simulation state
```csharp
SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
simulation.AddFunds(amount);
simulation.CurrentDate;
simulation.CompanyStats;  // StatCollection
simulation.CommanderStats; // StatCollection
```

**SimGameConversationManager** - Conversation management
```csharp
SimGameConversationManager manager = simulation.ConversationManager;
manager.thisConvoDef;    // Current conversation
manager.currentNode;      // Current node
manager.DoNodeActions();
```

**SimGameInterruptManager** - Popup/interrupt queue
```csharp
SimGameInterruptManager interruptQueue = simulation.interruptQueue;
interruptQueue.QueueConversation(conversation, header, subheader);
interruptQueue.DisplayIfAvailable();
```

**StarSystem** - Star system data
```csharp
StarSystem system = simulation.CurSystem;
system.Tags;  // TagSet
system.Stats; // StatCollection
```

**MetadataDatabase** - Game content database
```csharp
MetadataDatabase mdd = MetadataDatabase.Instance;
mdd.GetReleasedMapsAndEncountersByContractTypeAndOwnership(...);
```

### Stat Scopes (SimHelper.cs)

Access different StatCollection scopes via SimHelper:
- **1 (Company)**: `simulation.CompanyStats`
- **2 (Commander)**: `simulation.CommanderStats`
- **3 (CurrentSystem)**: `simulation.CurSystem.Stats`
- **4 (Flashpoint)**: `simulation.CompanyStats` (flashpoint stats stored here)

Usage:
```csharp
StatCollection stats = SimHelper.GetStatCollectionByScope(simulation, scope);
```

## Operations Reference

### Actions (15 total)
- `Time Skip` - Skip forward in time with mechanics processing
- `Time Skip To Date` - Skip to specific date
- `Set Current System` - Travel to star system
- `Modify Funds` - Add/remove C-Bills
- `Set Characters Visible` - Show/hide characters
- `Start Conversation` - Start group conversation
- `Sideload Conversation` - Load conversation seamlessly
- `Add Contract` - Add contract to list
- `Add Flashpoint` - Add flashpoint to starmap
- `Set Camera Hard Lock` - Lock camera to target
- `Add Mech` - Add mech to mechbay
- `Trigger Event` - Trigger specific event
- `Trigger Random Event` - Trigger random event
- `Reset BattleTech Viewscreen` - Reset viewscreen
- `Take Contract` - Immediately start contract

### Conditions (7 total)
- `Evaluate Tag For Current System` - Check system tags
- `Evaluate BattleTech String` - Check string stats (with date support)
- `Evaluate BattleTech Int` - Check integer stats
- `Evaluate BattleTech Float` - Check float stats
- `Evaluate Funds` - Check company funds
- `Evaluate Timeline` - Check dates vs current timeline
- `Evaluate Days Since Date` - Check days elapsed

### Value Getters (4 total)
- `Get BattleTech String` - Retrieve string stat
- `Get BattleTech Int` - Retrieve integer stat
- `Get BattleTech Float` - Retrieve float stat
- `Get Current Date` - Get simulation date

## Special Features

### Text Interpolation
ConversationInterpolator supports dialog tags:
- `<Stats.Company.StatName>` - Company stats
- `<Stats.Commander.StatName>` - Commander stats
- `<Stats.CurrentSystem.StatName>` - System stats
- `<Game.CurrentDate>` - Current date (yyyy-MM-dd format)

### Flexible Date Parsing (DateHelper.cs)
Supports multiple formats with fuzzy comparison:
- `"yyyy"` - Year only (2025)
- `"yyyy-MM"` - Year-month (2025-03)
- `"yyyy-MM-dd"` - Full date (2025-03-15)

Comparison precision matches less precise value:
- `"2025"` vs `"2025-03-15"` → compares at year precision
- `"2025-03"` vs `"2025-03-15"` → compares at month precision

## Build Configuration

**Target Framework**: .NET Framework 4.7.1 (Unity 2018.4 compatible)

**Dependencies:**
- HarmonyX (0Harmony.dll)
- Unity Engine (UnityEngine.dll, UnityEngine.CoreModule.dll)
- BattleTech (Assembly-CSharp.dll, ShadowrunDTO.dll)
- Newtonsoft.Json
- Krafs.Publicizer v2.2.1 (NuGet)

**Assembly References**: Currently hardcoded to developer's Steam path in .csproj

## Testing and Debugging

**Debug Settings in mod.json:**
```json
"Settings": {
    "EnableDebugLogging": true,
    "DebugLogConditions": true,
    "DebugLogActions": true,
    "DebugLogNodeText": true
}
```

**Log Location**: Check BattleTech logs directory for mod-specific log file

## Contributing Guidelines

When adding new operations:

1. **Add implementation** to Actions.cs, Conditions.cs, or ValueGetters.cs
2. **Register in ConversationUpgrades.cs** with proper type and inputs
3. **Create operation JSON** in operations/ folder for ConverseTek
4. **Follow naming conventions** exactly
5. **Add comprehensive logging** with [MethodName] tags
6. **Handle errors gracefully** with try-catch
7. **Validate all inputs** with early returns
8. **Document complex logic** with inline comments
9. **Test edge cases** (null inputs, empty strings, "0" strings)
10. **Update this file** if adding new patterns or conventions

## References

- **BattleTech Modding Wiki**: https://www.battletechgame.com/forums/
- **HarmonyX Documentation**: https://github.com/BepInEx/HarmonyX/wiki
- **ConverseTek**: Conversation editor tool (separate application)
