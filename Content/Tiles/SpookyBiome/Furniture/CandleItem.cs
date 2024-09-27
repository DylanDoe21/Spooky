using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class CandleItem : ModItem
	{
		public override void SetDefaults() 
        {
			Item.DefaultToPlaceableTile(ModContent.TileType<Candle>());
            Item.width = 16;
			Item.height = 16;
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
            .AddIngredient(ItemID.Torch)
			.AddIngredient(ItemID.ClayBlock)
            .Register();
        }
	}
}