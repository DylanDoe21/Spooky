/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyBiome.Boss;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SpookyBody : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Spooky Horseman's Chestmail");
			Tooltip.SetDefault("3% increased damage");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 30;
			Item.height = 24;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Generic) += 0.03f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 15)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 30)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
*/