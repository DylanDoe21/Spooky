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
    public class BossAchievementRotGourd : ModAchievement
    {
        public override void SetStaticDefaults()
        {
			Achievement.SetCategory(AchievementCategory.Slayer);
            AddNPCKilledCondition(ModContent.NPCType<RotGourd>());
        }
    }

	public class BossAchievementSpookySpirit : ModAchievement
    {
        public override void SetStaticDefaults()
        {
			Achievement.SetCategory(AchievementCategory.Slayer);
            AddNPCKilledCondition(ModContent.NPCType<SpookySpirit>());
        }
    }

	public class BossAchievementMoco : ModAchievement
    {
        public override void SetStaticDefaults()
        {
			Achievement.SetCategory(AchievementCategory.Slayer);
            AddNPCKilledCondition(ModContent.NPCType<Moco>());
        }
    }

	public class BossAchievementDaffodil : ModAchievement
    {
        public override void SetStaticDefaults()
        {
			Achievement.SetCategory(AchievementCategory.Slayer);
            AddNPCKilledCondition(ModContent.NPCType<DaffodilEye>());
        }
    }

	public class BossAchievementOrroboro : ModAchievement
    {
		public CustomFlagCondition EitherOrroBoroDead { get; private set; }

        public override void SetStaticDefaults()
        {
			Achievement.SetCategory(AchievementCategory.Slayer);
            EitherOrroBoroDead = AddCondition();
        }
    }

	public class BossAchievementSpookFishron : ModAchievement
	{
		public override bool Hidden => true;

		public override void SetStaticDefaults()
		{
			Achievement.SetCategory(AchievementCategory.Slayer);
			AddNPCKilledCondition(ModContent.NPCType<SpookFishron>());
		}
	}

	public class BossAchievementSpookFishronIce : ModAchievement
    {
        public override bool Hidden => true;

		public CustomFlagCondition FrostMoonCondition { get; private set; }

		public override void SetStaticDefaults()
        {
			Achievement.SetCategory(AchievementCategory.Slayer);
			FrostMoonCondition = AddCondition();
		}
    }

	public class BossAchievementBigBone : ModAchievement
	{
		public override void SetStaticDefaults()
		{
			Achievement.SetCategory(AchievementCategory.Slayer);
			AddNPCKilledCondition(ModContent.NPCType<BigBone>());
		}
	}
}