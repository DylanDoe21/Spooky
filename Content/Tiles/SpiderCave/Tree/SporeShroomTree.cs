using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Gores.Misc;
using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Tiles.SpiderCave.Tree
{
	public class SporeShroomTree : ModTree
	{
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
			GrowsOnTileId = new int[1] { ModContent.TileType<DampMushroomGrass>() };
		}

		//the sapling that grows into this tree
		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<SporeShroomTreeSapling>();
		}

		//drop wood
		public override int DropWood()
		{
			return -1; //ModContent.ItemType<SpookyWoodItem>();
		}

		//get the tree trunk texture
		public override Asset<Texture2D> GetTexture() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Tree/SporeShroomTree");
		}

		//branch Textures
		public override Asset<Texture2D> GetBranchTextures() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Tree/SporeShroomTreeBranches");
		}

		//top Textures
		public override Asset<Texture2D> GetTopTextures() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Tree/SporeShroomTreeTops");
		}

		//special settings for the tree tops
		public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
		{
			topTextureFrameWidth = 114;
            topTextureFrameHeight = 78;
		}

		public override int TreeLeaf()
		{
			return -1; //ModContent.GoreType<LeafOrange>();
        }

        public override int CreateDust()
        {
            return DustID.Slush;
        }

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            createLeaves = false;
            return false;
        }
    }
}