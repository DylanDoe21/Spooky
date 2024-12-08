using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Tiles.SpiderCave.Ambient
{
    public class HangingLightRoot1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.MultiTileSway[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default(AnchorData);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(247, 174, 73));
            DustType = DustID.Dirt;
            HitSound = SoundID.Dig;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];

			if (TileObjectData.IsTopLeft(tile))
			{
				Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.MultiTileVine);
			}

			return false;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.5f;
			g = 0.25f;
			b = 0.0f;
        }
    }

    public class HangingLightRoot2 : HangingLightRoot1
    {
    }

    public class HangingLightRoot3 : HangingLightRoot1
    {
    }

    public class HangingLightRoot4 : HangingLightRoot1
    {
    }
}