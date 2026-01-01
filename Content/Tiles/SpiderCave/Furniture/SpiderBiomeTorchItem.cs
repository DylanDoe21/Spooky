using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class SpiderBiomeTorchItem : ModItem
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
			Item.DefaultToPlaceableTile(ModContent.TileType<SpiderBiomeTorch>());
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
				Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
				
				float divide = 300f;
				Lighting.AddLight(position, new Vector3(255f / divide, 196f / divide, 0f / divide));
			}
		}

		public override void PostUpdate()
		{
			if (!Item.wet)
			{
				float divide = 300f;
				Lighting.AddLight(Item.Center, new Vector3(227f / divide, 171f / divide, 13f / divide));
			}
		}

		public override void AddRecipes()
        {
            CreateRecipe(3)
			.AddIngredient(ItemID.Torch, 3)
			.AddIngredient(ModContent.ItemType<WebBlockItem>(), 1)
            .Register();
        }
	}
}