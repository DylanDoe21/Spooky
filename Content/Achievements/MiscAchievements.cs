using Terraria;
using Terraria.ModLoader;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.Ocean;

namespace Spooky.Content.Achievements
{
    public class MiscAchievementTurkeyTamed : ModAchievement
    {
        public CustomFlagCondition TamedTurkeyCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            TamedTurkeyCondition = AddCondition();
        }
    }

    public class MiscAchievementGourdCarve : ModAchievement
    {
        public CustomFlagCondition CarvedRottenGourdCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            CarvedRottenGourdCondition = AddCondition();
        }
    }

    public class MiscAchievementKrampusHired : ModAchievement
    {
        public CustomFlagCondition KrampusHiredCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            KrampusHiredCondition = AddCondition();
        }
    }

    public class MiscAchievementNoseCultist : ModAchievement
    {
        public CustomFlagCondition NoseCultistCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            NoseCultistCondition = AddCondition();
        }
    }

    public class MiscAchievementSentientItem : ModAchievement
    {
        public CustomFlagCondition SentientCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            SentientCondition = AddCondition();
        }
    }

    public class MiscAchievementBigDunkEat : ModAchievement
    {
        public CustomFlagCondition BigDunkEatCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            BigDunkEatCondition = AddCondition();
        }
    }

    public class MiscAchievementLittleEyeQuest : ModAchievement
    {
        public CustomFlagCondition LittleEyeQuestCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            LittleEyeQuestCondition = AddCondition();
        }
    }

    public class MiscAchievementBloomHarvest : ModAchievement
    {
        public CustomFlagCondition BloomHarvestCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Explorer);
            BloomHarvestCondition = AddCondition();
        }
    }
}