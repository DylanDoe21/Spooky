using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class SnotArrow : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 14;
			Item.height = 40;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 3;
			Item.value = Item.buyPrice(silver: 5);
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<SnotArrowProj>();
			Item.shootSpeed = 4.5f;
			Item.ammo = AmmoID.Arrow;
		}

		public override void AddRecipes()
        {
            CreateRecipe(50)
			.AddIngredient(ItemID.WoodenArrow, 50)
			.AddIngredient(ModContent.ItemType<SnotGlob>())
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}