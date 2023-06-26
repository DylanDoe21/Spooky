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
			Item.width = 30;
			Item.height = 20;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Generic) += 0.08f;
			player.GetCritChance(DamageClass.Generic) += 8;
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
