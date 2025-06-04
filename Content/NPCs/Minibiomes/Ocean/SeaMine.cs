using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
	public class SeaMine : ModNPC
	{
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
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToNPC * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
			}

			if (SpawnGore)
			{
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawBody(false);

			return true;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/SeaMineGlow");
			HeatTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/SeaMineHeatGlow");

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

			if (NPC.ai[0] > 0)
			{
				Main.EntitySpriteDraw(HeatTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
				NPC.frame, NPC.GetAlpha(Color.White) * NPC.ai[0], NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
			}
        }

		public override bool CheckActive()
		{
			return false;
		}

		public override bool CheckDead()
        {
			NPC.ai[3] = 1;
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

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				NPC Parent = Main.npc[(int)NPC.ai[2]];

				Parent.active = false;

				DrawBody(true);
			}
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