using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Biomes;
using Spooky.Content.Generation;
using Spooky.Content.Projectiles.Blooms;

namespace Spooky.Core
{
    public class ProjectileGlobal : GlobalProjectile
    {
		public override bool InstancePerEntity => true;

		public bool SpongeAbsorbAttempt = false;

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            //creepy candle makes magic projectiles inflict on fire
            if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MagicCandle && projectile.DamageType == DamageClass.Magic)
            {
                if (Main.rand.NextBool(3))
                {
                    target.AddBuff(BuffID.OnFire, 120);
                }
            }

            //root armor set makes ranged projectiles life-steal sometimes
            if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().RootSet && Main.LocalPlayer.GetModPlayer<SpookyPlayer>().RootHealCooldown <= 0 && projectile.DamageType == DamageClass.Ranged && damageDone >= 2)
            {
                if (Main.rand.NextBool(5))
                {
                    //heal based on how much damage was done
                    int LifeHealed = damageDone > 12 ? 12 : damageDone / 2;
                    Main.LocalPlayer.statLife += LifeHealed;
                    Main.LocalPlayer.HealEffect(LifeHealed, true);
                    Main.LocalPlayer.GetModPlayer<SpookyPlayer>().RootHealCooldown = 300;
                }
            }
        }

        public override bool PreAI(Projectile projectile)
		{
            //convert spooky mod tiles with different clentaminator solutions
            if (projectile.type == ProjectileID.PureSpray || projectile.type == ProjectileID.DirtSpray)
            {
                TileConversionMethods.ConvertSpookyIntoPurity((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.HallowSpray)
            {
                TileConversionMethods.ConvertSpookyIntoHallow((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.CorruptSpray)
            {
                TileConversionMethods.ConvertSpookyIntoCorruption((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.CrimsonSpray)
            {
                TileConversionMethods.ConvertSpookyIntoCrimson((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.SnowSpray)
            {
                TileConversionMethods.ConvertSpookyIntoSnow((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.SandSpray)
            {
                TileConversionMethods.ConvertSpookyIntoDesert((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }

			if (Main.LocalPlayer.GetModPlayer<BloomBuffsPlayer>().VegetableEggplantPaint && projectile.velocity != Vector2.Zero && projectile.DamageType == DamageClass.Ranged && Main.GameUpdateCount % 10 == 0)
			{
				Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<EgplantPaint>(), projectile.damage / 3, 0f, Main.LocalPlayer.whoAmI, Main.rand.Next(0, 5));
			}

			return base.PreAI(projectile);
		}
    }

	public static partial class ProjectileUtil
	{
		public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
		{
			return projectile.ModProjectile as T;
		}
	}
}
