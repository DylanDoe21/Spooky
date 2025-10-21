using Terraria;
using Terraria.ModLoader;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.Ocean;

namespace Spooky.Content.Achievements
{
    public class MinibossAchievementBigDunk : ModAchievement
    {
        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Slayer);
            AddNPCKilledCondition(ModContent.NPCType<Dunkleosteus>());
        }
    }
}