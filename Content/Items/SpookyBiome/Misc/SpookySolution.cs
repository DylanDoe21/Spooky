using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

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
			Item.value = Item.buyPrice(0, 0, 25);
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
				Convert((int)(Projectile.position.X + (Projectile.width * 0.5f)) / 16, (int)(Projectile.position.Y + (Projectile.height * 0.5f)) / 16, 2);
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

		private static void Convert(int i, int j, int size = 4) 
        {
			for (int k = i - size; k <= i + size; k++) 
            {
				for (int l = j - size; l <= j + size; l++) 
                {
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size))) 
                    {
                        //replace normal grass with orange grass
                        int[] GrassReplace = { TileID.Grass, TileID.HallowedGrass };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType)) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyGrass>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace corrupt biome grasses with green grass
                        int[] GreenGrassReplace = { TileID.CorruptGrass, TileID.CrimsonGrass };

                        if (GreenGrassReplace.Contains(Main.tile[k, l].TileType)) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace dirt blocks with spooky dirt
						if (Main.tile[k, l].TileType == TileID.Dirt) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyDirt>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace stone blocks with spooky stone
						if (TileID.Sets.Conversion.Stone[Main.tile[k, l].TileType]) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyStone>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace grass walls with spooky grass walls
                        int[] WallReplace = { WallID.GrassUnsafe, WallID.FlowerUnsafe, WallID.Grass, WallID.Flower, 
                        WallID.CorruptGrassUnsafe, WallID.HallowedGrassUnsafe, WallID.CrimsonGrassUnsafe };

						if (WallReplace.Contains(Main.tile[k, l].WallType)) 
                        {
							Main.tile[k, l].WallType = (ushort)ModContent.WallType<SpookyGrassWall>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}
					}
				}
			}
		}
	}
}