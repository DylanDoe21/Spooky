﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<BigFlowerPot>()) 
				{
                    if (Main.npc[i].Distance(player.Center) > 320f)
                    {
                        return false;
                    }

                    if (Main.npc[i].ai[0] > 0) 
                    {
                        return false;
                    }
                }
            }

            if (!NPC.AnyNPCs(ModContent.NPCType<BigBone>()) && player.InModBiome(ModContent.GetInstance<Content.Biomes.CatacombBiome2>()))
            {
                return true;
            }

            return false;
        }
		
        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);

            for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<BigFlowerPot>()) 
				{
                    if (Main.npc[k].Distance(player.Center) <= 320f) 
                    {
                        Main.npc[k].ai[1] = 1;
                        Main.npc[k].netUpdate = true;
                    }
                }
            }

            return true;
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