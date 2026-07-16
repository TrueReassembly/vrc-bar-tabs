# VRChat Bar Tabs
A customizable bar tab system for VRChat

# Installation
1. Go to the latest release and download the unitypackage.
2. Drag and drop it into your editor.
3. Within the hierarchy, Open Tab Pool, Backboard, Food or Drinks (depending on what you wish to customize), Dropdown
4. In the Inspector, locate the Dropdown section and modify the options for the food and drinks you want. Note that the first option **must** be a "Please Select" option. Take note of the order you put them in.
5. Within the source files. Find ``DrinkHandler.cs`` and/or ``FoodHandler.cs``.
6. Under GetPrice(int), add a case for each item in your list where the returned value is the price of the item in the lowest denomination. Case 0 is always for the "Please select" option and your cases must be in order of the dropdown.
7. Under GetName(int), do the same thing but this time for the item names itself. You do not need to include "Please Select".
8. Repeat steps 3-7 for the other section if you wish.
9. Duplicate Backboard a few times for the maximum amount of menus you wish to add.
10. Go to the Tab Pool and add all Backboard objects to the Object Pool.
11. Move the SpawnTab button wherever it is most appropriate.
