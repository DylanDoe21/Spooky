using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Ambient
{
    public class OceanKelp : ModTile
    {
        private const int MaxChainLength = 18;
        private static readonly ushort[] ValidAnchors = new ushort[] { (ushort)ModContent.TileType<OceanSand>() };

        private const short FrameBottom = 0;
        private const short FrameMid = 18;
        private const short FrameTop = 36;
        private const short FrameWidth = 18;

        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.IsVine[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            HitSound = SoundID.Grass;
            DustType = DustID.Grass;
            AddMapEntry(new Color(26, 76, 38));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float divide = 1000f;

			r = 255f / divide;
			g = 196f / divide;
			b = 0f / divide;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Tile tile = Framing.GetTileSafely(i, j);
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!IsTopmostSegment(i, j))
                return;

            if (j <= 0 || Main.tile[i, j - 1].HasTile)
                return;

            if (!IsSupportedFromBelow(i, j))
                return;

            if (GetChainLengthUpward(i, j) >= MaxChainLength)
                return;

            bool placed = WorldGen.PlaceTile(i, j - 1, Type, mute: true, forced: true);
            if (placed)
            {
                SetFrame(i, j - 1, FrameTop);
                SetFrame(i, j, FrameMid);
                UpdateBaseSegment(i, j + GetChainLengthDownward(i, j));
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (!IsSupportedFromBelow(i, j))
            {
                WorldGen.KillTile(i, j, noItem: true, effectOnly: false);
                return false;
            }

            if (IsTopmostSegment(i, j))
                SetFrame(i, j, FrameTop);
            else if (IsBottomSegment(i, j))
                SetFrame(i, j, FrameBottom);
            else
                SetFrame(i, j, FrameMid);

            return false;
        }

        private static bool IsSupportedFromBelow(int i, int j)
        {
            if (j >= Main.maxTilesY - 1) return false;
            Tile below = Main.tile[i, j + 1];
            if (!below.HasTile) return false;

            ushort t = below.TileType;
            if (t == ModContent.TileType<OceanKelp>())
                return true;

            foreach (ushort anchor in ValidAnchors)
                if (t == anchor)
                    return true;

            return false;
        }

        private static bool IsTopmostSegment(int i, int j)
        {
            if (j <= 0) return true;
            Tile above = Main.tile[i, j - 1];
            return !(above.HasTile && above.TileType == ModContent.TileType<OceanKelp>());
        }

        private static bool IsBottomSegment(int i, int j)
        {
            if (j >= Main.maxTilesY - 1) return true;
            Tile below = Main.tile[i, j + 1];
            return !(below.HasTile && below.TileType == ModContent.TileType<OceanKelp>());
        }

        private static int GetChainLengthUpward(int i, int j)
        {
            int length = 1;
            int y = j - 1;
            int vineType = ModContent.TileType<OceanKelp>();
            while (y >= 0 && Main.tile[i, y].HasTile && Main.tile[i, y].TileType == vineType)
            {
                length++;
                y--;
                if (length > MaxChainLength) break;
            }
            return length;
        }

        private static int GetChainLengthDownward(int i, int j)
        {
            int length = 0;
            int y = j + 1;
            int vineType = ModContent.TileType<OceanKelp>();
            while (y < Main.maxTilesY && Main.tile[i, y].HasTile && Main.tile[i, y].TileType == vineType)
            {
                length++;
                y++;
            }
            return length;
        }

        private static void SetFrame(int i, int j, short frameYRow)
        {
            Tile tile = Main.tile[i, j];
            tile.TileFrameX = (short)(WorldGen.genRand.Next(3) * FrameWidth);
            tile.TileFrameY = frameYRow;
        }

        private static void UpdateBaseSegment(int i, int bottomY)
        {
            Tile t = Main.tile[i, bottomY];
            if (t.HasTile && t.TileType == ModContent.TileType<OceanKelp>())
                SetFrame(i, bottomY, FrameBottom);
        }
    }
}