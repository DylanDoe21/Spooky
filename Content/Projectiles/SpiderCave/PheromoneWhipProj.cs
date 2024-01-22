using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class PheromoneWhipProj : ModProjectile
	{
		public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			Projectile.DefaultToWhip();

			Projectile.WhipSettings.Segments = 30;
			Projectile.WhipSettings.RangeMultiplier = 1f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(damageDone * 0.8f);

			//TODO: add buff to enemies that makes them take more damage from spider-themed minions
		}

		public override bool PreDraw(ref Color lightColor) 
        {
			List<Vector2> list = new();
			Projectile.FillWhipControlPoints(Projectile, list);

			Main.instance.LoadProjectile(Type);
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) 
            {
				Rectangle frame = new Rectangle(0, 0, 20, 20);
				Vector2 origin = new Vector2(10, 10);
				float scale = 1;

				//tip of the whip
				if (i == list.Count - 2) 
				{
					frame.Y = 60;
					frame.Height = 22;
				}
				//loop between the two middle segments
				else if (i % 2 == 0) 
				{
					frame.Y = 46;
					frame.Height = 10;
				}
				else if (i % 1 == 0) 
				{
					frame.Y = 34;
					frame.Height = 10;
				}
				//the held part of the whip
				else if (i > 0) 
				{
					frame.Y = 0;
					frame.Height = 22;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; //This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, effects, 0);

				pos += diff;
			}

			return false;
		}
	}
}