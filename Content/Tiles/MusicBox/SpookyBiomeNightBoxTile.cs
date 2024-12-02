using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.MusicBox
{
    public class SpookyBiomeNightBoxTile : ModTile
    {
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(191, 142, 111), Lang.GetItemName(ItemID.MusicBox));
            DustType = -1;
            HitSound = SoundID.Dig;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<SpookyBiomeNightBox>();
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];

            if (Main.gamePaused || !Main.instance.IsActive || Lighting.UpdateEveryFrame && !Main.rand.NextBool(4))
            {
                return;
            }

            if (tile.TileFrameX == 36 && tile.TileFrameY % 36 == 0 && (int)Main.timeForVisualEffects % 7 == 0 && Main.rand.NextBool(3))
            {
                int goreType = Main.rand.Next(570, 573);
                Vector2 position = new Vector2(i * 16 + 8, j * 16 - 8);
                Vector2 velocity = new Vector2(Main.WindForVisuals * 2f, -0.5f);
                velocity.X *= 1f + Main.rand.NextFloat(-0.5f, 0.5f);
                velocity.Y *= 1f + Main.rand.NextFloat(-0.5f, 0.5f);
                if (goreType == 572)
                {
                    position.X -= 8f;
                }

                if (goreType == 571)
                {
                    position.X -= 4f;
                }

                Gore.NewGore(new EntitySource_TileUpdate(i, j), position, velocity, goreType, 0.8f);
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) 
		{
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/MusicBox/SpookyBiomeNightBoxTileGlow");

            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            int width = 16;
            int height = 16;
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;

            ulong randShakeEffect = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
            float drawPositionX = i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f;
            float drawPositionY = j * 16 - (int)Main.screenPosition.Y;
            for (int c = 0; c < 7; c++)
            {
                float shakeX = Utils.RandomInt(ref randShakeEffect, -10, 11) * 0.05f;
                float shakeY = Utils.RandomInt(ref randShakeEffect, -10, 11) * 0.05f;
                Main.spriteBatch.Draw(GlowTexture.Value, new Vector2(drawPositionX + shakeX, drawPositionY + shakeY + yOffset) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, width, height), new Color(100, 100, 100, 0), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
		}
    }
}