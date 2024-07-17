using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Blooms
{
    public class BlueberryExplosion : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

		public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int numDusts = 0; numDusts < 20; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<BlueberrySnowflake>(), 0f, -2f, 0, Color.Cyan * 0.5f, 1.2f);
                Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                Main.dust[dust].noGravity = true;
            }

            for (int target = 0; target < Main.maxNPCs; target++)
            {
                NPC npc = Main.npc[target];

                if (npc.Distance(Projectile.Center) <= 160f && npc.active && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type] && !npc.HasBuff(ModContent.BuffType<BlueberryFrost>()))
                {
                    npc.AddBuff(ModContent.BuffType<BlueberryFrost>(), 240);
                }
            }
        }
    }
}