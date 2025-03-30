using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class GoreBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 15;
			Item.width = 36;
			Item.height = 20;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 5);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Generic) += 0.1f;
			player.GetCritChance(DamageClass.Generic) += 12;
			player.aggro += 100;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 18)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 22)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
