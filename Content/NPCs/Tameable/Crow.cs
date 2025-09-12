using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.BossSummon;

namespace Spooky.Content.NPCs.Tameable
{
	public class Crow : ModNPC
	{
		int numSeedsEaten = 0;

        bool Flying = false;
		bool isAttacking = false;

		Player PlayerToFollow = null;
		Vector2 PositionToFlyAround = Vector2.Zero;

		public static readonly SoundStyle SquawkSound = new("Spooky/Content/Sounds/CrowSquawk", SoundType.Sound) { PitchVariance = 0.5f };
		public static readonly SoundStyle FlapSound = new("Spooky/Content/Sounds/TurkeyFlap", SoundType.Sound) { Volume = 0.2f };

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
			NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//ints
			writer.Write(numSeedsEaten);

			//bools
			writer.Write(Flying);
			writer.Write(isAttacking);

			//floats
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//ints
			numSeedsEaten = reader.ReadInt32();

			//bools
			Flying = reader.ReadBoolean();
			isAttacking = reader.ReadBoolean();

			//floats
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 20;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 18;
			NPC.height = 26;
            NPC.npcSlots = 0.5f;
			NPC.noGravity = false;
			NPC.chaseable = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Crow"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;

            if (Flying)
            {
                //wing flapping animation
                if (NPC.frame.Y < frameHeight * 4)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                if (NPC.frameCounter > 3)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
            else
            {
                //walking animation
                if (NPC.velocity.X != 0)
                {
					if (NPC.frameCounter > 5)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 5)
					{
						NPC.frame.Y = 1 * frameHeight;
					}
                }
				else
				{
					NPC.frame.Y = 0 * frameHeight;
				}
            }
		}

		public override bool CheckActive()
		{
			return !NPC.GetGlobalNPC<NPCGlobal>().NPCTamed; //has to be the opposite, return false = npc will not despawn
		}

		public override bool NeedSaving()
		{
			return NPC.GetGlobalNPC<NPCGlobal>().NPCTamed;
		}

		public override void AI()
        {
			//sprite direction stuff
			if (NPC.velocity.X < 0)
			{
				NPC.spriteDirection = -1;
			}
			else if (NPC.velocity.X > 0)
			{
				NPC.spriteDirection = 1;
			}
			else
			{
				NPC.spriteDirection = NPC.direction;
			}

			//randomly play a squawk sound
			if (Main.rand.NextBool(500))
            {
				SoundEngine.PlaySound(SquawkSound, NPC.Center);
			}

			//randomly fly in the air
			if (Main.rand.NextBool(300) && NPC.localAI[1] <= 0 && NPC.velocity.Y == 0 && NPC.localAI[2] == 0)
			{
				PositionToFlyAround = NPC.Center - new Vector2(0, 100);

				if (WorldGen.SolidOrSlopedTile((int)PositionToFlyAround.X / 16, (int)(PositionToFlyAround.Y / 16)))
				{
					Flying = false;
				}
				else
				{
					Flying = true;
					NPC.localAI[1] = 600;
					NPC.velocity.X = Main.rand.NextBool() ? -1 : 1;
					NPC.velocity.Y = -1;
				}
			}

			//delay before the crow can fly again
			if (!Flying && NPC.localAI[1] > 0)
			{
				NPC.localAI[1]--;
			}

			if (Flying && PositionToFlyAround != Vector2.Zero)
			{
				NPC.noGravity = true;
				NPC.aiStyle = -1;

				NPC.rotation = NPC.velocity.Y * (NPC.direction == 1 ? 0.05f : -0.05f);

				NPC.localAI[0]++;
				if (NPC.localAI[0] < 600)
				{
					FlyingAroundLocation(PositionToFlyAround, 150, 55, 2.5f, 2f, 0.01f, 0.05f);

					if (NPC.collideY && NPC.localAI[0] > 60)
					{
						PositionToFlyAround = Vector2.Zero;

						Flying = false;
						NPC.localAI[0] = 0;
					}
				}
				else
				{
					if (NPC.velocity.Y < 2)
					{
						NPC.velocity.Y += 0.1f;
					}

					NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -1, 1);

					if (NPC.collideY)
					{
						Flying = false;
						NPC.localAI[0] = 0;
					}
				}
			}
			else
			{
				NPC.noGravity = false;
				NPC.rotation = 0;
				NPC.aiStyle = 7;
			}
        }

		public void FlyingAroundLocation(Vector2 GoTo, int MaxDistX, int MaxDistY, float VelocityCapX, float VelocityCapY, float VelocityAccel, float OutOfAreaAccel)
		{
			NPC.ai[0]++;

			//if inside the x-bounds, then swim around
			if (NPC.Center.X > GoTo.X - MaxDistX && NPC.Center.X < GoTo.X + MaxDistX)
			{
				//swim right
				if (NPC.ai[0] > -200f && NPC.ai[0] < 200f)
				{
					if (NPC.velocity.X < VelocityCapX)
					{
						NPC.velocity.X += VelocityAccel;
					}
				}
				//swim left
				else
				{
					if (NPC.velocity.X > -VelocityCapX)
					{
						NPC.velocity.X -= VelocityAccel;
					}
				}
			}
			//if not, then accelerate to go back in
			else
			{
				if (NPC.Center.X <= GoTo.X - MaxDistX && NPC.velocity.X < VelocityCapX)
				{
					NPC.velocity.X += OutOfAreaAccel;
				}
				if (NPC.Center.X >= GoTo.X + MaxDistX && NPC.velocity.X > -VelocityCapX)
				{
					NPC.velocity.X -= OutOfAreaAccel;
				}
			}

			//if inside the y-bounds, then swim around
			if (NPC.Center.Y > GoTo.Y - MaxDistY && NPC.Center.Y < GoTo.Y + (MaxDistY / 2))
			{
				//swim down
				if (NPC.ai[0] > 0f)
				{
					if (NPC.velocity.Y < VelocityCapY)
					{
						NPC.velocity.Y += VelocityAccel;
					}
				}
				//swim up
				else
				{
					if (NPC.velocity.Y > -VelocityCapY)
					{
						NPC.velocity.Y -= VelocityAccel;
					}
				}
			}
			//if not, then accelerate to go back in
			else
			{
				if (NPC.Center.Y <= GoTo.Y - MaxDistY && NPC.velocity.Y < VelocityCapY)
				{
					NPC.velocity.Y += OutOfAreaAccel;
				}
				if (NPC.Center.Y >= GoTo.Y + (MaxDistY / 2) && NPC.velocity.Y > -VelocityCapY)
				{
					NPC.velocity.Y -= OutOfAreaAccel;
				}
			}

			if (NPC.ai[0] > 400f)
			{
				NPC.ai[0] = -400f;
			}

			float CollideSpeed = 0.7f;
			if (NPC.collideX)
			{
				NPC.netUpdate = true;
				NPC.velocity.X = NPC.oldVelocity.X * (0f - CollideSpeed);
				if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
				{
					NPC.velocity.X = 2f;
				}
				if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
				{
					NPC.velocity.X = -2f;
				}
			}
			if (NPC.collideY)
			{
				NPC.netUpdate = true;
				NPC.velocity.Y = NPC.oldVelocity.Y * (0f - CollideSpeed);
				if (NPC.velocity.Y > 0f && (double)NPC.velocity.Y < 1.5)
				{
					NPC.velocity.Y = 2f;
				}
				if (NPC.velocity.Y < 0f && (double)NPC.velocity.Y > -1.5)
				{
					NPC.velocity.Y = -2f;
				}
			}
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
				/*
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TurkeyGore" + numGores).Type);
                    }
                }

                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TurkeyFeatherGore").Type);
                    }
                }
				*/
            }
        }
	}
}