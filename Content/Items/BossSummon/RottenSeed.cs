using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

using Spooky.Content.NPCs.Boss.RotGourd;

namespace Spooky.Content.Items.BossSummon
{
    public class RottenSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
		
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 32;
            Item.consumable = true;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 9999;
        }
		
        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<RotGourd>()) && player.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyBiome>()))
            {
                return true;
            }

            return false;
        }
		
        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);

            NPC.SpawnOnPlayer(Main.myPlayer, ModContent.NPCType<RotGourd>());
            
            return true;
        }
    }
}
