using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    public class DaffodilHandLeft : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 18000;
            NPC.damage = 0;
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
                Vector2 armPosition = new Vector2(Parent.Center.X - 65, Parent.Center.Y);

                Vector2[] bezierPoints = { armPosition, armPosition + new Vector2(0, -60), NPC.Center + new Vector2(-20 * NPC.direction, 0).RotatedBy(NPC.rotation), NPC.Center + new Vector2(0 * NPC.direction, 0).RotatedBy(NPC.rotation) };
                float bezierProgress = 0.1f;
                float bezierIncrement = 20;

                Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArm").Value;
                Vector2 textureCenter = new Vector2(16, 16);

                float rotation;

                while (bezierProgress < 1)
                {
                    //draw stuff
                    Vector2 oldPos = BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress);

                    //increment progress
                    while ((oldPos - BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress)).Length() < bezierIncrement)
                    {
                        bezierProgress += 0.1f / BezierCurveUtil.BezierCurveDerivative(bezierPoints, bezierProgress).Length();
                    }

                    Vector2 newPos = BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress);
                    rotation = (newPos - oldPos).ToRotation() + MathHelper.Pi;

                    spriteBatch.Draw(texture, (oldPos + newPos) / 2 - Main.screenPosition, texture.Frame(), drawColor, rotation, textureCenter, NPC.scale, SpriteEffects.None, 0f);
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
            else if (Parent.ai[0] == 2 && Parent.localAI[0] >= 60)
            {
                NPC.frame.Y = frameHeight * 1;
            }   
            else
            {
                NPC.frame.Y = frameHeight * 0;
            }
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

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

            NPC.ai[0] = Parent.ai[0];

            switch ((int)NPC.ai[0])
            {
                case 0: 
                {
                    GoToPosition(-130, 180);

                    break;
                }

                case 1: 
                {
                    if (Parent.localAI[0] < 60)
                    {
                        GoToPosition(-130, 180);
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
                    GoToPosition(-70, 180);

                    break;
                }
            }
        }

        public void GoToPosition(float X, float Y)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            float goToX = (Parent.Center.X + X) - NPC.Center.X;
            float goToY = (Parent.Center.Y + Y) - NPC.Center.Y;

            NPC.ai[0]++;
            goToX += (float)Math.Sin(NPC.ai[0] / 30) * 15;
            goToY += (float)Math.Sin(NPC.ai[0] / 30) * 15;

            float speed = 0.35f;
            
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