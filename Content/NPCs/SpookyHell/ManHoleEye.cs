using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ManHoleEye : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stalking Eye");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 50;
            NPC.width = 28;
            NPC.height = 28;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //only draw if the parent is active
            if (Main.npc[(int)NPC.ai[3]].active)
			{
                Vector2 owner = NPC.Center;
                Vector2 rootPosition = Main.npc[(int)NPC.ai[3]].Center;

                Vector2[] bezierPoints = { rootPosition, rootPosition + new Vector2(0, -30), NPC.Center + new Vector2(-30 * NPC.direction, 0).RotatedBy(NPC.rotation), NPC.Center + new Vector2(-12 * NPC.direction, 0).RotatedBy(NPC.rotation) };
                float bezierProgress = 0;
                float bezierIncrement = 10;

                Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/ManHoleEyeChain").Value;
                Vector2 textureCenter = new Vector2(8, 8);

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

        public override void AI()
		{
            Player player = Main.player[NPC.target];

            //rotation
            Vector2 vector92 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num740 = Main.player[NPC.target].Center.X - vector92.X;
            float num741 = Main.player[NPC.target].Center.Y - vector92.Y;
            NPC.rotation = (float)Math.Atan2((double)num741, (double)num740) + 4.71f;

            //attack player if nearby
            if (NPC.Distance(player.Center) <= 500f) 
            {
                NPC.ai[0]++;

                if (NPC.ai[1] < 3)
                {
                    if (NPC.ai[0] == 180)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.position);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X = ChargeDirection.X * 35;
                        ChargeDirection.Y = ChargeDirection.Y * 25;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.ai[0] > 210)
                    {
                        NPC.ai[1]++;
                        NPC.ai[0] = 100;
                        NPC.velocity *= 0f;
                    }
                }
                else
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                }
            }
            
            //idle, float above parent
            int index1 = (int)NPC.ai[2];
            
            if (Main.npc[(int)NPC.ai[3]].active)
            {
                float goToX = Main.npc[(int)NPC.ai[3]].Center.X - NPC.Center.X;
                float goToY = (Main.npc[(int)NPC.ai[3]].Center.Y - 200) - NPC.Center.Y;

                float speed = 0.5f;

                if (Vector2.Distance(NPC.Center, Main.npc[(int)NPC.ai[3]].Center) >= 400f)
                {
                    speed = 0.8f;
                }
                else if (NPC.ai[0] >= 180)
                {
                    speed = 0.01f;
                }
                else
                {
                    speed = 0.5f;
                }
                
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

            if (!Main.npc[(int)NPC.ai[3]].active)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleEyeGore").Type);
                NPC.active = false;
            }
        }
    }
}