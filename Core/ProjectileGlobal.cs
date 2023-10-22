using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Linq;

using Spooky.Content.Biomes;
using Spooky.Content.Generation;
using Spooky.Content.Projectiles.Sentient;

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
                if (Main.rand.NextBool(5))
                {
                    target.AddBuff(BuffID.OnFire, 120);
                }
            }
        }

        public override bool PreAI(Projectile projectile)
		{
            //disable gravestones in the catacombs to prevent graveyards from forming there
            int[] Gravestones = new int[] {ProjectileID.Tombstone, ProjectileID.GraveMarker, ProjectileID.CrossGraveMarker,
            ProjectileID.Headstone, ProjectileID.Gravestone, ProjectileID.Obelisk, ProjectileID.RichGravestone1, ProjectileID.RichGravestone2,
            ProjectileID.RichGravestone3, ProjectileID.RichGravestone4, ProjectileID.RichGravestone5 };

			if (Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<CatacombBiome>()) || Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<CatacombBiome2>()))
            {
                if (Gravestones.Contains(projectile.type))
                {
                    projectile.active = false;
                    return false;
                }
			}

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

            //dont allow fishing in the blood lake in the valley of eyes, unless you have the goblin shark rod
            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
            {
                if (projectile.aiStyle == ProjAIStyleID.Bobber && projectile.wet && projectile.type != ModContent.ProjectileType<SentientChumCasterBobber>())
                {
                    projectile.Kill();
                }
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
