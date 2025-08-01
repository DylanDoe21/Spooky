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
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst); // Copies the attributes of the Amethyst hook's projectile.
		}

		// Use this hook for hooks that can have multiple hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook.
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

		// Amethyst Hook is 300, Static Hook is 600.
		public override float GrappleRange() 
        {
			return 500f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) 
        {
			numHooks = 2; // The amount of hooks that can be shot out
		}

		public override void GrappleRetreatSpeed(Player player, ref float speed) 
        {
			speed = 18f; // How fast the grapple returns to you after meeting its max shoot distance
		}

		public override void GrapplePullSpeed(Player player, ref float speed) 
        {
			speed = 10; // How fast you get pulled to the grappling hook projectile's landing position
		}

		// Adjusts the position that the player will be pulled towards. This will make them hang 50 pixels away from the tile being grappled.
		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) 
        {
			Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
			float hangDist = 10f;
			grappleX += dirToPlayer.X * hangDist;
			grappleY += dirToPlayer.Y * hangDist;
		}

		// Can customize what tiles this hook can latch onto, or force/prevent latching altogether, like Squirrel Hook also latching to trees
		public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
        {
			// By default, the hook returns null to apply the vanilla conditions for the given tile position (this tile position could be air or an actuated tile!)
			// If you want to return true here, make sure to check for Main.tile[x, y].HasUnactuatedTile (and Main.tileSolid[Main.tile[x, y].TileType] and/or Main.tile[x, y].HasTile if needed)

			// We make this hook latch onto trees just like Squirrel Hook

			// Tree trunks cannot be actuated so we don't need to check for that here
			Tile tile = Main.tile[x, y];
			if (TileID.Sets.IsATreeTrunk[tile.TileType] || tile.TileType == TileID.PalmTree) 
            {
				return true;
			}

			// In any other case, behave like a normal hook
			return null;
		}

		// Draws the grappling hook's chain.
		public override bool PreDrawExtras() 
        {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) 
            {
				directionToPlayer /= distanceToPlayer; // get unit vector
				directionToPlayer *= chainTexture.Height(); // multiply by chain link length

				center += directionToPlayer; // update draw position
				directionToPlayer = playerCenter - center; // update distance
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition, chainTexture.Value.Bounds, drawColor, chainRotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			return false;
		}
	}
}