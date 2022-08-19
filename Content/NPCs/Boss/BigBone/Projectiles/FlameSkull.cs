using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class FlameSkull : ModNPC
    {
        public float spinAmount = 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Skull");
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 2000;
            NPC.damage = 85;
            NPC.defense = 15;
            NPC.width = 36;
            NPC.height = 34;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.Item73;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, NPC.height * 0.5f);

            for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
                float scale = (NPC.scale) * (NPC.oldPos.Length - oldPos) / NPC.oldPos.Length * 1f;
                Color color = Color.Lerp(Color.Yellow, Color.Red, oldPos / (float)NPC.oldPos.Length) * 0.65f * ((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length);
                Main.EntitySpriteDraw(tex, drawPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, scale, effects, 0);
            }

            return true;
        }
        
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.5f, 0.15f, 0f);

            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC parent = Main.npc[(int)NPC.ai[1]];

            NPC.spriteDirection = NPC.direction;

            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            NPC.ai[2]++; 
            if (NPC.ai[2] >= 600)
            {
                spinAmount -= 0.05f;

                if (spinAmount <= 0.5f)
                {
                    spinAmount = 0.5f;
                }
            }
            else
            {
                spinAmount = 2f;
            }

            if (NPC.ai[2] == 700)
            {
                Vector2 ShootSpeed = parent.Center - NPC.Center;
                ShootSpeed.Normalize();
                        
                ShootSpeed.X *= -10;
                ShootSpeed.Y *= -10;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                    ProjectileID.Fireball, NPC.damage / 5, 0f, Main.myPlayer, 0, 0);
                }

                NPC.ai[2] = 0;
            }

            if (parent != Main.npc[0])
            {
                NPC.ai[0] += spinAmount;
                int distance = 250;
                double rad = NPC.ai[0] * (Math.PI / 180);
                NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
                NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;
            }

            if (!parent.active)
            {
                NPC.active = false;
            }
        }

        public override bool CheckDead()
        {
            Player player = Main.player[NPC.target];

            Vector2 ShootSpeed = player.Center - NPC.Center;
            ShootSpeed.Normalize();
                    
            ShootSpeed.X *= 15;
            ShootSpeed.Y *= 15;

            if (Main.netMode != NetmodeID.Server) 
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                ModContent.ProjectileType<FlameSkullProj>(), NPC.damage / 5, 0f, Main.myPlayer, 0, 0);
            }

            return true;
        }
    }
}