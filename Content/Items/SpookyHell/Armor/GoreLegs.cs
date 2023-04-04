using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class GoreLegs : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Gore Monger's Robe");
			/* Tooltip.SetDefault("10% increased movement speed"
			+ "\n5% increased critical strike chance"
			+ "\nEnemies are more likely to target you"); */
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 26;
			Item.height = 16;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Generic) += 5;
			player.moveSpeed += 0.10f;
			player.aggro += 75;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 15)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 25)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
