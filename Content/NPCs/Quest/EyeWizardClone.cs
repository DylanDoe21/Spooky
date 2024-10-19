using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class EyeWizardClone : ModNPC
	{
		public override string Texture => "Spooky/Content/NPCs/Quest/EyeWizard";

		int CurrentFrameX = 0; //0 = idle flying animation  1 = go inside cloak

		Vector2 SaveNPCPosition;
        Vector2 SavePlayerPosition = Vector2.Zero;

		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
			//vector2
            writer.WriteVector2(SavePlayerPosition);
            writer.WriteVector2(SaveNPCPosition);

			//ints
            writer.Write(CurrentFrameX);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
            SavePlayerPosition = reader.ReadVector2();
            SaveNPCPosition = reader.ReadVector2();

			//ints
            CurrentFrameX = reader.ReadInt32();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 1200;
            NPC.damage = 35;
			NPC.defense = 0;
			NPC.width = 54;
			NPC.height = 116;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = -1;
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/EyeWizardGlow");

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			
			if (CurrentFrameX == 1)
            {
                for (int i = 0; i < 360; i += 90)
                {
                    Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.HotPink);

                    Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));
                    spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.1f, effects, 0);
                }
            }

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.Red * 0.5f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            //flying animation
			if (CurrentFrameX == 0)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 10)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
			else
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 8)
				{
					NPC.frame.Y = 4 * frameHeight;
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			Player player = Main.player[Parent.target];

			NPC.TargetClosest(true);
			NPC.spriteDirection = NPC.direction;

            NPC.rotation = NPC.velocity.X * 0.03f;

			//kill this npc if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EyeWizard>())
			{
				for (int numDusts = 0; numDusts < 35; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.35f);
                    Main.dust[dustGore].color = Color.Red;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }

                NPC.active = false;
			}

			NPC.ai[2]++;

			//passively spawn dust rings below to make it look like its floating
			if (NPC.ai[2] % 10 == 0)
			{
				Vector2 NPCVelocity = NPC.velocity * 0.4f + Vector2.UnitY;
				Vector2 NPCOffset = NPC.Center + new Vector2(0, NPC.height / 2);

				for (int i = 0; i <= 20; i++)
				{
					Vector2 position = -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 20) * new Vector2(1f, 0.25f);
					Vector2 velocity = NPCVelocity + position * 1.25f;
					position = position * 12 + NPCOffset;
					Dust dust = Dust.NewDustPerfect(position, 90, velocity);
					dust.noGravity = true;
					dust.scale = 0.8f + 10 * 0.04f;
				}
			}

			NPC.ai[1]++;

			//find a position for this npc to go to relative to the player
			if (NPC.ai[1] % 120 == 0 || NPC.ai[1] == 1)
			{
				SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(-500, 500), player.Center.Y - Main.rand.Next(150, 300));
			}

			if (NPC.ai[1] < 1200 && SavePlayerPosition != Vector2.Zero)
			{
				//go to the player
				Vector2 GoTo = SavePlayerPosition;

				float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 15);
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
			}

			//shoot out a homing eye
			if (NPC.ai[1] % 240 == 0)
			{
				Vector2 NPCPosition = NPC.Center + new Vector2(0, 25).RotatedByRandom(360);

				SoundEngine.PlaySound(SoundID.Item79, NPCPosition);

				Vector2 ShootSpeed = NPC.Center - NPCPosition;
				ShootSpeed.Normalize();
				ShootSpeed *= Main.rand.NextFloat(-10f, -5f);

				NPCGlobalHelper.ShootHostileProjectile(NPC, NPCPosition, ShootSpeed, ModContent.ProjectileType<HomingEye>(), NPC.damage, 3.5f);
			}

			//scale up and down before switching places
			if (NPC.ai[1] >= 600)
			{
				NPC.ai[3]++;
                if (NPC.ai[3] < 4)
                {
                    NPC.scale -= 0.25f;
                }
                if (NPC.ai[3] >= 4)
                {
                    NPC.scale += 0.25f;
                }
                
                if (NPC.ai[3] > 8)
                {
                    NPC.ai[3] = 0;
                    NPC.scale = 1f;
                }
			}

			//swap places with bigger eye itself
			if (NPC.ai[1] == 660)
			{
				NPC.scale = 1f;

				SaveNPCPosition = NPC.position;
			}
			if (NPC.ai[1] == 662)
			{
				NPC.position = Parent.position;
				Parent.position = SaveNPCPosition;

				NPC.netUpdate = true;

				NPC.ai[1] = 0;
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 35; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.35f);
                    Main.dust[dustGore].color = Color.Red;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
	}
}