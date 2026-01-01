using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
    public class EmperorMortarSegment : ModNPC
    {
        float HeatMaskAlpha = 0f;

		bool SpawnedFirefly = false;

		private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> HeatGlowTexture;

        public static readonly SoundStyle SteamSound = new("Spooky/Content/Sounds/BigBone/BigBoneHeat", SoundType.Sound) { Pitch = 0.5f, Volume = 0.7f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SendExtraAI(BinaryWriter writer)
		{
			//bools
			writer.Write(SpawnedFirefly);

			//floats
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//bools
			SpawnedFirefly = reader.ReadBoolean();

			//floats
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
		}

        public override void SetDefaults()
        {
            NPC.lifeMax = 25000;
            NPC.damage = 50;
			NPC.defense = 40;
            NPC.width = 60;
            NPC.height = 38;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.noGravity = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = 0.4f };
            NPC.aiStyle = -1;
        }

		public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = (int)NPC.ai[2] * frameHeight;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            HeatGlowTexture ??= ModContent.Request<Texture2D>(Texture + "HeatGlow");

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            if (HeatMaskAlpha > 0)
            {
                Main.EntitySpriteDraw(HeatGlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White * HeatMaskAlpha), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

		public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];
            Player player = Main.player[Parent.target];

            NPC.alpha = Parent.alpha;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EmperorMortar>())
            {
                SpawnGores((int)NPC.ai[2] + 1, NPC.ai[2] == 5 ? 1 : 2);

                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.9f);
			}

			NPC.rotation = SegmentCenter.ToRotation() - 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				int Mult = 28;

				if (NPC.ai[2] == 4)
				{
					Mult = 30;
				}
				if (NPC.ai[2] == 5)
				{
					Mult = 32;
				}

				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * Mult;
			}

            bool AnotherMinibossPresent = SpiderWarWorld.EventActiveNPCCount() > 1;

            //different projectile behavior for each different mortar segment
            switch ((int)NPC.ai[2])
            {
                //first bottom segment
                case 0:
                {
                    if (Parent.ai[0] == 1)
                    {
                        NPC.localAI[0]++;
                        NPC.localAI[1]++;

                        if (NPC.localAI[0] == 2)
                        {
                            int MinDelay = AnotherMinibossPresent ? 30 : 15;
                            NPC.localAI[2] = Main.rand.Next(MinDelay, 61);
                        }

                        if (NPC.localAI[0] > 2 && NPC.localAI[1] >= NPC.localAI[2])
                        {
                            SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);

                            int Side = Main.rand.NextBool() ? -1 : 1;

                            int ShootFromX = 45;
                            int ShootFromY = 2;

                            Vector2 Position = NPC.Center + new Vector2(ShootFromX * Side, ShootFromY).RotatedBy(NPC.rotation);
                            Vector2 ShootSpeed = new Vector2(Main.rand.Next(2, 26) * Side, 0).RotatedBy(NPC.rotation);

                            for (int numDusts = 0; numDusts < 6; numDusts++)
                            {
                                Dust dust = Dust.NewDustPerfect(Position, DustID.Smoke, ShootSpeed + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)));
                                dust.noGravity = true;
                                dust.scale = 1.2f;
                                dust.velocity += NPC.velocity;

                                Dust dust2 = Dust.NewDustPerfect(Position, DustID.Torch, ShootSpeed + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)));
                                dust2.noGravity = true;
                                dust2.scale = 1.2f;
                                dust2.velocity += NPC.velocity;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, Position, ShootSpeed, ModContent.ProjectileType<MortarMine>(), NPC.damage * 2, 4.5f);

                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                        }
                    }

                    break;
                }

                //second bottom segment
                case 1:
                {
                    goto case 0;
                }

                //first middle segment
                case 2:
                {
                    int Frequency = AnotherMinibossPresent ? 40 : 20;

                    if (Parent.ai[0] == 2 && Parent.localAI[0] % Frequency == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);

                        for (int i = -1; i <= 1; i += 2)
                        {
                            int ShootFromY = Main.rand.NextBool() ? -8 : 6;
                            int ShootFromX = ShootFromY < 0 ? 35 : 38;

                            Vector2 Position = NPC.Center + new Vector2(ShootFromX * i, ShootFromY).RotatedBy(NPC.rotation);
                            Vector2 ShootSpeed = new Vector2(5 * i, 0).RotatedBy(NPC.rotation);

                            for (int numDusts = 0; numDusts < 12; numDusts++)
                            {
                                Dust dust = Dust.NewDustPerfect(Position, DustID.Torch, ShootSpeed + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)));
                                dust.noGravity = true;
                                dust.scale = 1.2f;
                                dust.velocity += NPC.velocity;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, Position, ShootSpeed, ModContent.ProjectileType<MortarRocket>(), NPC.damage, 4.5f, ai1: Main.rand.Next(0, 3));
                        }
                    }

                    break;
                }

                //second middle segment
                case 3:
                {
                    int Frequency = AnotherMinibossPresent ? 40 : 20;

                    if (Parent.ai[0] == 2 && Parent.localAI[0] % Frequency == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);

                        for (int i = -1; i <= 1; i += 2)
                        {
                            int ShootFromY = Main.rand.NextBool() ? -8 : 6;
                            int ShootFromX = ShootFromY < 0 ? 25 : 28;

                            Vector2 Position = NPC.Center + new Vector2(ShootFromX * i, ShootFromY).RotatedBy(NPC.rotation);
                            Vector2 ShootSpeed = new Vector2(5 * i, 0).RotatedBy(NPC.rotation);

                            for (int numDusts = 0; numDusts < 12; numDusts++)
                            {
                                Dust dust = Dust.NewDustPerfect(Position, DustID.Torch, ShootSpeed + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)));
                                dust.noGravity = true;
                                dust.scale = 1.2f;
                                dust.velocity += NPC.velocity;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, Position, ShootSpeed, ModContent.ProjectileType<MortarRocket>(), NPC.damage, 4.5f, ai1: Main.rand.Next(0, 3));
                        }
                    }

                    break;
                }

                //third middle segment
                case 4:
                {
                    break;
                }

                //top segment
                case 5:
                {
					if (!SpawnedFirefly)
					{
						int NewNPC = NPC.NewNPC(NPC.GetSource_ReleaseEntity(), (int)NPC.Center.X, (int)NPC.Center.Y - 1000, ModContent.NPCType<SpotlightFirefly>(), ai3: NPC.ai[3]);
						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
						}

						SpawnedFirefly = true;
					}

                    if (Parent.ai[0] == 3) 
                    {
                        if (HeatMaskAlpha < 1)
                        {
                            HeatMaskAlpha += 0.015f;

                            if (NPC.ai[0] == 0)
                            {
                                SoundEngine.PlaySound(SteamSound, NPC.Center);
                                NPC.ai[0]++;
                            }

                            int Smoke = Dust.NewDust(NPC.Center, 0, 0, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Gray * 0.5f, 0.2f);
                            Main.dust[Smoke].noGravity = true;
                            Main.dust[Smoke].position = NPC.Center;
                            Main.dust[Smoke].velocity = new Vector2(0, -2);
                        }
                        else
                        {
                            if (NPC.ai[0] == 1)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                                NPC.ai[0]++;
                            }

                            if (Parent.localAI[0] % 20 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.65f }, NPC.Center);
                            }

                            if (Parent.localAI[0] % 5 == 0)
                            {
                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= 5f;

                                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<MortarFire>(), NPC.damage, 4.5f);
                            }
                        }
                    }
                    else
                    {
                        NPC.ai[0] = 0;

                        if (HeatMaskAlpha > 0)
                        {
                            HeatMaskAlpha -= 0.02f;
                        }
                    }

                    break;
                }
            }

			return false;
        }

        public void SpawnGores(int type, int amount)
        {
            for (int numGores = 1; numGores <= amount; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EmperorMortarSegment" + type + "Gore" + numGores).Type);
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
