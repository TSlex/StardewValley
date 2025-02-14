[(← Back to readme)](README.md)

## 2.1.0
Released 14th February 2025 for SMAPI 4.1.10 or later.

New features:
- 2 new modes as an alternative to BUY/SELL and COMBINED modes that use their own special currency (`jmt`)
- Extented list of items that support meta saving (new meta-saving approach)
    - weapons and tools (enchantments)
    - trinkets levels and other characteristics
    - item colors (flowers and other items that can have color variants)
    - combined rings
- Option to disable command usage for non-host players
- Option to enable automatic research (will hold research button for you :) )
- Flavored items (honey, special bait, smoked fish, etc...) can now be researched as separate items
- 2 new commands for new modes (can increase or decrease currency)
- Pressing `left ctrl` + `rmb` will take `10` items from "research inventory"


Changes:
- Trinkets now have own category (`trinkets`)
- Torches moved to `craftable` category
- Artifacts from Ginger Island moved to `artifacts` category
- Qi Seasoning moved to `cooking`
- Some "tools" (horse flute, animal cracker, etc...) moved to `tools` category
- Some "treasure" items (pearl, golden pumpkin, etc...) moded to `treasure` category
- Mannequins moved to `non-craftable` category
- A lot of price balancing, to prevent too unfair money gaining in-game early stages (but still, have you heard about The Smoked Legendary Fish? ;) )
- Changed some colors
- And more...?


Bug fixes:
- Improved unique key matching for some items
- Fixed commands stopped working (God bless ConcernedApe!)
- Items with their own inventory (fishing rods, slingshots) will dump their content once researched/sold (you don't want to lose your precious bait, do ya?)
- Fixed quality merging for the same items
- Research area will no longer convert items to broken ones if it cannot find it in the registry
- And more...?

## 2.0.1
Released 4th August 2024 for SMAPI 4.0.8 or later.

Fixes:
- ResearchAmountMultiplier will reduce the required research amount not lower than 1
- Added check from incorrect values from config.json
- Fix for "Auto Loot Treasures" feature of the Joys of Efficiency (now menu content will not be looted)
- Fix for incorrect menu position caused by the change of player inventory position, ex: Bigger Backpack mod

## 2.0.0
Released 15th July 2024 for SMAPI 4.0.8 or later.

Added: 
- Updated for SDV 1.6 (Mostly rewritten from scratch)
- UI redesign
- Option to mark the item as a favorite and filter
- Progression display options
- Animations and sounds
- 2 new modes
- Saving item meta (clothes color and watering can level)
- Updated Mod Config Menu compatibility

Removed:
- Removed some commands 
- Removed the ability to have custom categories and price list for each save. They are now global under assets/config

## 1.0.1
Released somewhere the end of 2023 for SMAPI 3.9.5 or later.

Additions:
* Combined mode
* Buy and sell global multipliers in config
* Research amount global multiplier in config
* Item price tooltip is changed to show both buy and sell prices if they are not the same

Bugfixes:
* Optimized menu
* Few pricelist balance updates

## 1.0.0
Released 23th August 2021 for SMAPI 3.9.5 or later.

New stuff:
* Linux (and possible MacOS) support
* New commands ([see documentation](README.md#commands))
* Saving in game files
* Added banlist for any item that can break game (like quest items)
* New assets arrangement
* Multiplayer support (remote players)
* New mod config option
* In buy mode counter was changed to "($$$)"
* If item cannot be researched, instead of counter there will be "(X)"

Bugfixes:
* Fixed copper pan being hat instead of tool
* Fixed item not appearing in menu after research in some cases
* Fixed eat animation breaking
* Fixed mod crash on new worlds
* Fixed price changing command

## 1.0.0-alpha-r2
Released 24th June 2021 for SMAPI 3.9.5 or later.

* Added support for translation (menu only)
* Added russian translation (menu only)
* Added new tab to show player balance  
* Added ProjectE like mode (research costs 1, sell and buy any items)
* Added "Junk" category
* Moved hay to "Forage" category instead of "Misc"
* Added commands for ProjectE like mode
* Shift click insta deletes (or sells) items if they are researched
* Added support for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)

## 1.0.0-alpha
Released 15th June 2021 for SMAPI 3.9.5 or later.

* Initial release