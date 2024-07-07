using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.Boss.Moco
{
    public class MoclingMinion : ModNPC
    {
        public override string Texture => "Spooky/Content/NPCs/SpookyHell/Mocling";

        Vector2 GoToPosition;
        Vector2 SavePosition;
        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound) { Pitch = 0.7f, Volume = 0.5f };
        public static readonly SoundStyle FlyingSound = new("Spooky/Content/Sounds/Moco/MocoFlying", SoundType.Sound) { Pitch = 0.45f, Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 120;
            NPC.damage = 35;
            NPC.defense = 0;
            NPC.width = 38;
            NPC.height = 36;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit22 with { Pitch = 0.45f };
			NPC.DeathSound = SoundID.NPCDeath16;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/MoclingGlow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 2)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            if (NPC.ai[0] == 1 && NPC.localAI[0] > NPC.localAI[1] && NPC.localAI[0] < NPC.localAI[1] + 80)
            {
                NPC.frame.Y = 8 * frameHeight;
            }
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return NPC.ai[0] == 2;
		}

		public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC Parent = Main.npc[(int)NPC.ai[1]];

            if (!Parent.active || Parent.type != ModContent.NPCType<Moco>())
            {
                NPC.alpha += 10;

                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }
            else
            {
                if (NPC.alpha > 0)
                {
                    NPC.alpha -= 10;
                }
            }

			switch ((int)NPC.ai[0])
            {
                //fly around moco
                case 0:
                {
                    NPC.rotation = 0;
                    NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;

                    NPC.localAI[0]++;

                    //randomly go to a position around the invisible parent npc
                    if (NPC.localAI[0] == 1 || NPC.localAI[0] % 5 == 0)
                    {
                        GoToPosition = new Vector2(Parent.Center.X + Main.rand.Next(-150, 150), Parent.Center.Y + Main.rand.Next(-150, 150));
                    }

                    Vector2 GoTo = GoToPosition;

                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 6, 15, 100);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                    //switch to attacking with booger when moco uses that attack
                    if (Parent.ai[0] == 7 && Parent.localAI[0] == 140)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = Main.rand.Next(95, 120); //used to randomize the time it takes for each nose minion to shoot
                        NPC.localAI[2] = Main.rand.Next(70, 120); //used to randomize the time it takes for each nose minion to charge
                        NPC.ai[0] = Main.rand.Next(1, 3);
                    }

                    break;
                }

                //shoot snot at the player
                case 1:
                {
                    //EoC rotation
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float RotateX = player.Center.X - vector.X;
                    float RotateY = player.Center.Y - vector.Y;
                    NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
                    NPC.spriteDirection = NPC.direction;

                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 1 || NPC.localAI[0] % 20 == 0)
                    {
                        GoToPosition = new Vector2(player.Center.X + Main.rand.Next(-300, 300), player.Center.Y - Main.rand.Next(135, 165));
                    }

                    if (NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = GoToPosition;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    //save npc center
                    if (NPC.localAI[0] == 60)
                    {
                        NPC.velocity *= 0;

                        SavePosition = NPC.Center;
                    }

                    //shake before shooting
                    if (NPC.localAI[0] > 60 && NPC.localAI[0] < 90)
                    {
                        NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-7, 7);
                    }

                    if (NPC.localAI[0] >= NPC.localAI[1])
                    {
                        NPC.velocity *= 0.8f;
                    }

                    if (NPC.localAI[0] == NPC.localAI[1])
                    {
                        SoundEngine.PlaySound(SneezeSound, NPC.Center);

                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize(); 
                        Recoil *= -10;
                        NPC.velocity = Recoil;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 15f;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, ShootSpeed, ModContent.ProjectileType<NoseCultistGruntSnot>(), NPC.damage / 4, 0, NPC.target);
                    }

                    if (NPC.localAI[0] >= NPC.localAI[1] + 60)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.localAI[2] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                case 2:
                {
                    NPC.rotation = 0;
                    NPC.spriteDirection = NPC.direction;

                    NPC.localAI[0]++;

                    //go to the side of the player
                    if (NPC.localAI[0] >= 30 && NPC.localAI[0] < NPC.localAI[2])
                    {	
                        Vector2 GoTo = player.Center;
                        GoTo.X += (NPC.Center.X < player.Center.X) ? -420 : 420;
                        GoTo.Y -= 20;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    //stop before charging
                    if (NPC.localAI[0] == NPC.localAI[2])
                    {
                        SavePlayerPosition = player.Center;

                        NPC.velocity *= 0f;
                    }

                    //charge
                    if (NPC.localAI[0] == NPC.localAI[2] + 20)
                    {
                        SoundEngine.PlaySound(FlyingSound, NPC.Center);

                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 35f;
                        ChargeDirection.Y *= 35f / 2.5f;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] >= NPC.localAI[1] + 60)
                    {
                        NPC.velocity *= 0.1f;

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.localAI[2] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }

            for (int num = 0; num < Main.maxNPCs; num++)
			{
				NPC other = Main.npc[num];
				if (num != NPC.whoAmI && other.type == NPC.type && other.active && Math.Abs(NPC.position.X - other.position.X) + Math.Abs(NPC.position.Y - other.position.Y) < NPC.width)
				{
					const float pushAway = 0.2f;
					if (NPC.position.X < other.position.X)
					{
						NPC.velocity.X -= pushAway;
					}
					else
					{
						NPC.velocity.X += pushAway;
					}
					if (NPC.position.Y < other.position.Y)
					{
						NPC.velocity.Y -= pushAway;
					}
					else
					{
						NPC.velocity.Y += pushAway;
					}
				}
			}
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MoclingGore").Type);
                }
            }
        }
    }
}