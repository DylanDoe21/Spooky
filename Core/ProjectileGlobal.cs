using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Biomes;
using Spooky.Content.Generation;

namespace Spooky.Core
{
    public static partial class ProjectileUtil
    {
        public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
        {
            return projectile.ModProjectile as T;
        }
    }

    public class ProjectileGlobal : GlobalProjectile
    {	
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
            if (projectile.type == ProjectileID.PureSpray)
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

			return base.PreAI(projectile);
		}

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            //make the world globe change the spooky forest backgrounds
            if (projectile.type == ProjectileID.WorldGlobe && (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyBiome>()) || 
            Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyBiomeUg>())))
            {
                if (!Flags.SpookyBackgroundAlt)
                {
                    Flags.SpookyBackgroundAlt = true;
                }
                else
                { 
                    Flags.SpookyBackgroundAlt = false;
                }
                
                NetMessage.SendData(MessageID.WorldData);
            }

            return true;
        }
    }
}
