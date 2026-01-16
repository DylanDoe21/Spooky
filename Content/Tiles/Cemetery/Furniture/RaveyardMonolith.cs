using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
    public class RaveyardMonolith : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            RegisterItemDrop(ModContent.ItemType<RaveyardMonolithItem>());
            AddMapEntry(new Color(197, 187, 215));
            DustType = DustID.Bone;
            HitSound = SoundID.Dig;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) 
        {
            return true;
        }

        public void HandleActivation(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int left = i - tile.TileFrameX / 18 % 2;
			int top = j - tile.TileFrameY / 18 % 2;

            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    Tile CheckTile = Framing.GetTileSafely(x, y);
                    CheckTile.TileType = (ushort)ModContent.TileType<RaveyardMonolithOn>();
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, left, top, 6);
            }
        }

        public override void HitWire(int i, int j)
        {
            HandleActivation(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));

            HandleActivation(i, j);

            return true;
        }

        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

			player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<RaveyardMonolithItem>();
		}
	}

    public class RaveyardMonolithOn : RaveyardMonolith
    {
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> SpotlightTexture;

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            SpotlightTexture ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/LightConeUp");

            Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
			{
				Vector2 frameOrigin = new Vector2(SpotlightTexture.Width() / 2f, SpotlightTexture.Height());
				Vector2 drawPos = new Vector2(i * 16, j * 16) + new Vector2(210, -155) - Main.screenPosition + frameOrigin + new Vector2(-SpotlightTexture.Width() / 2, 0f);

                float Rotation = Main.GlobalTimeWrappedHourly * 0.5f;

				spriteBatch.Draw(SpotlightTexture.Value, drawPos, null, new Color(Main.DiscoColor.R, Main.DiscoColor.G, Main.DiscoColor.B, 0), MathF.Sin(Rotation), frameOrigin, 0.35f, SpriteEffects.None, 0);
                spriteBatch.Draw(SpotlightTexture.Value, drawPos, null, new Color(Main.DiscoColor.G, Main.DiscoColor.B, Main.DiscoColor.R, 0), MathF.Sin(-Rotation), frameOrigin, 0.35f, SpriteEffects.None, 0);
                spriteBatch.Draw(SpotlightTexture.Value, drawPos, null, new Color(Main.DiscoColor.B, Main.DiscoColor.R, Main.DiscoColor.G, 0), MathF.Cos(Rotation), frameOrigin, 0.35f, SpriteEffects.None, 0);
			}

            return true;
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.5f);
        }
        
        public void HandleActivation(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int left = i - tile.TileFrameX / 18 % 2;
			int top = j - tile.TileFrameY / 18 % 2;

            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    Tile CheckTile = Framing.GetTileSafely(x, y);
					CheckTile.TileType = (ushort)ModContent.TileType<RaveyardMonolith>();
				}
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, left, top, 6);
            }
        }

        public override void HitWire(int i, int j)
        {
            HandleActivation(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));

            HandleActivation(i, j);

            return true;
        }

        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

			player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<RaveyardMonolithItem>();
		}
    }
}