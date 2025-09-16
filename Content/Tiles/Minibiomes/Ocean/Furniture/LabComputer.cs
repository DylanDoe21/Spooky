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

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
    public class LabComputer : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(156, 154, 121));
            RegisterItemDrop(ModContent.ItemType<LabComputerUnsafeItem>());
            DustType = -1;
            HitSound = SoundID.NPCHit53 with { Volume = 0.5f };
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
            float divide = 700f;

			r = 255f / divide;
			g = 196f / divide;
			b = 0f / divide;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Main.tile[i, j];
            if ((tile.TileFrameX == 0 || tile.TileFrameX == 54) && tile.TileFrameY == 0)
            {
                var existing = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<LabComputerDummy>()).FirstOrDefault();
                if (existing == default)
                {
                    NPC.NewNPC(null, (int)(i + 1) * 16 + 8, (int)(j + 2) * 16 + 5, ModContent.NPCType<LabComputerDummy>());
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) 
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Furniture/LabComputerGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
		}
    }

    public class LabComputerSafe : LabComputer
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Ocean/Furniture/LabComputer";

        private Asset<Texture2D> GlowTexture;

        public override void NearbyEffects(int i, int j, bool closer)
        {
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) 
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Furniture/LabComputerGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
		}
    }
}