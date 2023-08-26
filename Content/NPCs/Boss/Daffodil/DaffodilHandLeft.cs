using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    public class DaffodilHandLeft : ModNPC
    {
        public bool GrabbingPlayer = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(GrabbingPlayer);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            GrabbingPlayer = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 18000;
            NPC.damage = 45;
            NPC.defense = 0;
            NPC.width = 56;
            NPC.height = 56;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.behindTiles = true;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
            {
                Vector2 ParentCenter = Parent.Center;

                Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArm");

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y - 10);
                Vector2 vectorFromProjectileToPlayerArms = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    var chainTextureToDraw = chainTexture;

                    Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            if (Parent.ai[0] == 1 && Parent.localAI[0] >= 60)
            {
                NPC.frame.Y = frameHeight * 1;
            }
            else if (Parent.ai[0] == -2 && Parent.localAI[0] > 180 && Parent.localAI[0] <= 300)
            {
                NPC.frame.Y = frameHeight * 1;
            }
            else if (Parent.ai[0] == 2 && Parent.localAI[0] > 60 && Parent.localAI[0] <= 240)
            {
                NPC.frame.Y = frameHeight * 1;
            }   
            else if (Parent.ai[0] == 3)
            {
                NPC.frame.Y = frameHeight * 1;
            }
            else
            {
                NPC.frame.Y = frameHeight * 0;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //TODO: daffodils hands will be able to hit the player if grabbing or punching
            return false;
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            Player player = Main.player[Parent.target];

            int Damage = Main.masterMode ? 60 / 3 : Main.expertMode ? 40 / 2 : 30;

            NPC.direction = -1;

            //add light for visibility
            Lighting.AddLight(NPC.Center, 0.5f, 0.45f, 0f);

            //kill the hand if the parent does not exist
            if (!Parent.active)
            {
                NPC.active = false;
            }

            if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
            {
                //set rotation based on the parent npc
                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                float RotateX = Parent.Center.X - 65 - vector.X;
                float RotateY = Parent.Center.Y - vector.Y;
                NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
            }

            switch ((int)Parent.ai[0])
            {
                case -4: 
                {
                    GoToPosition(0, 0);

                    break;
                }

                case -3: 
                {
                    GoToPosition(0, 0);

                    break;
                }

                case -2: 
                {
                    if (Parent.localAI[0] <= 180 || Parent.localAI[0] >= 360)
                    {
                        GoToPosition(-130, 180);
                    }

                    if (Parent.localAI[0] > 180 && Parent.localAI[0] <= 300)
                    {
                        SpookyPlayer.ScreenShakeAmount = 5;

                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.12f);
                            Main.dust[dustEffect].color = Color.Green;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (Parent.localAI[0] > 180 && Parent.localAI[0] <= 360)
                    {
                        GoToPosition(-240, 25);
                    }

                    break;
                }

                case -1: 
                {
                    GoToPosition(-130, 180);

                    break;
                }

                case 0: 
                {
                    GoToPosition(-130, 180);

                    break;
                }

                case 1: 
                {
                    if (Parent.localAI[0] < 60)
                    {
                        GoToPosition(-210, 100);
                    }

                    if (Parent.localAI[0] >= 60)
                    {
                        GoToPosition(-300, 35);
                    }

                    if (Parent.localAI[0] >= 120 && Parent.localAI[0] <= 240)
                    {
                        if (Main.rand.NextBool(10))
                        {
                            SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                            Vector2 ShootSpeed = Parent.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= -10f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-20, 20), NPC.Center.Y + Main.rand.Next(-20, 20), 
                            ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<ChlorophyllFlower>(), Damage, 0f, Main.myPlayer);
                        }
                    }

                    break;
                }

                case 2: 
                {
                    if (Parent.localAI[0] < 60 || Parent.localAI[0] > 240)
                    {
                        GoToPosition(-130, 160);
                    }
                    else if (Parent.localAI[0] > 60 && Parent.localAI[0] <= 240)
                    {
                        GoToPosition(-50, 200);
                    }

                    break;
                }

                case 3: 
                {
                    GoToPosition(-300, 35);

                    break;
                }

                case 4: 
                {
                    if (Parent.localAI[0] < 300)
                    {
                        GoToPosition(-300, -50);
                    }
                    else
                    {
                        GoToPosition(-130, 180);
                    }

                    break;
                }

                case 5: 
                {
                    GoToPosition(-130, 180);

                    break;
                }

                case 6: 
                {
                    GoToPosition(-130, 180);

                    break;
                }
            }
        }

        public void GoToPosition(float X, float Y)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            float goToX = (Parent.Center.X + X) - NPC.Center.X;
            float goToY = (Parent.Center.Y + Y) - NPC.Center.Y;

            NPC.ai[1]++;
            goToX += (float)Math.Sin(NPC.ai[1] / 30) * 15;
            goToY += (float)Math.Sin(NPC.ai[1] / 30) * 15;

            float speed = 0.3f;
            
            if (NPC.velocity.X > speed)
            {
                NPC.velocity.X *= 0.98f;
            }
            if (NPC.velocity.Y > speed)
            {
                NPC.velocity.Y *= 0.98f;
            }

            if (NPC.velocity.X < goToX)
            {
                NPC.velocity.X = NPC.velocity.X + speed;
                if (NPC.velocity.X < 0f && goToX > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + speed;
                }
            }
            else if (NPC.velocity.X > goToX)
            {
                NPC.velocity.X = NPC.velocity.X - speed;
                if (NPC.velocity.X > 0f && goToX < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - speed;
                }
            }
            if (NPC.velocity.Y < goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y + speed;
                if (NPC.velocity.Y < 0f && goToY > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + speed;
                    return;
                }
            }
            else if (NPC.velocity.Y > goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y - speed;
                if (NPC.velocity.Y > 0f && goToY < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - speed;
                    return;
                }
            }
        }
    }
}