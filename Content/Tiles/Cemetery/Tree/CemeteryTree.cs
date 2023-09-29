using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Gores.Misc;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Tiles.Cemetery.Tree
{
	public class CemeteryTree : ModTree
	{
		//copied from example mod, idk what this does yet
		public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings 
		{
			UseSpecialGroups = true,
			SpecialGroupMinimalHueValue = 11f / 72f,
			SpecialGroupMaximumHueValue = 0.25f,
			SpecialGroupMinimumSaturationValue = 0.88f,
			SpecialGroupMaximumSaturationValue = 1f
		};

		//the tiles this tree can grow on
		public override void SetStaticDefaults() 
		{
			GrowsOnTileId = new int[1] { ModContent.TileType<CemeteryGrass>() };
		}

		//the sapling that grows into this tree
		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<CemeterySapling>();
		}

		//drop wood
		public override int DropWood()
		{
			return ModContent.ItemType<SpookyWoodItem>();
		}

		//get the tree trunk texture
		public override Asset<Texture2D> GetTexture() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/Tree/CemeteryTree");
		}

		//branch Textures
		public override Asset<Texture2D> GetBranchTextures() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/Tree/CemeteryTreeBranches");
		}

		//top Textures
		public override Asset<Texture2D> GetTopTextures() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/Tree/CemeteryTreeTops");
		}

		//special settings for the tree tops
		public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
		{
			topTextureFrameWidth = 196;
            topTextureFrameHeight = 152;
		}

		public override int TreeLeaf()
		{
			return ModContent.GoreType<LeafTeal>();
        }

        public override int CreateDust()
        {
            return DustID.WoodFurniture;
        }

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            createLeaves = true;

            return false;
        }
    }
}