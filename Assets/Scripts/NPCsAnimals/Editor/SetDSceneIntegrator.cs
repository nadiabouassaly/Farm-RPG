using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public static class SetDSceneIntegrator
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Tools/Set D/Integrate NPCs And Animals")]
    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath);

        EnsurePlayerTag();
        EnsureItemRegistry();
        EnsureShopSellItems();
        DestroySceneObjects(
            "SetD_DialoguePanel",
            "SetD_FriendshipPanel",
            "SetD_AnimalInfoPanel",
            "SetD_ProductionPanel",
            "SetD_MinimapPanel",
            "SetD_TimeControls",
            "SetD_HelpOverlay");

        Canvas canvas = FindOrCreateCanvas();
        GameObject managers = FindOrCreate("SetDManagers");

        FriendshipManager friendshipManager = EnsureComponent<FriendshipManager>(managers);
        AnimalManager animalManager = EnsureComponent<AnimalManager>(managers);
        AnimalInfoPanel animalInfoPanel = EnsureComponent<AnimalInfoPanel>(managers);
        AnimalProductionUI productionUI = EnsureComponent<AnimalProductionUI>(managers);
        MinimapUI minimapUI = EnsureComponent<MinimapUI>(managers);
        HelpOverlayUI helpOverlayUI = EnsureComponent<HelpOverlayUI>(managers);

        GameObject dialoguePanel = BuildDialoguePanel(canvas.transform);
        GameObject friendshipPanel = BuildFriendshipPanel(canvas.transform, friendshipManager);
        GameObject animalInfoPanelObject = BuildAnimalInfoPanel(canvas.transform);
        GameObject productionPanel = BuildProductionPanel(canvas.transform, productionUI);
        GameObject minimapPanel = BuildMinimapPanel(canvas.transform);
        GameObject helpPanel = BuildHelpOverlay(canvas.transform);
        BuildTimeControls(canvas.transform, Object.FindFirstObjectByType<TimeSystem>());

        Transform waypointsRoot = EnsureRoot("SetD_Waypoints").transform;
        Transform home = EnsureWaypoint(waypointsRoot, "WP_Home", new Vector3(-8.8f, 43.0f, 0f));
        Transform shop = EnsureWaypoint(waypointsRoot, "WP_Shop", new Vector3(-2.2f, 41.7f, 0f));
        Transform farm = EnsureWaypoint(waypointsRoot, "WP_FarmBarn", new Vector3(12.2f, 41.0f, 0f));
        Transform park = EnsureWaypoint(waypointsRoot, "WP_Park", new Vector3(-18.4f, 47.1f, 0f));
        OpenBarnFenceGap();

        GameObject shopkeeper = EnsureNpc("NPC_ShopKeeper", "ShopKeeper", "Assets/Art/Sprites/npc_shopkeeper.png", "Assets/Art/Sprites/Characters/Human/WAITING/spikeyhair_waiting_strip9.png", new Vector3(-2.8f, 42.1f, 0f), dialoguePanel, shop, park, home);
        GameObject farmer = EnsureNpc("NPC_Farmer", "Farmer", "Assets/Art/Sprites/npc_farmer.png", "Assets/Art/Sprites/Characters/Human/WAITING/curlyhair_waiting_strip9.png", new Vector3(10.2f, 41.8f, 0f), dialoguePanel, farm, shop, home);
        GameObject villager = EnsureNpc("NPC_Villager", "Villager", "Assets/Art/Sprites/npc_villager.png", null, new Vector3(-13.0f, 45.4f, 0f), dialoguePanel, home, park, shop);

        WireDialogue(shopkeeper, "ShopKeeper",
            "Welcome to my shop. I sell seeds, tools, and animal feed. What can I help you with today?",
            new[] { "What seeds do you have?", "I need animal feed.", "Just looking around." },
            new[] { "I have wheat, carrot, potato, and seasonal seeds ready for the farm.", "The feed sacks by the counter work for chickens, cows, and sheep.", "Take your time. The shop is open during the day." },
            "Good to see you again. Need anything for the farm today?",
            new[] { "Any new stock?", "How is business?", "Any farming tip?" },
            new[] { "I restock seeds every new day, especially in season.", "Better now that the town is busier.", "Keep your animals fed before expecting good products." },
            "My favorite regular. I saved the best stock for you.",
            new[] { "What did you save?", "Any discount?", "Thanks!" },
            new[] { "Fresh premium feed. Your animals will love it.", "For you, I can always make a fair deal.", "Any time. A good farm keeps the whole town alive." });

        WireDialogue(farmer, "Farmer",
            "Morning. The crops need water and the animals need care. That is farm life.",
            new[] { "Need help?", "Nice crops.", "What animals do you raise?" },
            new[] { "Always. Start with feeding the animals in the barn.", "Thanks. Good soil and patience do most of the work.", "Chickens for eggs, cows for milk, and sheep for wool." },
            "The farm is holding together well this season.",
            new[] { "Any advice?", "How are the animals?", "What are you growing?" },
            new[] { "Water early and check every animal daily.", "They stay productive when they are fed and happy.", "Wheat, carrots, and anything the shop can keep in stock." },
            "Partner, you have become part of this place.",
            new[] { "Show me the barn.", "Can I help harvest?", "Thanks, Farmer." },
            new[] { "Come by. The animals are settled in there.", "When the crops are ready, every hand helps.", "You earned it." });

        WireDialogue(villager, "Villager",
            "Oh, hello. Welcome to the village.",
            new[] { "What is this place like?", "Where should I visit?", "Nice to meet you." },
            new[] { "Quiet, friendly, and usually smelling like fresh soil.", "The park is peaceful, and the shop is useful.", "Nice to meet you too." },
            "Hey neighbor. Beautiful day, right?",
            new[] { "What is happening?", "Do you like the farm?", "Want to walk?" },
            new[] { "Everyone is getting ready for the next harvest.", "The animals make the place feel alive.", "The park is my favorite route." },
            "There you are. I was hoping you would come by.",
            new[] { "What is new?", "Good to see you.", "Let's visit the park." },
            new[] { "The town is planning a small harvest table.", "Good to see you too.", "Yes, the flowers look best near sunset." });

        AddOrUpdateAnimal(GameObject.Find("cow"), "Bessie", AnimalData.AnimalType.Cow, new Vector3(13.55f, 39.65f, 0f), "Assets/Art/Sprites/animal_cow.png", "Assets/Art/Animations/Animal_Cow.controller");
        AddOrUpdateAnimal("Animal_Cow_Daisy", "Daisy", AnimalData.AnimalType.Cow, new Vector3(15.05f, 39.65f, 0f), "Assets/Art/Sprites/animal_cow.png", "Assets/Art/Animations/Animal_Cow.controller");
        AddOrUpdateAnimal("Animal_Chicken_Clucky", "Clucky", AnimalData.AnimalType.Chicken, new Vector3(12.05f, 41.15f, 0f), "Assets/Art/Sprites/animal_chicken.png", "Assets/Art/Animations/Animal_Chicken.controller");
        AddOrUpdateAnimal("Animal_Chicken_Nugget", "Nugget", AnimalData.AnimalType.Chicken, new Vector3(12.05f, 39.65f, 0f), "Assets/Art/Sprites/animal_chicken.png", "Assets/Art/Animations/Animal_Chicken.controller");
        AddOrUpdateAnimal("Animal_Sheep_Woolly", "Woolly", AnimalData.AnimalType.Sheep, new Vector3(13.55f, 41.15f, 0f), "Assets/Art/Sprites/animal_sheep.png", "Assets/Art/Animations/Animal_Sheep.controller");
        AddOrUpdateAnimal("Animal_Sheep_Cotton", "Cotton", AnimalData.AnimalType.Sheep, new Vector3(15.05f, 41.15f, 0f), "Assets/Art/Sprites/animal_sheep.png", "Assets/Art/Animations/Animal_Sheep.controller");

        WireAnimalManager(animalManager);
        WireAnimalInfoPanel(animalInfoPanel, animalInfoPanelObject);
        WireProductionUI(productionUI, productionPanel);
        WireFriendshipUI(EnsureComponent<FriendshipUI>(managers), friendshipPanel);
        WireMinimapUI(minimapUI, minimapPanel);
        WireHelpOverlay(helpOverlayUI, helpPanel);

        LabelArea("SetD_BarnLabel", "Barn / Coop", new Vector3(13.6f, 42.6f, 0f), new Color(1f, 0.82f, 0.25f));
        LabelArea("SetD_ParkLabel", "Park", park.position + new Vector3(0f, 0.8f, 0f), new Color(0.9f, 0.5f, 0.8f));

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Set D NPC/animal integration completed.");
    }

    private static void EnsurePlayerTag()
    {
        GameObject player = GameObject.Find("character") ?? GameObject.FindWithTag("Player");
        if (player == null) return;

        player.tag = "Player";
        EditorUtility.SetDirty(player);
    }

    private static void OpenBarnFenceGap()
    {
        Tilemap fence = null;
        foreach (Tilemap tilemap in Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None))
        {
            if (tilemap.name.ToLowerInvariant() == "fence")
            {
                fence = tilemap;
                break;
            }
        }

        if (fence == null) return;

        Vector3[] gapPoints =
        {
            new Vector3(10.85f, 41.05f, 0f),
            new Vector3(11.25f, 41.05f, 0f),
            new Vector3(11.65f, 41.05f, 0f),
            new Vector3(10.85f, 40.65f, 0f),
            new Vector3(11.25f, 40.65f, 0f),
            new Vector3(11.65f, 40.65f, 0f)
        };

        foreach (Vector3 point in gapPoints)
        {
            Vector3Int cell = fence.WorldToCell(point);
            fence.SetTile(cell, null);
            fence.SetTransformMatrix(cell, Matrix4x4.identity);
            fence.SetColor(cell, Color.white);
        }

        fence.RefreshAllTiles();

        TilemapCollider2D tilemapCollider = fence.GetComponent<TilemapCollider2D>();
        if (tilemapCollider != null)
        {
            tilemapCollider.enabled = false;
            tilemapCollider.enabled = true;
        }

        CompositeCollider2D compositeCollider = fence.GetComponent<CompositeCollider2D>();
        if (compositeCollider != null)
        {
            compositeCollider.enabled = false;
            compositeCollider.enabled = true;
        }

        EditorUtility.SetDirty(fence);
        EditorUtility.SetDirty(fence.gameObject);
    }

    private static void EnsureItemRegistry()
    {
        EnsureItemAsset(
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/QualityEgg.asset",
            "Quality Egg",
            "Large golden egg from a well-fed, happy chicken.",
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/Egg.asset");
        EnsureItemAsset(
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/QualityMilk.asset",
            "Quality Milk",
            "Rich milk from a well-fed, happy cow.",
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/Milk.asset");
        EnsureItemAsset(
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/QualityWool.asset",
            "Quality Wool",
            "Soft wool from a well-fed, happy sheep.",
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/Wool.asset");
        EnsureItemAsset(
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/PremiumEgg.asset",
            "Premium Egg",
            "Large golden egg from a very happy chicken.",
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/Egg.asset");
        EnsureItemAsset(
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/PremiumMilk.asset",
            "Premium Milk",
            "Rich milk from a very happy cow.",
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/Milk.asset");
        EnsureItemAsset(
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/PremiumWool.asset",
            "Premium Wool",
            "Soft wool from a very happy sheep.",
            "Assets/Scripts/FarmingTools/ItemAssets/Misc/Wool.asset");

        ItemRegistry registry = AssetDatabase.LoadAssetAtPath<ItemRegistry>("Assets/Scripts/FarmingTools/ItemRegistry.asset");
        if (registry == null) return;

        SerializedObject serializedRegistry = new SerializedObject(registry);
        SerializedProperty allItems = serializedRegistry.FindProperty("allItems");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/Egg.asset");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/Milk.asset");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/Wool.asset");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/QualityEgg.asset");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/QualityMilk.asset");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/QualityWool.asset");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/PremiumEgg.asset");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/PremiumMilk.asset");
        AddRegistryItem(allItems, "Assets/Scripts/FarmingTools/ItemAssets/Misc/PremiumWool.asset");
        serializedRegistry.ApplyModifiedProperties();
        EditorUtility.SetDirty(registry);
    }

    private static void EnsureItemAsset(string path, string itemName, string description, string templatePath)
    {
        ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemData>();
            AssetDatabase.CreateAsset(item, path);
        }

        ItemData template = AssetDatabase.LoadAssetAtPath<ItemData>(templatePath);
        item.itemName = itemName;
        item.itemType = ItemType.Misc;
        item.itemIcon = template != null ? template.itemIcon : null;
        item.maxStackSize = 64;
        item.description = description;
        item.toolType = ToolType.None;
        item.staminaCost = 0f;
        item.Category = "Animal Products";
        EditorUtility.SetDirty(item);
    }

    private static void EnsureShopSellItems()
    {
        ShopScript[] shops = Object.FindObjectsByType<ShopScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (ShopScript shop in shops)
        {
            SerializedObject so = new SerializedObject(shop);
            SerializedProperty inventory = so.FindProperty("ShopInventory");
            if (inventory == null) continue;

            EnsureShopItem(inventory, "Egg", 0, 8);
            EnsureShopItem(inventory, "Milk", 0, 18);
            EnsureShopItem(inventory, "Wool", 0, 24);
            EnsureShopItem(inventory, "Quality Egg", 0, 20);
            EnsureShopItem(inventory, "Quality Milk", 0, 46);
            EnsureShopItem(inventory, "Quality Wool", 0, 58);
            EnsureShopItem(inventory, "Premium Egg", 0, 20);
            EnsureShopItem(inventory, "Premium Milk", 0, 46);
            EnsureShopItem(inventory, "Premium Wool", 0, 58);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(shop);
        }
    }

    private static void EnsureShopItem(SerializedProperty inventory, string itemName, int buyPrice, int sellPrice)
    {
        SerializedProperty item = null;
        for (int i = 0; i < inventory.arraySize; i++)
        {
            SerializedProperty current = inventory.GetArrayElementAtIndex(i);
            if (current.FindPropertyRelative("Name").stringValue == itemName)
            {
                item = current;
                break;
            }
        }

        if (item == null)
        {
            inventory.arraySize++;
            item = inventory.GetArrayElementAtIndex(inventory.arraySize - 1);
            item.FindPropertyRelative("Quantity").intValue = 0;
        }

        item.FindPropertyRelative("Name").stringValue = itemName;
        item.FindPropertyRelative("Category").stringValue = "Animal Products";
        item.FindPropertyRelative("BuyPrice").intValue = buyPrice;
        item.FindPropertyRelative("SellPrice").intValue = sellPrice;
        SerializedProperty seasons = item.FindPropertyRelative("Season");
        seasons.arraySize = 4;
        seasons.GetArrayElementAtIndex(0).stringValue = "Spring";
        seasons.GetArrayElementAtIndex(1).stringValue = "Summer";
        seasons.GetArrayElementAtIndex(2).stringValue = "Fall";
        seasons.GetArrayElementAtIndex(3).stringValue = "Winter";
    }

    private static void AddRegistryItem(SerializedProperty allItems, string path)
    {
        ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if (item == null) return;

        for (int i = 0; i < allItems.arraySize; i++)
        {
            if (allItems.GetArrayElementAtIndex(i).objectReferenceValue == item) return;
        }

        allItems.arraySize++;
        allItems.GetArrayElementAtIndex(allItems.arraySize - 1).objectReferenceValue = item;
    }

    private static Canvas FindOrCreateCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null) return canvas;

        GameObject canvasObject = new GameObject("Canvas");
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static GameObject BuildDialoguePanel(Transform parent)
    {
        GameObject panel = RecreatePanel("SetD_DialoguePanel", parent, new Vector2(0.12f, 0f), new Vector2(0.88f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 112f), new Vector2(0f, 246f), new Color(0.03f, 0.05f, 0.06f, 0.96f));
        panel.SetActive(false);

        Text nameText = CreateText("NPC_Name_Text", panel.transform, "", 24, TextAnchor.MiddleCenter, new Vector2(0f, -10f), new Vector2(-28f, 34f), true);
        nameText.fontStyle = FontStyle.Bold;
        nameText.color = new Color(1f, 0.82f, 0.25f);

        Text npcText = CreateText("NPC_Text", panel.transform, "", 19, TextAnchor.UpperLeft, new Vector2(0f, -52f), new Vector2(-42f, 82f), true);
        npcText.horizontalOverflow = HorizontalWrapMode.Wrap;
        npcText.verticalOverflow = VerticalWrapMode.Overflow;

        GameObject choices = new GameObject("ChoicesContainer");
        choices.transform.SetParent(panel.transform, false);
        RectTransform choicesRect = choices.AddComponent<RectTransform>();
        choicesRect.anchorMin = new Vector2(0f, 0f);
        choicesRect.anchorMax = new Vector2(1f, 0f);
        choicesRect.pivot = new Vector2(0.5f, 0f);
        choicesRect.anchoredPosition = new Vector2(0f, 18f);
        choicesRect.sizeDelta = new Vector2(-46f, 128f);
        VerticalLayoutGroup layout = choices.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;

        return panel;
    }

    private static GameObject BuildFriendshipPanel(Transform parent, FriendshipManager friendshipManager)
    {
        GameObject panel = RecreatePanel("SetD_FriendshipPanel", parent, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-22f, -12f), new Vector2(360f, 260f), new Color(0.04f, 0.08f, 0.09f, 0.96f));
        panel.SetActive(false);
        Text title = CreateText("Title", panel.transform, "Friendships", 22, TextAnchor.MiddleCenter, new Vector2(0f, -12f), new Vector2(-24f, 34f), true);
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(1f, 0.82f, 0.25f);

        GameObject list = new GameObject("FriendshipList");
        list.transform.SetParent(panel.transform, false);
        RectTransform listRect = list.AddComponent<RectTransform>();
        listRect.anchorMin = new Vector2(0f, 0f);
        listRect.anchorMax = new Vector2(1f, 1f);
        listRect.offsetMin = new Vector2(14f, 16f);
        listRect.offsetMax = new Vector2(-14f, -58f);
        VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;

        friendshipManager.RegisterNPC("ShopKeeper");
        friendshipManager.RegisterNPC("Farmer");
        friendshipManager.RegisterNPC("Villager");
        return panel;
    }

    private static GameObject BuildAnimalInfoPanel(Transform parent)
    {
        GameObject panel = RecreatePanel("SetD_AnimalInfoPanel", parent, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-18f, 18f), new Vector2(280f, 184f), new Color(0.04f, 0.10f, 0.08f, 0.95f));
        panel.SetActive(false);
        Text name = CreateText("AnimalName", panel.transform, "Animal", 19, TextAnchor.MiddleCenter, new Vector2(0f, -10f), new Vector2(-18f, 28f), true);
        name.fontStyle = FontStyle.Bold;
        name.color = new Color(1f, 0.82f, 0.25f);
        CreateText("AnimalType", panel.transform, "Type", 14, TextAnchor.MiddleCenter, new Vector2(0f, -40f), new Vector2(-18f, 24f), true);
        CreateText("HungerLabel", panel.transform, "Hunger", 12, TextAnchor.MiddleLeft, new Vector2(16f, -72f), new Vector2(70f, 18f), false);
        Slider hunger = CreateSlider("HungerSlider", panel.transform, new Vector2(86f, -72f), new Vector2(145f, 18f), new Color(1f, 0.55f, 0.25f));
        CreateText("HappinessLabel", panel.transform, "Happy", 12, TextAnchor.MiddleLeft, new Vector2(16f, -103f), new Vector2(70f, 18f), false);
        Slider happiness = CreateSlider("HappinessSlider", panel.transform, new Vector2(86f, -103f), new Vector2(145f, 18f), new Color(0.45f, 0.9f, 0.4f));
        hunger.maxValue = 100;
        happiness.maxValue = 100;
        CreateText("ProductionText", panel.transform, "Not ready", 15, TextAnchor.MiddleCenter, new Vector2(0f, -142f), new Vector2(-18f, 28f), true);
        return panel;
    }

    private static GameObject BuildProductionPanel(Transform parent, AnimalProductionUI productionUI)
    {
        GameObject panel = RecreatePanel("SetD_ProductionPanel", parent, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-38f, -28f), new Vector2(390f, 402f), new Color(0.16f, 0.09f, 0.045f, 0.98f));
        panel.SetActive(false);
        AddOutline(panel, new Color(0.82f, 0.56f, 0.20f, 0.95f), new Vector2(2.5f, -2.5f));

        Text title = CreateText("Title", panel.transform, "Animal Products [G]", 22, TextAnchor.MiddleCenter, new Vector2(0f, -14f), new Vector2(-24f, 34f), true);
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(1f, 0.82f, 0.25f);

        Text subtitle = CreateText("Subtitle", panel.transform, "Feed with E. Happiness above 75% makes premium products.", 13, TextAnchor.MiddleCenter, new Vector2(0f, -47f), new Vector2(-24f, 22f), true);
        subtitle.color = new Color(0.86f, 0.93f, 0.88f);

        GameObject content = CreatePanelChild("ContentBox", panel.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -78f), new Vector2(-36f, 234f), new Color(0.05f, 0.12f, 0.10f, 0.98f));
        AddOutline(content, new Color(0.18f, 0.42f, 0.32f, 0.85f), new Vector2(1.5f, -1.5f));
        Text inventoryText = CreateText("InventoryText", content.transform, "No products collected yet.", 13, TextAnchor.UpperLeft, new Vector2(0f, -10f), new Vector2(-22f, 212f), true);
        inventoryText.horizontalOverflow = HorizontalWrapMode.Wrap;
        inventoryText.verticalOverflow = VerticalWrapMode.Overflow;

        Button collect = CreateButton("CollectAllButton", panel.transform, "Collect All", new Vector2(0f, -348f), new Vector2(216f, 40f));
        StyleButton(collect, new Color(0.20f, 0.44f, 0.28f, 1f), 15);
        UnityEventTools.AddPersistentListener(collect.onClick, productionUI.CollectAllProducts);

        Text closeHint = CreateText("CloseHint", panel.transform, "Press G to close", 12, TextAnchor.MiddleCenter, new Vector2(0f, -384f), new Vector2(-24f, 18f), true);
        closeHint.color = new Color(0.78f, 0.86f, 0.80f);
        return panel;
    }

    private static GameObject BuildMinimapPanel(Transform parent)
    {
        GameObject panel = RecreatePanel("SetD_MinimapPanel", parent, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-38f, -190f), new Vector2(306f, 252f), new Color(0.035f, 0.06f, 0.075f, 0.96f));
        panel.SetActive(false);
        AddOutline(panel, new Color(0.20f, 0.42f, 0.34f, 0.85f), new Vector2(2f, -2f));

        Text title = CreateText("MinimapTitle", panel.transform, "Farm Map [M]", 18, TextAnchor.MiddleCenter, new Vector2(0f, -8f), new Vector2(-18f, 28f), true);
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(1f, 0.82f, 0.25f);

        GameObject mapArea = new GameObject("MapArea");
        mapArea.transform.SetParent(panel.transform, false);
        RectTransform rect = mapArea.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = new Vector2(14f, 54f);
        rect.offsetMax = new Vector2(-14f, -42f);
        Image mapImage = mapArea.AddComponent<Image>();
        mapImage.color = new Color(0.06f, 0.13f, 0.12f, 0.94f);
        AddOutline(mapArea, new Color(0.15f, 0.33f, 0.27f, 0.9f), new Vector2(1.5f, -1.5f));

        CreateLegendItem(panel.transform, "You", new Color(0.22f, 0.55f, 1f), new Vector2(34f, 16f));
        CreateLegendItem(panel.transform, "NPC", new Color(1f, 0.82f, 0.22f), new Vector2(102f, 16f));
        CreateLegendItem(panel.transform, "Animal", new Color(0.3f, 0.85f, 0.3f), new Vector2(170f, 16f));
        CreateLegendItem(panel.transform, "Place", new Color(0.9f, 0.5f, 0.8f), new Vector2(254f, 16f));
        return panel;
    }

    private static GameObject BuildHelpOverlay(Transform parent)
    {
        GameObject overlay = RecreatePanel("SetD_HelpOverlay", parent, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0f, 0f, 0f, 0.68f));
        overlay.SetActive(true);

        GameObject card = CreatePanelChild("HelpCard", overlay.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(840f, 548f), new Color(0.035f, 0.060f, 0.070f, 0.98f));
        AddOutline(card, new Color(0.82f, 0.62f, 0.28f, 0.95f), new Vector2(3f, -3f));

        Text title = CreateText("Title", card.transform, "Farm RPG Controls", 30, TextAnchor.MiddleCenter, new Vector2(0f, -22f), new Vector2(-40f, 44f), true);
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(1f, 0.82f, 0.25f);

        Text subtitle = CreateText("Subtitle", card.transform, "Press H anytime to show or hide this guide.", 17, TextAnchor.MiddleCenter, new Vector2(0f, -66f), new Vector2(-40f, 28f), true);
        subtitle.color = new Color(0.82f, 0.90f, 0.86f);

        CreateHelpSection(card.transform, "Movement", new Vector2(42f, -108f), new[]
        {
            ("WASD", "Move around the farm"),
            ("E", "Interact, talk, enter doors, and feed animals"),
            ("Mouse", "Click UI buttons, shops, and dialogue choices"),
            ("H", "Show or hide this help screen")
        });

        CreateHelpSection(card.transform, "Farm", new Vector2(438f, -108f), new[]
        {
            ("1-9", "Select hotbar slot"),
            ("Tab", "Open or close inventory"),
            ("Tools", "Hoe, water, harvest, chop, and mine"),
            ("Stamina", "Advance day or sleep to recover")
        });

        CreateHelpSection(card.transform, "People & Animals", new Vector2(42f, -310f), new[]
        {
            ("F", "Open friendships"),
            ("P", "Pet a nearby animal"),
            ("G", "Open products and collect all"),
            ("M", "Open farm map")
        });

        CreateHelpSection(card.transform, "Time & Shops", new Vector2(438f, -310f), new[]
        {
            ("+1 Hour", "Advance schedules and shop hours"),
            ("Next Day", "Grow crops and create animal products"),
            ("Shop", "Buy and sell during open hours"),
            ("Save", "Use save and load menu buttons")
        });

        Button close = CreateButton("CloseButton", card.transform, "Close (H)", new Vector2(0f, -504f), new Vector2(156f, 38f));
        StyleButton(close, new Color(0.62f, 0.36f, 0.20f, 1f), 14);
        HelpOverlayUI help = Object.FindFirstObjectByType<HelpOverlayUI>();
        if (help != null) UnityEventTools.AddPersistentListener(close.onClick, help.Hide);

        return overlay;
    }

    private static GameObject BuildTimeControls(Transform parent, TimeSystem timeSystem)
    {
        GameObject panel = RecreatePanel("SetD_TimeControls", parent, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-38f, -98f), new Vector2(306f, 82f), new Color(0.32f, 0.18f, 0.10f, 0.98f));
        AddOutline(panel, new Color(0.86f, 0.58f, 0.22f, 0.95f), new Vector2(2.5f, -2.5f));

        GameObject inset = CreatePanelChild("Inset", panel.transform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-18f, -18f), new Color(0.48f, 0.29f, 0.16f, 0.96f));
        AddOutline(inset, new Color(0.18f, 0.08f, 0.04f, 0.75f), new Vector2(1f, -1f));

        Text title = CreateText("Title", panel.transform, "Time Controls", 14, TextAnchor.MiddleCenter, new Vector2(0f, -8f), new Vector2(-18f, 22f), true);
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(1f, 0.82f, 0.25f);

        Button hour = CreateButton("AdvanceHourButton", panel.transform, "+1 Hour", new Vector2(-72f, -40f), new Vector2(130f, 34f));
        Button day = CreateButton("AdvanceDayButton", panel.transform, "Next Day", new Vector2(72f, -40f), new Vector2(130f, 34f));
        StyleButton(hour, new Color(0.22f, 0.45f, 0.34f, 1f), 14);
        StyleButton(day, new Color(0.57f, 0.25f, 0.08f, 1f), 14);

        if (timeSystem != null)
        {
            UnityEventTools.AddPersistentListener(hour.onClick, timeSystem.AdvanceOneHour);
            UnityEventTools.AddPersistentListener(day.onClick, timeSystem.AdvanceOneDay);
        }

        return panel;
    }

    private static GameObject EnsureNpc(string name, string npcName, string spritePath, string accessoryPath, Vector3 position, GameObject dialoguePanel, Transform first, Transform second, Transform third)
    {
        GameObject npc = FindOrCreate(name);
        npc.transform.position = position;
        npc.transform.localScale = Vector3.one * 1.65f;

        SpriteRenderer renderer = EnsureComponent<SpriteRenderer>(npc);
        renderer.sprite = LoadFirstSprite(spritePath);
        renderer.sortingOrder = 110;
        EnsureNpcAccessory(npc.transform, accessoryPath);

        Rigidbody2D rb = EnsureComponent<Rigidbody2D>(npc);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        CircleCollider2D collider = EnsureComponent<CircleCollider2D>(npc);
        collider.radius = 0.34f;
        collider.isTrigger = false;

        NPCDialogue dialogue = EnsureComponent<NPCDialogue>(npc);
        NPCNameDisplay nameDisplay = EnsureComponent<NPCNameDisplay>(npc);
        NPCSchedule schedule = EnsureComponent<NPCSchedule>(npc);
        EnsurePrompt(npc.transform, "E", 0.95f, 0.11f);
        SetPrivateString(dialogue, "_npcName", npcName);
        SetPrivateString(nameDisplay, "_npcName", npcName);
        SetPrivateVector3(nameDisplay, "_offset", new Vector3(0f, 0.86f, 0f));

        WireNpcUi(dialogue, dialoguePanel);
        WireSchedule(schedule, new[] { (6, first), (12, second), (18, third) });
        EditorUtility.SetDirty(npc);
        return npc;
    }

    private static void EnsureNpcAccessory(Transform npc, string spritePath)
    {
        Transform old = npc.Find("SetD_Accessory");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        if (string.IsNullOrWhiteSpace(spritePath)) return;

        Sprite sprite = LoadFirstSprite(spritePath);
        if (sprite == null) return;

        GameObject accessory = new GameObject("SetD_Accessory");
        accessory.transform.SetParent(npc, false);
        accessory.transform.localPosition = new Vector3(0.01f, 0.30f, -0.01f);
        accessory.transform.localScale = Vector3.one * 4.35f;

        SpriteRenderer renderer = accessory.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 116;
    }

    private static void WireNpcUi(NPCDialogue dialogue, GameObject dialoguePanel)
    {
        SerializedObject so = new SerializedObject(dialogue);
        so.FindProperty("_dialoguePanel").objectReferenceValue = dialoguePanel;
        so.FindProperty("_npcNameText").objectReferenceValue = dialoguePanel.transform.Find("NPC_Name_Text").GetComponent<Text>();
        so.FindProperty("_npcText").objectReferenceValue = dialoguePanel.transform.Find("NPC_Text").GetComponent<Text>();
        so.FindProperty("_choicesContainer").objectReferenceValue = dialoguePanel.transform.Find("ChoicesContainer");
        so.FindProperty("_choiceButtonPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ChoiceButton.prefab");
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(dialogue);
    }

    private static void WireDialogue(GameObject npc, string npcName, string lowRoot, string[] lowChoices, string[] lowResponses, string midRoot, string[] midChoices, string[] midResponses, string highRoot, string[] highChoices, string[] highResponses)
    {
        NPCDialogue dialogue = npc.GetComponent<NPCDialogue>();
        SerializedObject so = new SerializedObject(dialogue);
        SetDialogueNode(so.FindProperty("_rootNode"), lowRoot, lowChoices, lowResponses);
        SerializedProperty npcData = so.FindProperty("_npcData");
        npcData.FindPropertyRelative("npcName").stringValue = npcName;
        npcData.FindPropertyRelative("friendshipPoints").intValue = 0;
        SetDialogueNode(npcData.FindPropertyRelative("lowFriendshipDialogue"), lowRoot, lowChoices, lowResponses);
        SetDialogueNode(npcData.FindPropertyRelative("midFriendshipDialogue"), midRoot, midChoices, midResponses);
        SetDialogueNode(npcData.FindPropertyRelative("highFriendshipDialogue"), highRoot, highChoices, highResponses);
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(dialogue);
    }

    private static void SetDialogueNode(SerializedProperty node, string rootText, string[] choiceTexts, string[] responseTexts)
    {
        node.FindPropertyRelative("npcText").stringValue = rootText;
        SerializedProperty choices = node.FindPropertyRelative("choices");
        int count = Mathf.Min(choiceTexts.Length, responseTexts.Length);
        choices.arraySize = count;
        for (int i = 0; i < count; i++)
        {
            SerializedProperty choice = choices.GetArrayElementAtIndex(i);
            choice.FindPropertyRelative("choiceText").stringValue = choiceTexts[i];
            choice.FindPropertyRelative("nextNode").managedReferenceValue = new DialogueNode
            {
                npcText = responseTexts[i],
                choices = new List<DialogueChoice>()
            };
        }
    }

    private static void AddOrUpdateAnimal(string objectName, string animalName, AnimalData.AnimalType type, Vector3 position, string spritePath, string controllerPath)
    {
        AddOrUpdateAnimal(GameObject.Find(objectName) ?? FindOrCreate(objectName), animalName, type, position, spritePath, controllerPath);
    }

    private static void AddOrUpdateAnimal(GameObject animal, string animalName, AnimalData.AnimalType type, Vector3 position, string spritePath, string controllerPath)
    {
        if (animal == null) return;

        animal.transform.position = position;
        float scale = type switch
        {
            AnimalData.AnimalType.Chicken => 0.82f,
            AnimalData.AnimalType.Sheep => 0.78f,
            _ => 0.92f
        };
        animal.transform.localScale = Vector3.one * scale;

        SpriteRenderer renderer = EnsureComponent<SpriteRenderer>(animal);
        renderer.sprite = LoadFirstSprite(spritePath);
        renderer.sortingOrder = 105;

        Animator animator = EnsureComponent<Animator>(animal);
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);

        Rigidbody2D rb = EnsureComponent<Rigidbody2D>(animal);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        CapsuleCollider2D collider = EnsureComponent<CapsuleCollider2D>(animal);
        collider.size = type == AnimalData.AnimalType.Chicken ? new Vector2(0.55f, 0.42f) : new Vector2(0.95f, 0.65f);
        collider.isTrigger = false;

        AnimalController controller = EnsureComponent<AnimalController>(animal);
        EnsureComponent<AnimalStateDisplay>(animal);
        EnsureComponent<InteractionFeedback>(animal);
        EnsurePrompt(animal.transform, "E Feed\nP Pet", type == AnimalData.AnimalType.Chicken ? 1.05f : 1.25f, 0.13f);

        SerializedObject so = new SerializedObject(controller);
        SerializedProperty data = so.FindProperty("_animalData");
        data.FindPropertyRelative("animalName").stringValue = animalName;
        data.FindPropertyRelative("animalType").enumValueIndex = (int)type;
        data.FindPropertyRelative("hunger").intValue = 50;
        data.FindPropertyRelative("happiness").intValue = 50;
        data.FindPropertyRelative("productionReady").boolValue = false;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(animal);
    }

    private static void WireAnimalManager(AnimalManager manager)
    {
        AnimalController[] animals = Object.FindObjectsByType<AnimalController>(FindObjectsSortMode.None);
        SerializedObject so = new SerializedObject(manager);
        SerializedProperty list = so.FindProperty("_animals");
        list.arraySize = animals.Length;
        for (int i = 0; i < animals.Length; i++)
        {
            list.GetArrayElementAtIndex(i).objectReferenceValue = animals[i];
        }
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(manager);
    }

    private static void WireAnimalInfoPanel(AnimalInfoPanel panelComponent, GameObject panel)
    {
        SerializedObject so = new SerializedObject(panelComponent);
        so.FindProperty("_panel").objectReferenceValue = panel;
        so.FindProperty("_animalNameText").objectReferenceValue = panel.transform.Find("AnimalName").GetComponent<Text>();
        so.FindProperty("_animalTypeText").objectReferenceValue = panel.transform.Find("AnimalType").GetComponent<Text>();
        so.FindProperty("_hungerSlider").objectReferenceValue = panel.transform.Find("HungerSlider").GetComponent<Slider>();
        so.FindProperty("_happinessSlider").objectReferenceValue = panel.transform.Find("HappinessSlider").GetComponent<Slider>();
        so.FindProperty("_productionText").objectReferenceValue = panel.transform.Find("ProductionText").GetComponent<Text>();
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(panelComponent);
    }

    private static void WireProductionUI(AnimalProductionUI productionUI, GameObject panel)
    {
        SerializedObject so = new SerializedObject(productionUI);
        so.FindProperty("_panel").objectReferenceValue = panel;
        so.FindProperty("_inventoryText").objectReferenceValue = panel.transform.Find("ContentBox/InventoryText").GetComponent<Text>();
        so.FindProperty("_itemRegistry").objectReferenceValue = AssetDatabase.LoadAssetAtPath<ItemRegistry>("Assets/Scripts/FarmingTools/ItemRegistry.asset");
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(productionUI);
    }

    private static void WireFriendshipUI(FriendshipUI friendshipUI, GameObject panel)
    {
        SerializedObject so = new SerializedObject(friendshipUI);
        so.FindProperty("_panel").objectReferenceValue = panel;
        so.FindProperty("_listContainer").objectReferenceValue = panel.transform.Find("FriendshipList");
        so.FindProperty("_friendshipRowPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FriendshipRow.prefab");
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(friendshipUI);
    }

    private static void WireMinimapUI(MinimapUI minimapUI, GameObject panel)
    {
        SerializedObject so = new SerializedObject(minimapUI);
        so.FindProperty("_panel").objectReferenceValue = panel;
        so.FindProperty("_mapArea").objectReferenceValue = panel.transform.Find("MapArea").GetComponent<RectTransform>();
        so.FindProperty("_worldCenter").vector2Value = new Vector2(0f, 42f);
        so.FindProperty("_worldRangeX").floatValue = 42f;
        so.FindProperty("_worldRangeY").floatValue = 24f;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(minimapUI);
    }

    private static void WireHelpOverlay(HelpOverlayUI helpOverlayUI, GameObject panel)
    {
        SerializedObject so = new SerializedObject(helpOverlayUI);
        so.FindProperty("_panel").objectReferenceValue = panel;
        so.FindProperty("_showOnStart").boolValue = true;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(helpOverlayUI);
    }

    private static void WireSchedule(NPCSchedule schedule, (int hour, Transform location)[] entries)
    {
        SerializedObject so = new SerializedObject(schedule);
        SerializedProperty list = so.FindProperty("_schedule");
        list.arraySize = entries.Length;
        for (int i = 0; i < entries.Length; i++)
        {
            SerializedProperty entry = list.GetArrayElementAtIndex(i);
            entry.FindPropertyRelative("hour").intValue = entries[i].hour;
            entry.FindPropertyRelative("location").objectReferenceValue = entries[i].location;
        }
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(schedule);
    }

    private static void EnsurePrompt(Transform parent, string label, float yOffset, float characterSize)
    {
        Transform existing = parent.Find("press tp enter_0");
        GameObject prompt = existing != null ? existing.gameObject : new GameObject("press tp enter_0");
        prompt.transform.SetParent(parent, false);
        prompt.transform.localPosition = new Vector3(0f, yOffset, 0f);
        TextMesh text = EnsureComponent<TextMesh>(prompt);
        text.text = label;
        text.characterSize = characterSize;
        text.fontSize = 32;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = Color.white;
        MeshRenderer renderer = prompt.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.sortingOrder = 160;
        prompt.SetActive(false);
    }

    private static Transform EnsureWaypoint(Transform root, string name, Vector3 position)
    {
        Transform waypoint = root.Find(name);
        if (waypoint == null)
        {
            GameObject go = new GameObject(name);
            waypoint = go.transform;
            waypoint.SetParent(root, false);
        }

        waypoint.position = position;
        return waypoint;
    }

    private static void LabelArea(string name, string text, Vector3 position, Color color)
    {
        GameObject label = FindOrCreate(name);
        label.transform.position = position;
        TextMesh mesh = EnsureComponent<TextMesh>(label);
        mesh.text = text;
        mesh.characterSize = 0.11f;
        mesh.fontSize = 28;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = color;
        MeshRenderer renderer = label.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.sortingOrder = 150;
    }

    private static GameObject RecreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, Color color)
    {
        Transform old = parent.Find(name);
        if (old != null) Object.DestroyImmediate(old.gameObject);

        GameObject panel = new GameObject(name);
        panel.layer = 5;
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>().color = color;
        return panel;
    }

    private static GameObject CreatePanelChild(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.layer = 5;
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>().color = color;
        return panel;
    }

    private static void AddOutline(GameObject target, Color color, Vector2 distance)
    {
        UnityEngine.UI.Outline outline = target.GetComponent<UnityEngine.UI.Outline>();
        if (outline == null) outline = target.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = color;
        outline.effectDistance = distance;
    }

    private static void StyleButton(Button button, Color color, int fontSize)
    {
        if (button == null) return;

        if (button.targetGraphic is Image image)
        {
            image.color = color;
            AddOutline(image.gameObject, new Color(0.08f, 0.04f, 0.02f, 0.85f), new Vector2(1.5f, -1.5f));
        }

        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.16f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        Text label = button.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.fontSize = fontSize;
            label.fontStyle = FontStyle.Bold;
            label.color = Color.white;
        }
    }

    private static void CreateLegendItem(Transform parent, string label, Color color, Vector2 position)
    {
        GameObject root = new GameObject("Legend_" + label);
        root.layer = 5;
        root.transform.SetParent(parent, false);
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(62f, 22f);

        GameObject swatch = CreatePanelChild("Swatch", root.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0f), new Vector2(12f, 12f), color);
        AddOutline(swatch, new Color(0f, 0f, 0f, 0.65f), new Vector2(1f, -1f));

        Text text = CreateText("Label", root.transform, label, 11, TextAnchor.MiddleLeft, new Vector2(18f, 0f), new Vector2(44f, 18f), false);
        text.color = new Color(0.86f, 0.94f, 0.90f);
    }

    private static void CreateHelpSection(Transform parent, string title, Vector2 position, (string key, string action)[] rows)
    {
        const float rowHeight = 34f;
        GameObject section = CreatePanelChild("Help_" + title, parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, new Vector2(360f, rows.Length * rowHeight + 48f), new Color(0.08f, 0.12f, 0.13f, 0.88f));
        AddOutline(section, new Color(0.13f, 0.28f, 0.24f, 0.8f), new Vector2(1.5f, -1.5f));

        Text header = CreateText("Header", section.transform, title, 18, TextAnchor.MiddleLeft, new Vector2(18f, -12f), new Vector2(322f, 28f), false);
        header.fontStyle = FontStyle.Bold;
        header.color = new Color(1f, 0.82f, 0.25f);

        for (int i = 0; i < rows.Length; i++)
        {
            float y = -48f - i * rowHeight;
            GameObject key = CreatePanelChild("Key_" + rows[i].key, section.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, y), new Vector2(84f, 24f), new Color(0.18f, 0.36f, 0.32f, 1f));
            Text keyText = CreateText("Text", key.transform, rows[i].key, 13, TextAnchor.MiddleCenter, Vector2.zero, Vector2.zero, true);
            keyText.fontStyle = FontStyle.Bold;
            RectTransform keyRect = keyText.GetComponent<RectTransform>();
            keyRect.anchorMin = Vector2.zero;
            keyRect.anchorMax = Vector2.one;
            keyRect.pivot = new Vector2(0.5f, 0.5f);
            keyRect.anchoredPosition = Vector2.zero;
            keyRect.sizeDelta = Vector2.zero;

            Text actionText = CreateText("Action_" + i, section.transform, rows[i].action, 13, TextAnchor.MiddleLeft, new Vector2(118f, y), new Vector2(222f, 26f), false);
            actionText.color = new Color(0.88f, 0.95f, 0.91f);
            actionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        }
    }

    private static Text CreateText(string name, Transform parent, string value, int size, TextAnchor anchor, Vector2 position, Vector2 dimensions, bool stretchWidth)
    {
        GameObject go = new GameObject(name);
        go.layer = 5;
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = stretchWidth ? new Vector2(0f, 1f) : new Vector2(0f, 1f);
        rect.anchorMax = stretchWidth ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
        rect.pivot = new Vector2(stretchWidth ? 0.5f : 0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = dimensions;
        go.AddComponent<CanvasRenderer>();
        Text text = go.AddComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = anchor;
        return text;
    }

    private static Slider CreateSlider(string name, Transform parent, Vector2 position, Vector2 size, Color fillColor)
    {
        GameObject root = new GameObject(name);
        root.layer = 5;
        root.transform.SetParent(parent, false);
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        GameObject bg = new GameObject("Background");
        bg.layer = 5;
        bg.transform.SetParent(root.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bg.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.45f);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.layer = 5;
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(2f, 2f);
        fillAreaRect.offsetMax = new Vector2(-2f, -2f);

        GameObject fill = new GameObject("Fill");
        fill.layer = 5;
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fill.AddComponent<Image>().color = fillColor;

        Slider slider = root.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 50;
        slider.fillRect = fillRect;
        slider.targetGraphic = bg.GetComponent<Image>();
        return slider;
    }

    private static Button CreateButton(string name, Transform parent, string label, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.layer = 5;
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.20f, 0.45f, 0.32f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        Text text = CreateText("Text", buttonObject.transform, label, 14, TextAnchor.MiddleCenter, Vector2.zero, Vector2.zero, true);
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = Vector2.zero;
        return button;
    }

    private static Sprite LoadFirstSprite(string path)
    {
        foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if (asset is Sprite sprite) return sprite;
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static T EnsureComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        return component != null ? component : go.AddComponent<T>();
    }

    private static void SetPrivateString(Object target, string propertyName, string value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(propertyName);
        if (prop != null) prop.stringValue = value;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private static void SetPrivateVector3(Object target, string propertyName, Vector3 value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(propertyName);
        if (prop != null) prop.vector3Value = value;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private static GameObject FindOrCreate(string name)
    {
        GameObject existing = GameObject.Find(name);
        return existing != null ? existing : new GameObject(name);
    }

    private static void DestroySceneObjects(params string[] names)
    {
        foreach (string name in names)
        {
            GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (GameObject obj in objects)
            {
                if (obj == null) continue;
                if (obj.name == name)
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }
    }

    private static GameObject EnsureRoot(string name)
    {
        GameObject root = FindOrCreate(name);
        root.transform.SetParent(null);
        return root;
    }
}
