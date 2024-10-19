using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{ 
    public class WarlockSkull : ModNPC
    {
        int OffsetX = Main.rand.Next(-65, 66);
        int OffsetY = Main.rand.Next(-65, 66);

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 2;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 16;
            NPC.height = 16;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.alpha = 255;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw aura
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.OrangeRed, Color.Indigo, i / 30));

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

                for (int repeats = 0; repeats < 4; repeats++)
                {
                    Vector2 DrawPosition = NPC.Center + circular - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;

                    spriteBatch.Draw(NPCTexture.Value, DrawPosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.12f, SpriteEffects.None, 0f);
                }
            }

            return true;
        }
        
        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[1]];

            NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? -1 : 1;

            NPC.rotation = NPC.velocity.X * 0.1f;

            NPC.ai[0]++;

            if (NPC.ai[0] < 75)
            {
                if (NPC.alpha > 0)
                {
                    NPC.alpha -= 8;
                }

                float goToX = Parent.Center.X + OffsetX - NPC.Center.X;
                float goToY = Parent.Center.Y + OffsetY - NPC.Center.Y;
                float speed = 0.12f;

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

            if (NPC.ai[0] == 75)
            {
                double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - NPC.position.Y, Main.player[Main.myPlayer].position.X - NPC.position.X);
                NPC.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 7;
            }

            if (NPC.ai[0] > 100)
            {
                NPC.alpha += 5;

                if (NPC.alpha > 255)
                {
                    NPC.active = false;
                }
            }
        }
    }
}