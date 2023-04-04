using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class EyeLegs : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Eye Leggings");
			// Tooltip.SetDefault("5% increased movement speed");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 22;
			Item.height = 16;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.moveSpeed += 0.05f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<EyeBlockItem>(), 32)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 65)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
