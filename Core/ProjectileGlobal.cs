using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Linq;

using Spooky.Content.Biomes;

namespace Spooky.Core
{
    public class ProjectileGlobal : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MagicCandle && projectile.DamageType == DamageClass.Magic)
            {
                if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(BuffID.OnFire, 120);
                }
            }
        }

        public override bool PreAI(Projectile projectile)
		{
            //disable gravestones in the catacombs to prevent clutter
            int[] Gravestones = new int[] {ProjectileID.Tombstone, ProjectileID.GraveMarker, ProjectileID.CrossGraveMarker,
            ProjectileID.Headstone, ProjectileID.Gravestone, ProjectileID.Obelisk, ProjectileID.RichGravestone1, ProjectileID.RichGravestone2,
            ProjectileID.RichGravestone3, ProjectileID.RichGravestone4, ProjectileID.RichGravestone5 };

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<CatacombBiome>()) || Main.LocalPlayer.InModBiome(ModContent.GetInstance<DeepCatacombBiome>()))
            {
                if (Gravestones.Contains(projectile.type))
                {
                    projectile.active = false;
                    return false;
                }
			}

			return base.PreAI(projectile);
		}
    }
}
