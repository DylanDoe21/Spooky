using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ManHoleEye : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stalking Eye");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.damage = 50;
            NPC.width = 28;
            NPC.height = 28;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //only draw if the parent is active
            if (Main.npc[(int)NPC.ai[3]].active)
			{
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];

            //rotation
            Vector2 vector = new(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

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
                                
                        ChargeDirection.X *= 30;
                        ChargeDirection.Y *= 20;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.ai[0] > 195)
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
            if (Main.npc[(int)NPC.ai[3]].active && NPC.ai[0] < 180)
            {
                float goToX = Main.npc[(int)NPC.ai[3]].Center.X - NPC.Center.X;
                float goToY = (Main.npc[(int)NPC.ai[3]].Center.Y - 200) - NPC.Center.Y;

                float speed;

                if (Vector2.Distance(NPC.Center, Main.npc[(int)NPC.ai[3]].Center) >= 400f)
                {
                    speed = 0.8f;
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

            //kill npc if parent is not active
            if (!Main.npc[(int)NPC.ai[3]].active)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleEyeGore").Type);
                }
                
                NPC.active = false;
            }
        }
    }
}