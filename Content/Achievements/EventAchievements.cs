using Terraria;
using Terraria.ModLoader;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.RotGourd;
using Spooky.Content.NPCs.Boss.SpookFishron;
using Spooky.Content.NPCs.Boss.SpookySpirit;

namespace Spooky.Content.Achievements
{
    public class EventAchievementPandoraBox : ModAchievement
    {
        public CustomFlagCondition PandoraBoxCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Slayer);
            PandoraBoxCondition = AddCondition();
        }
    }

    public class EventAchievementEggEvent : ModAchievement
    {
        public CustomFlagCondition EggEventCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Slayer);
            EggEventCondition = AddCondition();
        }
    }

    public class EventAchievementSpore : ModAchievement
    {
        public CustomFlagCondition SporeCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            SporeCondition = AddCondition();
        }
    }

    public class EventAchievementSpiderWar : ModAchievement
    {
        public CustomFlagCondition SpiderWarCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Slayer);
            SpiderWarCondition = AddCondition();
        }
    }

    public class EventAchievementSpiderWarEnd : ModAchievement
    {
        public CustomFlagCondition SpiderWarEndCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Challenger);
            SpiderWarEndCondition = AddCondition();
        }
    }

    public class EventAchievementRaveyard : ModAchievement
    {
        public CustomFlagCondition RaveyardCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            RaveyardCondition = AddCondition();
        }
    }
}