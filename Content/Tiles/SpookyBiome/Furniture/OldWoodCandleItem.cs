using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class OldWoodCandleItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.holdStyle = ItemHoldStyleID.HoldFront;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<OldWoodCandle>();
		}

		public override void HoldItem(Player player) 
		{
			if (Main.rand.Next(3) == 0) 
			{
				Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, 
				player.itemLocation.Y - 16f + player.velocity.Y), true);

				int newDust = Dust.NewDust(new Vector2(player.direction < 0 ? position.X : position.X - 10f, position.Y), 4, 4, DustID.Torch, 0f, 0f, 100, default, 1f);

				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].scale = 1.2f;
				Main.dust[newDust].velocity *= 0.3f;
				Main.dust[newDust].velocity.Y -= 0.001f;
			}
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 4)
			.AddIngredient(ItemID.Torch, 1)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}