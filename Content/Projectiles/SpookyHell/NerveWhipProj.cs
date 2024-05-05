using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class NerveWhipProj : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			Projectile.DefaultToWhip();

			Projectile.WhipSettings.Segments = 30;
			Projectile.WhipSettings.RangeMultiplier = 1.25f;
		}

		int numHits = 0;

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			numHits++;

			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			
			if (numHits < 5)
			{
				Projectile.damage = (int)(damageDone * 1.25f);
			}
		}

		public override bool PreDraw(ref Color lightColor) 
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            List<Vector2> list = new();
			Projectile.FillWhipControlPoints(Projectile, list);

			Main.instance.LoadProjectile(Type);

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) 
            {
				Rectangle frame = new Rectangle(0, 0, 14, 18);
				Vector2 origin = new Vector2(7, 7);
				
				Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
				float t = Projectile.ai[0] / timeToFlyOut;
				float scale = MathHelper.Lerp(0.75f, 1.2f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));

				//tip of the whip
				if (i == list.Count - 2) 
				{
					frame.Y = 50;
					frame.Height = 32;
				}
				//loop between the two middle segments
				else if (i % 2 == 0) 
				{
					frame.Y = 32;
					frame.Height = 18;
				}
				else if (i % 1 == 0) 
				{
					frame.Y = 16;
					frame.Height = 18;
				}
				//the held part of the whip
				else if (i > 0) 
				{
					frame.Y = 0;
					frame.Height = 16;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; //This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(ProjTexture.Value, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}

			return false;
		}
	}
}