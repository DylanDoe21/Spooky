using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.EggEvent;

namespace Spooky.Content.Items.BossSummon
{
	public class StrangeCyst : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Item.ResearchUnlockCount = 3;
        }

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 34;
			Item.consumable = true;
			Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
			Item.maxStack = 9999;
		}

		public override bool? UseItem(Player player)
		{
			bool OrroboroDoesNotExist = !NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) && !NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) && !NPC.AnyNPCs(ModContent.NPCType<BoroHead>());

			if (!EggEventWorld.EggEventActive && OrroboroDoesNotExist)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.type == ModContent.NPCType<OrroboroEgg>() && npc.Distance(player.Center) <= 200f)
					{
						npc.ai[2] = 1;

						return true;
					}
				}
			}

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 5)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 10)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
	}
}