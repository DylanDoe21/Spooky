using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
    public class EmporerMortarSegment : ModNPC
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
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

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

		public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];
            Player player = Main.player[Parent.target];

            NPC.alpha = Parent.alpha;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EmporerMortar>())
            {
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

            //different projectile behavior for each different mortat segment
            switch ((int)NPC.ai[2])
            {
                //first bottom segment
                case 0:
                {
                    break;
                }

                //second bottom segment
                case 1:
                {
                    break;
                }

                //first middle segment
                case 2:
                {
                    if (Parent.ai[0] == 2 && Parent.localAI[0] % 20 == 0)
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

                            NPCGlobalHelper.ShootHostileProjectile(NPC, Position, ShootSpeed, ModContent.ProjectileType<MortarRocket>(), NPC.damage, 1f);
                        }
                    }

                    break;
                }

                //second middle segment
                case 3:
                {
                    if (Parent.ai[0] == 2 && Parent.localAI[0] % 20 == 0)
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

                            NPCGlobalHelper.ShootHostileProjectile(NPC, Position, ShootSpeed, ModContent.ProjectileType<MortarRocket>(), NPC.damage, 1f, ai1: Main.rand.Next(0, 3));
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
                    if (Parent.ai[0] == 3) 
                    {
                        if (Parent.localAI[0] % 20 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.65f }, NPC.Center);
                        }

                        if (Parent.localAI[0] % 3 == 0)
                        {
                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 5f;

                            NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<MortarFire>(), NPC.damage, 4.5f);
                        }
                    }

                    break;
                }
            }

			return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
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
