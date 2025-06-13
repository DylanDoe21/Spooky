using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
	public class SeaMine : ModNPC
	{
		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> HeatTexture;
		private static Asset<Texture2D> ChainTexture;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5;
			NPC.damage = 200;
			NPC.defense = 0;
			NPC.width = 54;
			NPC.height = 54;
			NPC.npcSlots = 0.5f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.hide = true;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.aiStyle = -1;
		}

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
		}

		public void DrawBody(bool SpawnGore)
		{
			NPC Parent = Main.npc[(int)NPC.ai[2]];

			if (Parent.active && Parent.type == ModContent.NPCType<SeaMineBase>() && !SpawnGore)
			{
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/SeaMineChain");

				Vector2 ParentCenter = Parent.Center;
                
                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = NPC.Center;
                Vector2 VectorToNPC = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToNPC = VectorToNPC.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToNPC.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = VectorToNPC.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
					if (!SpawnGore)
					{
						Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

						Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
					}
					else
					{
						if (Main.netMode != NetmodeID.Server)
						{
							Gore.NewGore(NPC.GetSource_Death(), chainDrawPosition, NPC.velocity, ModContent.Find<ModGore>("Spooky/SeaMineChainGore" + Main.rand.Next(1, 3)).Type);
						}
					}

					chainDrawPosition += unitVectorToNPC * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawBody(false);

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			HeatTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/SeaMineHeatGlow");

			//draw aura
            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(75 - NPC.alpha, 75 - NPC.alpha, 75 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

                Vector2 circular = new Vector2(3f, 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(HeatTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/SeaMineGlow");

			//glowmask
            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

			//bomb explosion glow
			if (NPC.ai[0] > 0)
			{
				Main.EntitySpriteDraw(HeatTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(Color.White) * NPC.ai[0], NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
			}
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

		public override bool CanHitNPC(NPC target)
		{
			return false;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool CheckDead()
        {
			NPC.ai[3] = 1;
			NPC.life = 1;
            return false;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[2]];
			if (!Parent.active || Parent.type != ModContent.NPCType<SeaMineBase>())
			{
				NPC.active = false;
			}

			Vector2 GoTo = new Vector2(Parent.Center.X, Parent.Center.Y - NPC.ai[1]);

			if (NPC.Distance(GoTo) > 50f)
			{
				Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 2;
				NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
			}
			else
			{
				NPC.velocity *= 0.95f;
			}

			if (NPC.ai[3] > 0)
			{
				if (NPC.ai[0] < 1)
                {
					NPC.ai[0] += 0.01f;
				}
				else
				{
					SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Pitch = -0.5f }, NPC.Center);

					//spawn gores
					for (int numGores = 1; numGores <= 10; numGores++)
					{
						if (Main.netMode != NetmodeID.Server)
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), ModContent.Find<ModGore>("Spooky/SeaMineGore" + numGores).Type);
						}
					}

					//flame dusts
					for (int numDust = 0; numDust < 45; numDust++)
					{
						int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
						Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-7f, 8f);
						Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-6f, 7f);
						Main.dust[dustGore].noGravity = true;
					}

					//explosion smoke
					for (int numExplosion = 0; numExplosion < 5; numExplosion++)
					{
						int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, 0.45f);
						Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-1f, 2f);
						Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-1f, 2f);
						Main.dust[DustGore].noGravity = true;
					}

					foreach (Player player in Main.ActivePlayers)
					{
						if (!player.dead && player.Distance(NPC.Center) <= 150f)
						{
							player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " " + Language.GetTextValue("Mods.Spooky.DeathReasons.SeaMineExplosion")), NPC.damage + Main.rand.Next(-30, 30), 0);
						}
					}

					DrawBody(true);

					Parent.active = false;
					NPC.active = false;
				}
			}

			foreach (Player player in Main.ActivePlayers)
			{
				if (!player.dead && player.Hitbox.Intersects(NPC.Hitbox))
				{
					NPC.ai[3] = 1;

					float Aceleration = 0.1f;
                    if (NPC.Center.X < player.Center.X)
                    {
                        NPC.velocity.X -= Aceleration;
                    }
                    else
                    {
                        NPC.velocity.X += Aceleration;
                    }
                    if (NPC.Center.Y < player.Center.Y)
                    {
                        NPC.velocity.Y -= Aceleration;
                    }
                    else
                    {
                        NPC.velocity.Y += Aceleration;
                    }
				}
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
	}

	public class SeaMineBase : ModNPC
	{
		public override string Texture => "Spooky/Content/Projectiles/Blank";

		public override void SetStaticDefaults()
		{
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 2;
			NPC.height = 2;
			NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.alpha = 255;
			NPC.aiStyle = -1;
		}

		public override void AI()
		{
			if (NPC.ai[0] == 0)
			{
				int SeaMine = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SeaMine>(), ai1: Main.rand.Next(100, 200), ai2: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: SeaMine);
				}

				NPC.position.Y += 15;

				NPC.ai[0]++;

				NPC.netUpdate = true;
			}
		}
	}
}