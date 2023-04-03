namespace EtrianLike.Main
{
    public enum GameView
    {
        BattleScene_BattleView,
        BattleScene_CommandView,
        BattleScene_TargetView,
        ConversationScene_ConversationView,
        ConversationScene_ConversationView2,
        ConversationScene_ConversationView3,
        ConversationScene_SelectionView,
        CreditsScene_CreditsView,
        MapScene_MapView,
        MapScene_MenuView,
        ShopScene_ConstructView,
        ShopScene_HeroNameView,
        StatusScene_AbilitiesView,
        StatusScene_EquipmentView,
        StatusScene_ItemView,
        StatusScene_PartyView,
        StatusScene_StatusView,
        StatusScene_SwapEquipView,
        StatusScene_SystemView,
        TitleScene_SettingsView,
        TitleScene_TitleView,

        None = -1
    }

    public enum GameSound
    {
        a_mainmenuconfirm,
        a_mainmenuselection,
        Back,
        Blip,
        block_dissapear,
        block_junk_break,
        block_match,
        block_swap,
        Bounce,
        Chest,
        clear,
        combo,
        Confirm,
        Construct,
        Counter,
        Cursor,
        damange_blue,
        damange_cyan,
        damange_generic,
        damange_green,
        damange_red,
        damange_yellow,
        dialogue_auto_scroll,
        Encounter,
        EnemyDeath,
        enemy_action,
        enemy_encounter,
        Error,
        Fireball,
        gameover,
        GetItem,
        Heal,
        JoinParty,
        Laser,
        menu_cursor_change,
        menu_select,
        Miss,
        move_selection_cursor,
        Pickup,
        Rest,
        Selection,
        shink,
        Slash,
        Talk,
        Thunder,
        tink,
        wall_bump,
        wall_enter,

        None = -1
    }

    public enum GameMusic
    {
        Battle,
        NewDestinations,
        SchoolDay,
        Selection,
        SMP_DUN,
        SMP_TNS,
        Title,

        None = -1
    }

    public enum GameData
    {
        AbilityData,
        ClassData,
        ConversationData,
        EncounterData,
        EnemyData,
        HeroData,
        ItemData,
        ShopData,

        None = -1
    }

    public enum GameShader
    {
        BattleEnemy,
        BattleIntro,
        BattlePlayer,
        ColorFade,
        DayNight,
        Default,
        HeatDistortion,
        Pinwheel,
        Portrait,
        Wall,
        WallPlus,

        None = -1
    }

    public enum GameSprite
    {
        Enter,
        Gamepad,
        Keyboard,
        MiniMap,
        YouAreHere,
        Actors_Android,
        Actors_ArcWelderDrone,
        Actors_Base,
        Actors_BattlerShadow,
        Actors_Beast,
        Actors_Blank,
        Actors_BlowtorchDrone,
        Actors_Ch_Actor1,
        Actors_Crab,
        Actors_DarkKnight,
        Actors_DeflectorDrone,
        Actors_DroneShadow,
        Actors_FutureChest,
        Actors_HumanF,
        Actors_Inventor,
        Actors_MutantF,
        Actors_Plant,
        Actors_RepairDrone,
        Actors_Scorpion,
        Actors_Sentinel,
        Actors_Snake,
        Actors_Specter,
        Actors_WarriorF,
        Actors_WarriorM,
        Background_Blank,
        Background_Cave,
        Background_Desert,
        Background_Splash,
        Background_Steel,
        Background_Title,
        Background_Trees,
        Battlers_WarriorF,
        Battlers_WarriorM,
        Enemies_ArcherF,
        Enemies_ArcherM,
        Enemies_Bee,
        Enemies_Behemoth,
        Enemies_Beholder,
        Enemies_Cockatrice,
        Enemies_Commando,
        Enemies_DarkKnight,
        Enemies_Demon,
        Enemies_Diety,
        Enemies_Dragon,
        Enemies_Drone,
        Enemies_EarthSpirit,
        Enemies_Fairy,
        Enemies_FighterF,
        Enemies_FighterM,
        Enemies_FireSpirit,
        Enemies_Ghost,
        Enemies_GiantCrab,
        Enemies_Goblin,
        Enemies_Golem,
        Enemies_Griffin,
        Enemies_Guard,
        Enemies_Harpy,
        Enemies_Hawk,
        Enemies_Hydra,
        Enemies_Medusa,
        Enemies_Minotaur,
        Enemies_NinjaF,
        Enemies_NinjaM,
        Enemies_Ogre,
        Enemies_Owl,
        Enemies_Phoenix,
        Enemies_PoisonFlower,
        Enemies_Robot,
        Enemies_Samurai,
        Enemies_Scorpion,
        Enemies_Security,
        Enemies_SeekerBot,
        Enemies_Shogun,
        Enemies_Skeleton,
        Enemies_Slime,
        Enemies_Snake,
        Enemies_Squid,
        Enemies_Succubus,
        Enemies_ThiefF,
        Enemies_ThiefM,
        Enemies_Turtle,
        Enemies_Vampire,
        Enemies_WaterSpirit,
        Enemies_WhiteWizF,
        Enemies_WhiteWizM,
        Enemies_WindSpirit,
        Enemies_Wisp,
        Enemies_WizardF,
        Enemies_WizardM,
        Enemies_Wolf,
        Particles_Bash,
        Particles_BlueHeal,
        Particles_DamageDigits,
        Particles_Electric,
        Particles_Flame,
        Particles_GreenHeal,
        Particles_Gunshot,
        Particles_Shock,
        Particles_Slash,
        Particles_Smoke,
        Particles_Sparks,
        Particles_Star,
        Portraits_Christine,
        Portraits_ChristineFace,
        Portraits_Jane,
        Portraits_JaneFace,
        Portraits_WarriorF,
        Portraits_WarriorM,
        Tiles_Tl_World_A1,
        Tiles_Tl_World_A2,
        Tiles_Tl_World_B,
        Walls_BlackboardClassroomWall,
        Walls_Blank,
        Walls_ClassroomFloor,
        Walls_ClassroomWall,
        Walls_DoorClassroomWall,
        Walls_DoorFoyerWall,
        Walls_DoubleDoorFoyerWall,
        Walls_Foundry_C_Base,
        Walls_Foundry_C_Lit,
        Walls_Foundry_D_Base,
        Walls_Foundry_D_BaseDark,
        Walls_Foundry_D_SPC,
        Walls_Foundry_F_Base,
        Walls_Foundry_F_Lit,
        Walls_Foundry_W_Base,
        Walls_Foundry_W_BaseFlat,
        Walls_Foundry_W_Dark,
        Walls_Foundry_W_DarkFlat,
        Walls_Foundry_W_DarkPipes,
        Walls_Foundry_W_DarkPipesB,
        Walls_FoyerFloor,
        Walls_LightCeiling,
        Walls_LockerFoyerWall,
        Walls_Office_C_Base,
        Walls_Office_C_Hole,
        Walls_Office_C_HoleB,
        Walls_Office_C_HoleC,
        Walls_Office_C_Light,
        Walls_Office_D_Base,
        Walls_Office_D_Overgrown,
        Walls_Office_D_SPC,
        Walls_Office_F_Base,
        Walls_Office_F_Petals,
        Walls_Office_F_PetalsB,
        Walls_Office_W_Base,
        Walls_Office_W_PL1,
        Walls_Office_W_W1,
        Walls_Office_W_W2,
        Walls_PlainCeiling,
        Walls_PlainFoyerWall,
        Walls_WindowClassroomWall,
        Walls_WindowFoyerWall,
        Widgets_BattleFrame,
        Widgets_BattleWindow,
        Widgets_Blank,
        Widgets_ClassicFrame,
        Widgets_ClassicWindow,
        Widgets_LabelGlow,
        Widgets_MagicFrame,
        Widgets_MagicFrameSelected,
        Widgets_MagicLabel,
        Widgets_MagicSelected,
        Widgets_MagicWindow,
        Widgets_SelectedFrame,
        Widgets_TechFrame,
        Widgets_TechFrameSelected,
        Widgets_TechLabel,
        Widgets_TechSelected,
        Widgets_TechWindow,
        Background_Canyon_Canyon0,
        Background_Canyon_Canyon1,
        Widgets_Buttons_ClearPanel,
        Widgets_Buttons_GamePanel,
        Widgets_Buttons_GamePanelOpaque,
        Widgets_Buttons_GamePanelSelected,
        Widgets_Buttons_Panel,
        Widgets_Buttons_SelectedPanel,
        Widgets_Buttons_Technology,
        Widgets_Gauges_TechGauge,
        Widgets_Gauges_TechGaugeBackground,
        Widgets_Gauges_TechGaugeBar,
        Widgets_Gauges_TechSlider,
        Widgets_Gauges_ui_healthBar,
        Widgets_Gauges_ui_healthBarBackground,
        Widgets_Gauges_ui_healthBarInside,
        Widgets_Gauges_ui_thumbSlider,
        Widgets_Icons_Armor,
        Widgets_Icons_Axe,
        Widgets_Icons_Blank,
        Widgets_Icons_Chest,
        Widgets_Icons_Defend,
        Widgets_Icons_Delay,
        Widgets_Icons_Fire,
        Widgets_Icons_Flee,
        Widgets_Icons_Greatsword,
        Widgets_Icons_Gun,
        Widgets_Icons_Heal,
        Widgets_Icons_Heart,
        Widgets_Icons_Lightning,
        Widgets_Icons_Shield,
        Widgets_Icons_Staff,
        Widgets_Icons_Sword,
        Widgets_Images_Proceed,
        Widgets_Images_Settings,
        Widgets_Textplate_ClearPanel,
        Widgets_Textplate_Panel,
        Widgets_Windows_ClearPanel,
        Widgets_Windows_GamePanel,
        Widgets_Windows_GamePanelOpaque,
        Widgets_Windows_Panel,

        None = -1
    }

    public enum GameMap
    {
        DarkLibrary,
        DarkSchool,
        Foundry,
        Office,
        Overworld,
        School,
        SchoolHall,
        SchoolOrigin,
        Walls,
        Tilesets_Tl_World_A1,
        Tilesets_Tl_World_A2,
        Tilesets_Tl_World_B,

        None = -1
    }

}
