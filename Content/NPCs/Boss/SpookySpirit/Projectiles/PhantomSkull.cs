using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class PhantomSkull : ModNPC
    {   
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Skull");
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.damage = 25;
            NPC.defense = 0;
            NPC.width = 28;
            NPC.height = 24;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.alpha = 255;
        }
        
        public override void AI()
        {
            Player player = Main.player[NPC.target];

            Vector2 vector92 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num740 = Main.player[NPC.target].Center.X - vector92.X;
            float num741 = Main.player[NPC.target].Center.Y - vector92.Y;
            NPC.rotation = (float)Math.Atan2((double)num741, (double)num740) + 4.71f;

            NPC parent = Main.npc[(int)NPC.ai[0]];

            if (!parent.active)
            {
                NPC.active = false;
            }

            if (NPC.alpha > 0)
			{
				NPC.alpha -= 5;
			}

            NPC.ai[2] += 0f; // wave
            NPC.ai[1] += 2f; // speed
            int distance = 165 + (int)(Math.Sin(NPC.ai[2] / 60) * 30);
            double rad = NPC.ai[1] * (Math.PI / 180); // angle to radians
            NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
            NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;

            //shoot itself as a projectile
            NPC.ai[3]++;

            if (NPC.ai[3] >= 45)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                float rotation = (float)Math.Atan2(NPC.Center.Y - player.Center.Y, NPC.Center.X - player.Center.X);

                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)((Math.Cos(rotation) * 3.8f) * -1),
                (float)((Math.Sin(rotation) * 3.8f) * -1), ModContent.ProjectileType<PhantomSkullProj>(), NPC.damage, 0f, Main.myPlayer);

                NPC.active = false;
            }
        }
    }
}