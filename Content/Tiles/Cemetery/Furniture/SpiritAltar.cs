using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
	public class SpiritAltar : ModTile
    {
		public override void SetStaticDefaults()
		{
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 5;	
			TileObjectData.newTile.Origin = new Point16(1, 4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Spirit Altar");
			AddMapEntry(new Color(136, 0, 253), name);
            DustType = DustID.Stone;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}

        public override bool RightClick(int i, int j)
		{
            int x = i;
            int y = j;
            while (Main.tile[x, y].TileType == Type) x--;
            x++;
            while (Main.tile[x, y].TileType == Type) y--;
            y++;

            if (!NPC.downedBoss2)
            {
                Main.NewText("The spirit has not deemed you worthy of playing it's organ", 171, 64, 255);
            }
            else
            {
                //good spawn noise: DD2_EtherianPortalOpen
                //DD2_DefeatScene and DD2_WinScene are cool too
            }

            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/Furniture/SpiritAltarOutline").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            float glowspeed = Main.GameUpdateCount * 0.02f;
            float glowbrightness = (float)MathF.Sin(j / 15f - glowspeed);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Fuchsia * glowbrightness);
		}
    }
}