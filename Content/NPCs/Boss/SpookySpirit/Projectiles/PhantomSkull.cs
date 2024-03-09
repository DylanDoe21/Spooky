using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class PhantomSkull : ModNPC
    {   
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.damage = 35;
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
            Lighting.AddLight(NPC.Center, 0.5f, 0.35f, 0.7f);

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

            NPC.ai[1] += 2f;
            int distance = 165;
            double rad = NPC.ai[1] * (Math.PI / 180);
            NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
            NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;

            //shoot itself as a projectile
            NPC.ai[2]++;

            if (NPC.ai[2] >= 45)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                float rotation = (float)Math.Atan2(NPC.Center.Y - player.Center.Y, NPC.Center.X - player.Center.X);

                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)((Math.Cos(rotation) * 3.8f) * -1),
                (float)((Math.Sin(rotation) * 3.8f) * -1), ModContent.ProjectileType<PhantomSkullProj>(), NPC.damage / 2, 0f, Main.myPlayer);

                NPC.active = false;
            }
        }
    }
}