using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyBiome.Boss;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
	public class WarlockRobe : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Warlock's Robe");
			Tooltip.SetDefault("5% increased summon damage and movement speed"
			+ "\nIncreases your max minions by 1");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.05f;
			player.moveSpeed += 0.05f;
			player.maxMinions += 1;
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
