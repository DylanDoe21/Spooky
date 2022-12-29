using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
	public class SpookyClearSolution : ModItem
	{
		public override void SetStaticDefaults() 
        {
			DisplayName.SetDefault("Anti Spooky Solution");
			Tooltip.SetDefault("Used by the Clentaminator\nClears the Spooky Forest");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
		}

		public override void SetDefaults() 
        {
            Item.width = 12;
			Item.height = 12;
			Item.value = Item.buyPrice(0, 0, 25);
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = 999;
			Item.consumable = true;
            Item.ammo = AmmoID.Solution;
            Item.shoot = ModContent.ProjectileType<SpookyClearSolutionProj>() - ProjectileID.PureSpray;
		}
	}

	public class SpookyClearSolutionProj : ModProjectile
	{
		public ref float Progress => ref Projectile.ai[0];

		public override void SetStaticDefaults() 
        {
			DisplayName.SetDefault("Spooky Spray");
		}

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
			int dustType = DustID.GemDiamond;

			if (Projectile.owner == Main.myPlayer)
            {
				Convert((int)(Projectile.position.X + (Projectile.width * 0.5f)) / 16, (int)(Projectile.position.Y + (Projectile.height * 0.5f)) / 16, 2);
			}

			if (Projectile.timeLeft > 133) 
            {
				Projectile.timeLeft = 133;
			}

			if (Progress > 7f) {
				float dustScale = 1f;

				if (Progress == 8f) 
                {
					dustScale = 0.2f;
				}
				else if (Progress == 9f) 
                {
					dustScale = 0.4f;
				}
				else if (Progress == 10f) 
                {
					dustScale = 0.6f;
				}
				else if (Progress == 11f) 
                {
					dustScale = 0.8f;
				}

				Progress += 1f;


				var dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);

				dust.noGravity = true;
				dust.scale *= 1.75f;
				dust.velocity.X *= 2f;
				dust.velocity.Y *= 2f;
				dust.scale *= dustScale;
			}
			else 
            {
				Progress += 1f;
			}

			Projectile.rotation += 0.3f * Projectile.direction;
		}

		private static void Convert(int i, int j, int size = 4) 
        {
			for (int k = i - size; k <= i + size; k++) 
            {
				for (int l = j - size; l <= j + size; l++) 
                {
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size))) 
                    {

                        //replace spooky grasses with regular grass
                        int[] GrassReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>() };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType)) 
                        {
							Main.tile[k, l].TileType = TileID.Grass;
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace spooky dirt with dirt 
						if (Main.tile[k, l].TileType == ModContent.TileType<SpookyDirt>()) 
                        {
							Main.tile[k, l].TileType = TileID.Dirt;
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace spooky stone with stone
						if (Main.tile[k, l].TileType == ModContent.TileType<SpookyStone>()) 
                        {
							Main.tile[k, l].TileType = TileID.Stone;
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace spooky grass walls with grass walls
                        if (Main.tile[k, l].WallType == ModContent.WallType<SpookyGrassWall>()) 
                        {
							Main.tile[k, l].WallType = WallID.GrassUnsafe;
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}
					}
				}
			}
		}
	}
}