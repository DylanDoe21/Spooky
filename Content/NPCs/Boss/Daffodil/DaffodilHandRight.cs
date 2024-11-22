using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    public class DaffodilHandRight : ModNPC
    {
        Vector2 SavePlayerPosition;

        public bool HasHitSurface = false;

        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePlayerPosition);

            //bools
            writer.Write(HasHitSurface);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
            SavePlayerPosition = reader.ReadVector2();

            //bools
            HasHitSurface = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 18000;
            NPC.damage = 50;
            NPC.defense = 0;
            NPC.width = 56;
            NPC.height = 56;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.behindTiles = true;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
            {
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArm");

                Vector2 ParentCenter = Parent.Center;

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y - 10);
                Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;
    
                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }

                if (Parent.ai[0] == 4 && Parent.localAI[0] >= 40 && Parent.localAI[0] < 200 && !HasHitSurface)
                {
                    NPCTexture ??= ModContent.Request<Texture2D>(Texture);
                    Color newColor = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

                    for (int repeats = 0; repeats < 4; repeats++)
                    {
                        Color color = newColor;
                        color = NPC.GetAlpha(color);
                        Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + NPC.rotation.ToRotationVector2() - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                        Main.spriteBatch.Draw(NPCTexture.Value, afterImagePosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, SpriteEffects.None, 0f);
                    }
                }
            }

            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            if (Parent.ai[0] == 1 && Parent.localAI[0] >= 60 && Parent.localAI[0] <= 260)
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
            else if (Parent.ai[0] == 4 && Parent.localAI[0] >= 40 && Parent.localAI[0] < 200)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else
            {
                NPC.frame.Y = frameHeight * 0;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            return Parent.ai[0] == 4;
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            Player player = Main.player[Parent.target];

            NPC.direction = 1;

            //kill the hand if the parent does not exist
            if (!Parent.active)
            {
                NPC.active = false;
            }

            if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
            {
                //set rotation based on the parent npc
                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                float RotateX = Parent.Center.X + 65 - vector.X;
                float RotateY = Parent.Center.Y - vector.Y;
                NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
            }

            switch ((int)Parent.ai[0])
            {
                case -5: 
                {
                    GoToPosition(0, 0);

                    break;
                }

                case -4:
                {
                    GoToPosition(0, 0);

                    break;
                }

                case -3: 
                {
                    GoToPosition(0, 0);
                    NPC.velocity *= 0.98f;

                    break;
                }

                case -2: 
                {
                    if (Parent.localAI[0] <= 180 || Parent.localAI[0] >= 360)
                    {
                        GoToPosition(130, 180);
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
                        GoToPosition(240, 25);
                    }

                    break;
                }

                case -1: 
                {
                    GoToPosition(130, 180);

                    break;
                }

                case 0: 
                {
                    GoToPosition(130, 180);

                    break;
                }

                case 1: 
                {
                    if (Parent.localAI[0] < 60 || Parent.localAI[0] > 260)
                    {
                        GoToPosition(210, 100);
                    }

                    if (Parent.localAI[0] >= 60 && Parent.localAI[0] <= 260)
                    {
                        GoToPosition(300, 35);
                    }

                    if (Parent.localAI[0] >= 120 && Parent.localAI[0] <= 240)
                    {
                        if (Main.rand.NextBool(10))
                        {
                            SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                            Vector2 ShootSpeed = Parent.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= -10f;

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-20, 20), NPC.Center.Y + Main.rand.Next(-20, 20)), 
                            ShootSpeed, ModContent.ProjectileType<ChlorophyllFlower>(), NPC.damage, 4.5f);
                        }
                    }

                    break;
                }

                case 2: 
                {
                    if (Parent.localAI[0] < 60 || Parent.localAI[0] > 240)
                    {
                        GoToPosition(130, 160);
                    }
                    else if (Parent.localAI[0] > 60 && Parent.localAI[0] <= 240)
                    {
                        GoToPosition(50, 200);
                    }

                    break;
                }

                case 3: 
                {
                    GoToPosition(300, 35);

                    break;
                }

                case 4: 
                {
                    if (Parent.localAI[0] < 60)
                    {
                        GoToPosition(250, 35);

                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.12f);
                            Main.dust[dustEffect].color = Color.Red;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (Parent.localAI[0] == 60)
                    {
                        SavePlayerPosition = player.Center;

                        NPC.netUpdate = true;
                    }

                    if (Parent.localAI[0] == 70)
                    {
                        Vector2 Recoil = SavePlayerPosition - NPC.Center;
                        Recoil.Normalize();
                        Recoil *= -5;
                        NPC.velocity = Recoil;
                    }
                    
                    if (Parent.localAI[0] == 80)
                    {
                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                        ChargeDirection *= 30;
                        NPC.velocity = ChargeDirection;
                    }

                    if (Parent.localAI[0] > 80 && Parent.localAI[0] < 140 && !HasHitSurface)
                    {
                        if (NPC.velocity.X <= 0.1f && NPC.velocity.X >= -0.1f)
                        {
                            NPC.velocity *= 0;
                        }

                        if (NPC.velocity.Y <= 0.1f && NPC.velocity.Y >= -0.1f)
                        {
                            NPC.velocity *= 0;
                        }

                        if (NPC.velocity == Vector2.Zero)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath43, NPC.Center);
                            
                            SpookyPlayer.ScreenShakeAmount = 8;

                            HasHitSurface = true;

                            NPC.netUpdate = true;
                        }
                    }
                    
                    if (Parent.localAI[0] > 140 && HasHitSurface)
                    {
                        GoToPosition(130, 180);
                        NPC.velocity *= 0.99f;
                    }

                    if (Parent.localAI[0] >= 250)
                    {
                        HasHitSurface = false;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                case 5: 
                {
                    if (Parent.localAI[0] < 300)
                    {
                        GoToPosition(300, -50);
                    }
                    else
                    {
                        GoToPosition(130, 180);
                    }

                    break;
                }

                case 6: 
                {
                    GoToPosition(130, 180);

                    break;
                }

                case 7: 
                {
                    GoToPosition(130, 180);

                    break;
                }
            }
        }

        public void GoToPosition(float X, float Y)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            float goToX = (Parent.Center.X + X) - NPC.Center.X;
            float goToY = (Parent.Center.Y + Y) - NPC.Center.Y;

            NPC.ai[1]++;
            goToX -= (float)Math.Sin(NPC.ai[1] / 30) * 15;
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