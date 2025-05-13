using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
    public class ChristmasWoodWall : ModWall 
    {
        private static Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(76, 52, 42));
            RegisterItemDrop(ModContent.ItemType<ChristmasWoodWallItem>());
            DustType = DustID.WoodFurniture;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasWoodWallMerge");

			Vector2 posLeft = TileGlobal.TileCustomPosition(i - 1, j);
            Vector2 posRight = TileGlobal.TileCustomPosition(i + 1, j);
			Color color = TileGlobal.GetWallColorWithPaint(i, j, Lighting.GetColor(i, j));

			//left wall merge
			if (Main.tile[i - 1, j].WallType != Type && Main.tile[i - 1, j].WallType > 0 && !Main.wallLight[Main.tile[i - 1, j].WallType])
			{
				spriteBatch.Draw(MergeTexture.Value, posLeft + new Vector2(6, 0), new Rectangle(18 * 1, 0, 16, 16), color);
			}
			//right wall merge
			if (Main.tile[i + 1, j].WallType != Type && Main.tile[i + 1, j].WallType > 0 && !Main.wallLight[Main.tile[i + 1, j].WallType])
			{
				spriteBatch.Draw(MergeTexture.Value, posRight - new Vector2(6, 0), new Rectangle(18 * 0, 0, 16, 16), color);
			}
		}
    }

    public class ChristmasWoodWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasWoodWall";

        private static Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(76, 52, 42));
            DustType = DustID.WoodFurniture;
        }
        
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasWoodWallMerge");

			Vector2 posLeft = TileGlobal.TileCustomPosition(i - 1, j);
			Vector2 posRight = TileGlobal.TileCustomPosition(i + 1, j);
			Color color = TileGlobal.GetWallColorWithPaint(i, j, Lighting.GetColor(i, j));

			//left wall merge
			if (Main.tile[i - 1, j].WallType != Type && Main.tile[i - 1, j].WallType > 0 && !Main.wallLight[Main.tile[i - 1, j].WallType])
			{
				spriteBatch.Draw(MergeTexture.Value, posLeft + new Vector2(6, 0), new Rectangle(18 * 1, 0, 16, 16), color);
			}
			//right wall merge
			if (Main.tile[i + 1, j].WallType != Type && Main.tile[i + 1, j].WallType > 0 && !Main.wallLight[Main.tile[i + 1, j].WallType])
			{
				spriteBatch.Draw(MergeTexture.Value, posRight - new Vector2(6, 0), new Rectangle(18 * 0, 0, 16, 16), color);
			}
		}
    }
}