using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
	public class DinoArmHookProj : ModProjectile
	{
        private static Asset<Texture2D> chainTexture;

		public override void Load() 
        {
			chainTexture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Minibiomes/Desert/DinoArmHookChain");
		}

		public override void SetDefaults() 
        {
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
		}

		public override bool? CanUseGrapple(Player player) 
        {
			int hooksOut = 0;
			foreach (var projectile in Main.ActiveProjectiles) 
            {
				if (projectile.owner == Main.myPlayer && projectile.type == Projectile.type) 
                {
					hooksOut++;
				}
			}

			return hooksOut <= 2;
		}

		//kill the oldest hook when this is used if max grappling hooks are active
	    public override void UseGrapple(Player player, ref int type) 
        {
			int hooksOut = 0;
			int oldestHookIndex = -1;
			int oldestHookTimeLeft = 100000;
			foreach (var otherProjectile in Main.ActiveProjectiles)
            {
				if (otherProjectile.owner == player.whoAmI && otherProjectile.type == type) 
                {
					hooksOut++;
					if (otherProjectile.timeLeft < oldestHookTimeLeft) 
                    {
						oldestHookIndex = otherProjectile.whoAmI;
						oldestHookTimeLeft = otherProjectile.timeLeft;
					}
				}
			}

			if (hooksOut > 1) 
            {
				Main.projectile[oldestHookIndex].Kill();
		    }
		}

		public override float GrappleRange() 
        {
			return 500f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) 
        {
			numHooks = 2;
		}

		public override void GrappleRetreatSpeed(Player player, ref float speed) 
        {
			speed = 18f;
		}

		public override void GrapplePullSpeed(Player player, ref float speed) 
        {
			speed = 10;
		}

		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) 
        {
			Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
			float hangDist = 10f;
			grappleX += dirToPlayer.X * hangDist;
			grappleY += dirToPlayer.Y * hangDist;
		}

		public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
        {
			Tile tile = Main.tile[x, y];
			if (TileID.Sets.IsATreeTrunk[tile.TileType] || tile.TileType == TileID.PalmTree) 
            {
				return true;
			}

			//prevent hook from grappling out of the world
			if (!WorldGen.InWorld(x, y, 2))
			{
				return false;
			}

			return null;
		}

		public override bool PreDrawExtras() 
        {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) 
            {
				directionToPlayer /= distanceToPlayer; 
				directionToPlayer *= chainTexture.Height(); 

				center += directionToPlayer;
				directionToPlayer = playerCenter - center; 
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition, chainTexture.Value.Bounds, drawColor, chainRotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			return false;
		}
	}
}