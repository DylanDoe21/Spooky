using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Buffs.WhipDebuff;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
	public class EggplantWhipProj : ModProjectile
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
			Projectile.WhipSettings.RangeMultiplier = 0.85f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Player owner = Main.player[Projectile.owner];

			owner.MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(damageDone * 0.8f);

			target.AddBuff(ModContent.BuffType<EggplantWhipDebuff>(), 240);
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
				Rectangle frame = new Rectangle(0, 0, 28, 18);
				Vector2 origin = new Vector2(14, 14);
				float scale = 1;

				//tip of the whip
				if (i == list.Count - 2) 
				{
					frame.Y = 44;
					frame.Height = 22;
				}
				else if (i <= list.Count - 3 && i >= list.Count - 7)
				{
					frame.Y = 36;
					frame.Height = 6;
				}
				else if (i < list.Count - 7 && i >= list.Count - 12)
				{
					frame.Y = 28;
					frame.Height = 6;
				}
				else if (i < list.Count - 12 && i >= list.Count - 16)
				{
					frame.Y = 20;
					frame.Height = 6;
				}
				//the held part of the whip
				else if (i > 0) 
				{
					frame.Y = 12;
					frame.Height = 6;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;

				//draw the whip itself
				Color color = Lighting.GetColor(element.ToTileCoordinates());
				Main.EntitySpriteDraw(ProjTexture.Value, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}

			return false;
		}
	}
}