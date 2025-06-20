using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Relic
{
	public class DunkleosteusRelicItem : ModItem
	{
		public override string Texture => "Spooky/Content/Tiles/Relic/DunkleosteusRelicGoldItem";

		private Asset<Texture2D> Texture1;
		private Asset<Texture2D> Texture2;
		private Asset<Texture2D> Texture3;

		public override void SetDefaults() 
        {
			Item.DefaultToPlaceableTile(ModContent.TileType<DunkleosteusRelic>());
			Item.width = 32;
			Item.height = 42;
			Item.rare = ItemRarityID.Quest;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicCopperItem");
			Texture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicSilverItem");
			Texture3 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicGoldItem");

			Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Item[Item.type].Value.Width * 0.5f, Item.height * 0.5f);

			if (!Main.expertMode)
			{
				Main.spriteBatch.Draw(Texture1.Value, position, null, Color.White, 0f, drawOrigin, scale, SpriteEffects.None, 0f);
			}
			else if (Main.expertMode && !Main.masterMode)
			{
				Main.spriteBatch.Draw(Texture2.Value, position, null, Color.White, 0f, drawOrigin, scale, SpriteEffects.None, 0f);
			}
			else if (Main.masterMode)
			{
				Main.spriteBatch.Draw(Texture3.Value, position, null, Color.White, 0f, drawOrigin, scale, SpriteEffects.None, 0f);
			}

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicCopperItem");
			Texture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicSilverItem");
			Texture3 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicGoldItem");

			Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Item[Item.type].Value.Width * 0.5f, Item.height * 0.5f);

			if (!Main.expertMode)
			{
				Main.spriteBatch.Draw(Texture1.Value, Item.Center - Main.screenPosition, null, Color.White, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
			}
			if (Main.expertMode && !Main.masterMode)
			{
				Main.spriteBatch.Draw(Texture2.Value, Item.Center - Main.screenPosition, null, Color.White, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
			}
			if (Main.masterMode)
			{
				Main.spriteBatch.Draw(Texture3.Value, Item.Center - Main.screenPosition, null, Color.White, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
			}

			return false;
		}
	}
}