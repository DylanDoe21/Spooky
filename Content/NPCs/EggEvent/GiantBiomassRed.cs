using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class GiantBiomassRed : ModNPC
    {
		float GlowAlpha = 0;
		float addedStretch = 0f;
		float stretchRecoil = 0f;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> GlowTexture;

		public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/BiomassExplode1", SoundType.Sound);

		public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SetDefaults()
        {
            NPC.lifeMax = 25;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 20;
            NPC.height = 20;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			NPC.hide = true;
			NPC.aiStyle = -1;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/GiantBiomassGlow");

			float stretch = 0f;
			stretch = Math.Abs(stretch) - addedStretch;

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, scaleStretch, SpriteEffects.None, 0);
			spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White * GlowAlpha), NPC.rotation, NPC.frame.Size() / 2, scaleStretch, SpriteEffects.None, 0);

			return false;
		}

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
		}

		public override void AI()
		{
			NPC.rotation += 0.02f * (float)NPC.direction + (NPC.velocity.X / 40);

			addedStretch = -stretchRecoil;

			if (NPC.ai[1] == 0)
			{
				NPC.velocity.Y = -3;

				NPC.rotation = 1.57f;

				NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
				NPC.Center += Main.rand.NextVector2Square(-5, 5);

				if (!NPCGlobalHelper.IsColliding(NPC))
				{
					SoundEngine.PlaySound(SoundID.Dig with { Pitch = -1.2f }, NPC.Center);

					GlowAlpha = 1;

					//spawn blood splatter
					for (int i = 0; i < 3; i++)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-8, 9), Main.rand.Next(-8, -4), ModContent.ProjectileType<RedSplatter>(), 0, 0);
						}
					}

					NPC.ai[1]++;
					NPC.netUpdate = true;
				}
			}
			else
			{
				NPC.ai[0]++;
				if (NPC.ai[0] == 1)
				{
					NPC.velocity = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, -5f));
					NPC.netUpdate = true;
				}

				if (NPC.ai[0] >= 70)
				{
					NPC.velocity *= 0.95f;

					if (NPC.ai[2] == 5)
					{
						NPC.velocity.Y = NPC.velocity.Y + 0.75f;
					}

					NPC.ai[3]++;
					if (NPC.ai[3] % 5 == 0)
					{
						stretchRecoil = 0.25f;
					}
					else
					{
						if (stretchRecoil > 0.1f)
						{
							stretchRecoil -= 0.125f;
						}
					}
				}

				if (NPC.ai[2] == 5)
				{
					if (NPC.velocity.Y > 0 && NPCGlobalHelper.IsColliding(NPC))
					{
						OnKill();
						NPC.active = false;
						NPC.netUpdate = true;
					}
				}
				else
				{
					if (NPC.ai[0] >= 150)
					{
						OnKill();
						NPC.active = false;
						NPC.netUpdate = true;
					}
				}
			}
		}

		public void SpawnNPC(int Type)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int NewEnemy = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, Type);

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: NewEnemy);
				}
			}
		}

		public override void OnKill()
		{
			SoundEngine.PlaySound(ExplosionSound, NPC.Center);

			//spawn blood splatter
			for (int i = 0; i < 8; i++)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-8, 9), Main.rand.Next(-8, 5), ModContent.ProjectileType<RedSplatter>(), 0, 0);
				}
			}

			//spawn blood explosion clouds
			for (int numExplosion = 0; numExplosion < 8; numExplosion++)
			{
				int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(1f, 1.2f));
				Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
				Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-3f, 0f);
				Main.dust[DustGore].noGravity = true;

				if (Main.rand.NextBool(2))
				{
					Main.dust[DustGore].scale = 0.5f;
					Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}

			//spawn vanilla blood dust
			for (int numDust = 0; numDust < 25; numDust++)
			{
				int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
				Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
				Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
				Main.dust[newDust].scale *= Main.rand.NextFloat(1.8f, 2.5f);
				Main.dust[newDust].noGravity = true;

				if (Main.rand.NextBool(2))
				{
					Main.dust[newDust].scale = 0.5f;
					Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}

			//spawn enemy depending on what ai[2] is set to
			switch ((int)NPC.ai[2])
			{
				case 0:
				{
					SpawnNPC(ModContent.NPCType<HoppingHeart>());
					break;
				}

				case 1:
				{
					SpawnNPC(ModContent.NPCType<TongueBiter>());
					break;
				}

				case 2:
				{
					SpawnNPC(ModContent.NPCType<ExplodingAppendix>());
					break;
				}

				case 3:
				{
					SpawnNPC(ModContent.NPCType<CoughLungs>());
					break;
				}

				case 4:
				{
					SpawnNPC(ModContent.NPCType<HoverBrain>());
					break;
				}

				case 5:
				{
					SpawnNPC(ModContent.NPCType<FleshBolster>());
					break;
				}
			}
		}
	}
}