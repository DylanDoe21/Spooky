using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
    public class LabScreenRusted : ModTile
    {
        public override void SetStaticDefaults()
        {
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileWaterDeath[Type] = false;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(156, 154, 121));
            DustType = -1;
            HitSound = SoundID.NPCHit53 with { Volume = 0.5f };
		}
    }
}