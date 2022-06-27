using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Buffs;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class Candy2 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Candy");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 30;
			Item.maxStack = 1;
		}

		public override bool ItemSpace(Player player)
		{
			return true;
		}

		public override bool OnPickup(Player player)
		{
			player.AddBuff(ModContent.BuffType<CandyBuff2>(), 600);

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) 
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			Rectangle frame;

			if (Main.itemAnimations[Item.type] != null) 
			{
				frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
			}
			else 
			{
				frame = texture.Frame();
			}

			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
			Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f) 
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.25f) 
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(155, 155, 155, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f) 
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(155, 155, 155, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}