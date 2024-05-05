using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.Tiles.SpiderCave.Ambient
{
    public class OldHunterPileHat : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(128, 38, 10));
            HitSound = SoundID.Dig;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<OldHunterHat>());
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public static void DrawGlow(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
        {
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));

            Main.spriteBatch.Draw(tex, drawPos, source, Color.White, 0, origin ?? source.Value.Size() / 3f, 2f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Ambient/OldHunterPileGlow");

            Tile tile = Framing.GetTileSafely(i, j);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Vector2 Offset = new Vector2(8, 2);

                DrawGlow(i, j - 3, GlowTexture.Value, new Rectangle(0, 0, 14, 48), TileOffset.ToWorldCoordinates(), Offset);
            }

            return true;
        }
    }

    public class OldHunterPileLegs : OldHunterPileHat
    {
        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<OldHunterLegs>());
        }
    }

    public class OldHunterPileSkull : OldHunterPileHat
    {
        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<OldHunterSkull>());
        }
    }

    public class OldHunterPileTorso : OldHunterPileHat
    {
        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<OldHunterTorso>());
        }
    }
}