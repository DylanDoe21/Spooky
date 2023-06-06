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

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    public class DaffodilHandRight : ModNPC
    {
        public override void SetStaticDefaults()
        {
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
            //only draw if the parent is active
            if (Main.npc[(int)NPC.ai[3]].active)
			{
                Vector2 armPosition = new Vector2(Main.npc[(int)NPC.ai[3]].Center.X + 65, Main.npc[(int)NPC.ai[3]].Center.Y);

                Vector2[] bezierPoints = { armPosition, armPosition + new Vector2(0, -60), NPC.Center + new Vector2(20 * NPC.direction, 0).RotatedBy(NPC.rotation), NPC.Center + new Vector2(0 * NPC.direction, 0).RotatedBy(NPC.rotation) };
                float bezierProgress = 0.1f;
                float bezierIncrement = 20;

                Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArm").Value;
                Vector2 textureCenter = NPC.spriteDirection == -1 ? new Vector2(16, 16) : new Vector2(16, 16);

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
            NPC.TargetClosest(true);

            NPC.direction = 1;

            //add light for visibility
            Lighting.AddLight(NPC.Center, 0.5f, 0.45f, 0f);

            //kill the hand if the parent does not exist
            if (!Main.npc[(int)NPC.ai[3]].active)
            {
                NPC.active = false;
            }

            //set rotation based on the parent npc
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = Main.npc[(int)NPC.ai[3]].Center.X + 65 - vector.X;
            float RotateY = Main.npc[(int)NPC.ai[3]].Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            if (Main.npc[(int)NPC.ai[3]].active && Main.npc[(int)NPC.ai[3]].type == ModContent.NPCType<DaffodilEye>())
            {
                GoToPosition(130, 180);
            }
        }

        public void GoToPosition(float X, float Y)
        {
            //NPC.ai[3] is the parent npc for this hand
            float goToX = (Main.npc[(int)NPC.ai[3]].Center.X + X) - NPC.Center.X;
            float goToY = (Main.npc[(int)NPC.ai[3]].Center.Y + Y) - NPC.Center.Y;

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