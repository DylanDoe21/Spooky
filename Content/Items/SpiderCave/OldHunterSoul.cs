using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Items.SpiderCave
{
	public class OldHunterSoul : ModItem
	{
		private Asset<Texture2D> GlowTexture;

		public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 54;
            Item.rare = ItemRarityID.Quest;
        }

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Items/SpiderCave/OldHunterSoulAura");

			Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Item[Item.type].Value.Width * 0.5f, Item.height * 0.5f);

			for (int i = 0; i < 360; i += 90)
			{
				Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Lerp(Color.White, Color.Cyan, i / 30));

				Vector2 circular = new Vector2(Main.rand.NextFloat(0f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

				Main.spriteBatch.Draw(GlowTexture.Value, position + circular, null, color, 0f, drawOrigin, scale, SpriteEffects.None, 0f);
			}

			return true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Items/SpiderCave/OldHunterSoulAura");

			Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Item[Item.type].Value.Width * 0.5f, Item.height * 0.5f);

			for (int i = 0; i < 360; i += 90)
			{
				Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Lerp(Color.White, Color.Cyan, i / 30));

				Vector2 circular = new Vector2(Main.rand.NextFloat(0f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

				Main.spriteBatch.Draw(GlowTexture.Value, Item.Center - Main.screenPosition + circular, null, color, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
			}

			return true;
		}
	}
}
