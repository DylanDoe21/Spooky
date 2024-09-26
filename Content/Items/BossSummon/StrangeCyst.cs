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
			Item.maxStack = 9999;
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