using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.OldHunter;
using Spooky.Content.NPCs.Friendly;

namespace Spooky.Content.Items.BossSummon
{
	public class OldHunterSoul : ModItem
	{
		private Asset<Texture2D> GlowTexture;

		public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 54;
			Item.consumable = false;
			Item.noUseGraphic = true;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.maxStack = 9999;
        }

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Items/BossSummon/OldHunterSoulAura");

			frame = GlowTexture.Frame();

			Vector2 drawPos = position;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 1f;

			if (time >= 1f) 
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.2f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(GlowTexture.Value, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, Color.Cyan * 0.1f, 0, origin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.2f) 
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(GlowTexture.Value, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, Color.White * 0.1f, 0, origin, scale, SpriteEffects.None, 0);
			}

			return true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) 
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Items/BossSummon/OldHunterSoulAura");

			Rectangle frame;

			frame = GlowTexture.Frame();

			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
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

			for (float i = 0f; i < 1f; i += 0.2f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(GlowTexture.Value, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, Color.Cyan * 0.1f, rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.2f) 
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(GlowTexture.Value, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, Color.White * 0.1f, rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}

		public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<OldHunterBoss>()) && player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
            {
                return true;
            }

            return false;
        }
		
		public override bool? UseItem(Player player)
		{
            SoundEngine.PlaySound(SoundID.Roar, player.Center);

            foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.type == ModContent.NPCType<OldHunterDead>() && npc.ai[1] <= 0)
				{
					if (npc.Distance(player.Center) <= 150f)
					{
						npc.ai[1] = 1;
						npc.netUpdate = true;

						if (Main.netMode == NetmodeID.Server)
                        {
                           	NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                        }
					}

                    break;
                }
            }

            return true;
        }
	}
}
