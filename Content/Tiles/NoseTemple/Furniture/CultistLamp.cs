using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class CultistLamp : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(98, 67, 82), name);
            RegisterItemDrop(ModContent.ItemType<CultistLampItem>());
            DustType = DustID.Stone;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 1;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            for (int x = left; x < left + 1; x++)
            {
                for (int y = top; y < top + 3; y++)
                {
                    if (Main.tile[x, y].TileFrameX >= 18)
                    {
                        Main.tile[x, y].TileFrameX -= 18;
                    }
                    else
                    {
                        Main.tile[x, y].TileFrameX += 18;
                    }
                }
            }

            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
                Wiring.SkipWire(left, top + 2);
            }

            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 18)
            {
                r = 0.5f;
                g = 0.85f;
                b = 0.3f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/NoseTemple/Furniture/CultistLampGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}