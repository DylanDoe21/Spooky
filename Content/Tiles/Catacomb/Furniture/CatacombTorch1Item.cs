using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class CatacombTorch1Item : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.Torches[Type] = true;
			ItemID.Sets.WaterTorches[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;

			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<CatacombTorch1>());
            Item.width = 16;
			Item.height = 16;
			Item.holdStyle = 1;
			Item.flame = true;
			Item.noWet = true;
			Item.value = Item.buyPrice(copper: 40);
		}

		public override void HoldItem(Player player)
		{
			if (!player.wet)
			{
				if (Main.rand.NextBool(player.itemAnimation > 0 ? 7 : 30))
				{
					//Dust dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f) , player.itemLocation.Y - 14f * player.gravDir), 4, 4, HeldDustType, 0f, 0f, 100);

					//if (!Main.rand.NextBool(3))
					//{
						//dust.noGravity = true;
					//}

					//dust.velocity *= 0.3f;
					//dust.velocity.Y -= 1.5f;
					//dust.position = player.RotatedRelativePoint(dust.position);
				}

				Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
				
				float divide = 300f;
				Lighting.AddLight(position, new Vector3(166f / divide, 125f / divide, 217f / divide));
			}
		}

		public override void PostUpdate()
		{
			if (!Item.wet)
			{
				float divide = 300f;
				Lighting.AddLight(Item.Center, new Vector3(166f / divide, 125f / divide, 217f / divide));
			}
		}

		public override void AddRecipes()
        {
            CreateRecipe(3)
			.AddIngredient(ItemID.Torch, 3)
			.AddIngredient(ModContent.ItemType<CatacombBrick1Item>(), 1)
            .Register();
        }
	}
}