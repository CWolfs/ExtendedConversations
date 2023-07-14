# Extended Conversations

Extended Conversations is a HBS' BattleTech conversation-based utility mod. It extends the conversation system to provide more actions, conditions and more.

Use [ConverseTek - Sim Conversation Editor](https://github.com/CWolfs/ConverseTek) for support with the additional functionality that this mod provides.

## Installation Instructions

- Move into your ModTek `Mods` folder
- Move the `operations` folder into your `ConverseTek/defs` folder. This will enable all the conditions, actions and value getters from Extended Conversations in ConverseTek.

## Features

### Dialog Tags

Dialog tags allow you to inject data into your dialog text.

- `<Game.CurrentDate>` - This allows you to inject the current date into the dialogue in the format of yyyy-MM-dd (e.g. `3050-01-20`)
- `<Stats.Company.[StatName]>` - This allows you to inject any company stat (e.g. `<Stats.Company.MyStatName>`)
- `<Stats.Commander.[StatName]`> - This allows you to inject any commander stat
- `<Stats.CurrentSystem.[StatName]`> - This allows you to inject any current system stat

### Conditions

Conditions control if a conversation Response Node (yellow node) appears or not.

- `Evaluate Tag for Current System` - This allows you to check if the current star system has the tag specified.
- `Evaluate BattleTech String` - This allows you to check against a commander, company or current system string statistic.
- `Evaluate BattleTech Int` - This allows you to check against a commander, company or current system integer statistic.
- `Evaluate BattleTech Float` - This allows you to check against a commander, company or current system float statistic.
- `Evaluate Funds` - This allows you to check a fund amount against the company funds.
- `Evaluate Timeline` - This allows you to flexibly check and test a date against the current timeline

### Actions

Actions are pieces of functionality that can be run.

- `Time Skip` - This allows you to jump forward in time by the set amount. This processes the usual mechanics like healing, repairs and monthly fees.
- `Set Current System` - This allows you to set the current star system by star system id (e.g. starsystemdef_Smithon), with the option of using the calculated travel time or not.
- `Modify Funds` - This allows you to add, or remove, X amount of cbills from the company funds.
- `Start Conversation Custom` - This starts a group conversation for you. You specify the conversation id, header and subheader.
- `Set Characters Visible` - This allows you to show and hide characters in the dropship. You specify the characters in a comma separated list.
- `Add Contract` - This allows you to add a contract to the contracts list in the XOs room.
- `Add Flashpoint` - This allows you to add a flashpoint to the starmap. See [usage instructions](https://github.com/CWolfs/ExtendedConversations/issues/44#issuecomment-1335134292).
- `Set BattleTech Camera Hard Lock` - This has one advantage over the vanilla `Set BattleTech Camera Lock`. It is useful in 1-on-1 conversations and letting other characters talk without transitioning away. The vanilla lock doesn't work for 1-on-1 conversations.
- `Sideload Conversation` - This loads another conversation into the current active conversation. It's seamless so you it feels like it's the same conversation. It can enter the new conversation at a specific node and can also return to the original conversation if enabled. **Important: It's recommended you use this on Response nodes, but you can use it on Prompt nodes too. When using on Prompt nodes conditionals for the following responses at that specific level might not work**.
- `Add Mech` - This allows you to add a mech to your mechbay. If the mechbay is full it will present you with the usual overflow popup. You can choose to silently award the mech (unless it overflows).
- `Trigger Event` - This allows you to trigger a specific event. It gives you control over the requirements being tested, forcing the trigger if the event is already in the discard pile and if you want to add it to the discard pile.
- `Trigger Random Event` - This simple action allows you to trigger a random event. This is similar to the vanilla random event system.

### Value Getters

- `Get BattleTech String` - This can be used in the above conditions and actions. It allows you to grab a commander, company or current system string statistic for use in the other operations. An example would be storing a system id in a company stat, then using this to pull out the system id in the `Set Current System` action.
- `Get BattleTech Int` - Same as above but for the return type of 'int'
- `Get BattleTech Float` - Same as above but for the return type of 'float'

## Author

Richard Griffiths (CWolf)

- [Twitter](https://twitter.com/CWolf)
- [LinkedIn](https://www.linkedin.com/in/richard-griffiths-436b7a19/)
