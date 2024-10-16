using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

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
            NPC.lifeMax = 45;
            NPC.damage = 35;
            NPC.defense = 0;
            NPC.width = 28;
            NPC.height = 24;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath6;
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

            NPC Parent = Main.npc[(int)NPC.ai[0]];

            if (!Parent.active)
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
            NPC.position.X = Parent.Center.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
            NPC.position.Y = Parent.Center.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;

            //shoot itself as a projectile
            NPC.ai[2]++;

            if (NPC.ai[2] >= 45)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.position);

                Vector2 ShootSpeed = Parent.Center - NPC.Center;
                ShootSpeed.Normalize();
                ShootSpeed.X *= -Main.rand.NextFloat(2f, 4f);
                ShootSpeed.Y *= -Main.rand.NextFloat(2f, 4f);

                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<PhantomSkullProj>(), NPC.damage, 4.5f);

                NPC.active = false;
            }
        }
    }
}