using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Content.Generation;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
	public class SpookySolution : ModItem
	{
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

		public override void SetDefaults() 
        {
            Item.width = 12;
			Item.height = 12;
			Item.consumable = true;
			Item.value = Item.buyPrice(silver: 15);
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Orange;
            Item.ammo = AmmoID.Solution;
            Item.shoot = ModContent.ProjectileType<SpookySolutionProj>() - ProjectileID.PureSpray;
		}
	}

	public class SpookySolutionProj : ModProjectile
	{
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults() 
        {
			Projectile.width = 6;
			Projectile.height = 6;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 2;
            Projectile.alpha = 255;
		}

		public override void AI() 
        {
			if (Projectile.owner == Main.myPlayer)
            {
				TileConversionMethods.ConvertPurityIntoSpooky((int)(Projectile.position.X + (Projectile.width * 0.5f)) / 16, (int)(Projectile.position.Y + (Projectile.height * 0.5f)) / 16, 2);
			}

			if (Projectile.timeLeft > 133) 
            {
				Projectile.timeLeft = 133;
			}

			if (Projectile.ai[0] > 7f) 
			{
				float dustScale = 1f;

				if (Projectile.ai[0] == 8f) 
                {
					dustScale = 0.2f;
				}
				else if (Projectile.ai[0] == 9f) 
                {
					dustScale = 0.4f;
				}
				else if (Projectile.ai[0] == 10f) 
                {
					dustScale = 0.6f;
				}
				else if (Projectile.ai[0] == 11f) 
                {
					dustScale = 0.8f;
				}

				Projectile.ai[0] += 1f;


				var dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 
				DustID.AmberBolt, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);

				dust.noGravity = true;
				dust.scale *= Main.rand.NextFloat(1.75f, 3.5f);
				dust.velocity.X *= 2f;
				dust.velocity.Y *= 2f;
				dust.scale *= dustScale;
			}
			else 
            {
				Projectile.ai[0] += 1f;
			}

			Projectile.rotation += 0.3f * Projectile.direction;
		}
	}
}