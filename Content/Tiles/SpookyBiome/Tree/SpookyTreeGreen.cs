using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Gores;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Tiles.SpookyBiome.Tree
{
	public class SpookyTreeGreen : ModTree
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
			GrowsOnTileId = new int[1] { ModContent.TileType<SpookyGrassGreen>() };
		}

		//the sapling that grows into this tree
		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<SpookySaplingGreen>();
		}

		//drop wood
		public override int DropWood()
		{
			return ModContent.ItemType<SpookyWoodItem>();
		}

		//get trunk texture
		public override Asset<Texture2D> GetTexture() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/SpookyTree");
		}

		//branch Textures
		public override Asset<Texture2D> GetBranchTextures() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/SpookyTreeGreenBranches");
		}

		//top Textures
		public override Asset<Texture2D> GetTopTextures() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/SpookyTreeGreenTops");
		}

		//special settings for the tree tops
		public override void SetTreeFoliageSettings(Tile tile, int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight) 
		{
			topTextureFrameWidth = 196;
            topTextureFrameHeight = 144;
            xoffset = 36;
            floorY = 2;
		}

		public override int GrowthFXGore()
		{
			return ModContent.GoreType<LeafGreenTreeFX>();
		}
	}
}