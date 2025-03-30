using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class GoreLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 12;
			Item.width = 30;
			Item.height = 16;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 3);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Generic) += 5;
			player.moveSpeed += 0.20f;
			player.aggro += 75;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 15)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 18)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
