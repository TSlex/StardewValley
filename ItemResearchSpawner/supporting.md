[(← Back to readme)](README.md)

If you want to support this mod development, you can:

* [Help balancing categories](#balance-categories)
* [Help balancing individual item prices for the "Buy" mode]()
* [Suggest improvement on the forum]()
* [Endorse this mod and tell somebody about it :)]()

### [Tip] How to get an item "Unique Key"

Unique key is by far the best way to determine item, because some items does not have unique ID and unique Name   
Unique key is a combination of __\<item name\>:\<item id\>__   
Unique key was tested on every item, and from __2000+__ items __only 2__ was not unique (two Copper Pans :/)

---

Place item in research area, and you will get output in console like

```
[Item Research 'n' Spawn] Current item - name: Deluxe Speed-Gro, ID: 466, category: Usable
[Item Research 'n' Spawn] Unique key: Deluxe Speed-Gro:466
```

Where "_Deluxe Speed-Gro:466_" is unique key (case sensitive)

---
Open you mod save file located in    
.../Stardew Valley/Mods/ItemResearchSpawner/save/<farmer>_<farm>/progress.json

Find your item key by searching it (Ctrl+F in most editors)

### Balance categories

You can balance categories by changing needed for research count, base item price and change item categories

Open mod categories file located in .../Stardew Valley/Mods/ItemResearchSpawner/assets/categories-progress.json

File has the following syntax:

```json
[
  {
    "Label": "category i18n name (do not change this)",
    "When": { //used to determine which items belong to the category
      "ObjCategory": [ //item game-categories. See https://stardewvalleywiki.com/Modding:Object_data#Categories
        -19,
        ...
      ],
      "ItemId": [ //item IDs. Note: not every item has ID! See https://stardewids.com/
        "Object:286",
        ...
      ],
      "Class": [  //item classes. See Stardew Valley sourse code to understand this :/
        "StardewValley.Tools.Slingshot"
      ],
      "UniqueKey": [ //item Unique Keys. See: [Tip] How to get an item "Unique Key"
        "Qi Fruit:889",
        "Ancient Fruit:454"
      ],
    },
    "Except": { //used to determine which items do not belong to the category
      "ObjCategory": [],
      "ItemId": [],
      "Class": [],
      "UniqueKey": [],
    },
    "ResearchCount": 10, //item count for research completion. Note, "Buy mode" use "1" instead of this
    "BaseCost": 100 //item base cost for "Buy" mode in case item does not have own price
  },
  {}...
]
```

Please pay attention to commas (or validate via any json validator online)

To change research count, edit __\"ResearchCount\": \<number here\>__   
To change base price, edit __\"BaseCost\": \<number here\>__ (number must be non-negative)

To change which item belong to category please consider using [Unique Keys](#[Tip]-How-to-get-an-item-"Unique Key").
If you experienced enough you can use ObjCategory, ItemId, Class as well. However here is some note: this file is read top-down, so
the first category that privatise the item, will have it. If you want to add item to category below, you __should add it to 
"When" of new category and to "Except" in old category__.

### Balance individual item prices

### Suggest improvement

### Endorse this mod and tell somebody about it

:)