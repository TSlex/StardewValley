SDV 1.6 Status: Version 2.0 is finally completed, thank you for waiting <3

Secret Note #1: some items (especially new) might be imbalanced by price or have the wrong category, and it needs a lot more time to figure it all out (and I don't want to delay release anymore). If you want to help, please submit your suggestions in the forum :)

Secret Note #2: due to new ID and naming mechanics introduced by SDV 1.6 already researched items in SDV 1.5 might get lost. I added an option to see those items (will appear as "Missing Item" in the menu with the original "unique key"), so you can find and research them again.

## Description

This mod introduces mechanics similar to Terraria's Journey Mode and Minecraft's ProjectE mod, where you can "research" an item and create much more of it (for free or by spending some currency). There are different modes and settings available, allowing you to play the mod the way you like, more game-progression friendly or straight-up cheaty.

## How to use

1. Press `R` (or your configured key) to open the menu.
2. Place items into the research area (a small animated book).
3. Press the research button (under research area) until the item disappears from the research area.
4. Check the menu, if the amount of item was sufficient, the researched item will appear in the menu and leftovers will be returned to your inventory.

Press `left alt` to mark/unmark the item as a favorite.
Press `left ctrl` to quickly move items to the research area and back. If the item is researched, it will be sold or removed (bypassing the move to the research area).
Press `left ctrl` on a trash can to remove all researched items from inventory (only for Research and Mr. Qi modes)

## Install

1. [Install the latest version of SMAPI](https://smapi.io/).
2. Download the mod and unzip it into Stardew Valley/Mods/ directory.
3. Start the game using [SMAPI](https://stardewvalleywiki.com/Modding:Installing_SMAPI_on_Windows#Configure_your_game_client).

## Сompatibility
- **Stardew Valley 1.6** (*for SDV 1.5 use version 1.0.1*).
- **SMAPI 4.0.8** or later.
- **Windows**, *Linux*, *MacOS* (Linux and MacOS not tested, but should work ok)
- **Singleplayer** or **Multiplayer** (not split screen). For multiplayer, the mod must be installed by the host and optionally installed for other players (if they want to use the menu themselves).
- **Keyboard+Mouse only** (gamepad support will be added later, maybe...)
- Items added by other mods should work, but are not guaranteed. Besides that, there is no known mod conflict.

## Features

- 5 different modes to choose from:
	- Research mode - each item has own required amount for research, after research completion you can generate as much item you want for free.
	- Buy/Sell mode - you need only 1 of each item for research, giving items will cont in-game money and you can sell items as well. Also shows the money bar and tooltip for item price.
	- Combined mode - researching like in Research mode, after research completion like in Buy/Sell mode.
	- Mr. Qi mode - Research mode but all items already researched/unlocked. Basically **CJB Item Spawner** functionality.
	- JojaMart mode - Buy/Sell mode but all items already researched/unlocked. Items costs a lot, and sell price is 1. For true JojaMart fans. :)

- Filtering:
    - Research progression tracking.
    - Favorite item filtering.

- Higly customizable:
    - Customizable buy and sell prices (using corresponding modifier).
    - Customizable research amount (using corresponding modifier).
    - Customizable categories, pricelist and item blacklist (using corresponding .json file in assets/config/ folder).

## Configuration

First of all, installing [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) is mandatory, as this is the only way to change settings for the started save file. A config.json file is only for the default setting for new farms, if you want to change them after you start the save, you have to use Config Menu mod. Anyway, you already using it, aren't you? ;)

Below are listed possible configurations. Note: some of them can be changed by the host player only (does not matter in singleplayer):
- `ShowMenuButton` - a key to open the menu. [Default is R]
- `DefaultMode` - actually the active menu mode for active saves. [Default is "Research"] [Host only]
- `ShowMissingItems` - to show each "Missing Item" in the menu if there are any. [Default is false]
- `EnableSounds` - determines if the mod menu is muted or not. [Default is true]
- `ResearchAmountMultiplier` - multiplies the base research amount. [Default is 1.5] [Host only]
- `SellPriceMultiplier` - multiplies the item selling price in the menu, if applicable. [Default is 0.8] [Host only]
- `BuyPriceMultiplier` - multiplies the item buying price in the menu, if applicable. [Default is 1.2] [Host only]
- `ResearchTimeSeconds` - determines how long you need to press the research button (in seconds). [Default is 1.0] [Host only]
- `UseCustomUIColor` - determines if a custom color should be used instead of the mode's default one. [Default is false]
- `CustomUIColor` - RGB value for custom color.

## Commands

There are special commands available. Since Chat Commands is outdated and Default On Cheats is not a replacement for it!; I made my own little system of chat commands. Works in both singleplayer and multiplayer. Each command must start with `!rns_`:
- `!rns_help` - prints all commands and their description.
- `!rns_get_key` - prints active hotbar item "unique key". Can be used to tune price list and categories.
- `!rns_unlock_active` - completes the research of active hotbar item.
- `!rns_unlock_all` - completed the research of all items.
- `!rns_dump_progression` - saves progression of all players in .json format. [Host only]
- `!rns_load_progression` - loads progressions back from .json files and replaces whatever is in the mod now. [Host only]
