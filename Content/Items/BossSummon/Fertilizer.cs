using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.BigBone;

namespace Spooky.Content.Items.BossSummon
{
    [LegacyName("Fertalizer")]
    public class Fertilizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
		
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.consumable = true;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 9999;
        }

        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<BigBone>()) && player.InModBiome(ModContent.GetInstance<Content.Biomes.CatacombBiome2>()))
            {
                return true;
            }

            return false;
        }
		
        public override void UseAnimation(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);

            foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.type == ModContent.NPCType<BigFlowerPot>() && npc.ai[1] <= 0)
				{
					if (npc.Distance(player.Center) <= 320f)
					{
						npc.ai[1] = 1;
						npc.netUpdate = true;
					}

                    break;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHusk, 2)
            .AddIngredient(ItemID.Bone, 25)
            .AddIngredient(ItemID.DirtBlock, 100)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}