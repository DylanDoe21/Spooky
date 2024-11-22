using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyBiome.GourdBlocks;

namespace Spooky.Content.Tiles.SpookyBiome.Gourds
{
    public class GourdWhite : ModTile
    {
        public static readonly SoundStyle CarveSound = new("Spooky/Content/Sounds/PumpkinCarve", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(165, 173, 177));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<GourdBlockWhiteItem>(), Main.rand.Next(5, 11));
        }

        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

            bool HasCarvingKit = ItemGlobal.ActiveItem(player).type == ModContent.ItemType<PumpkinCarvingKit>();

			player.cursorItemIconEnabled = HasCarvingKit ? true : false;
			player.cursorItemIconID = HasCarvingKit ? ModContent.ItemType<PumpkinCarvingKit>() : 0;
			player.cursorItemIconText = "";
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled = false;
			player.cursorItemIconID = 0;
		}

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (ItemGlobal.ActiveItem(player).type == ModContent.ItemType<PumpkinCarvingKit>())
            {
                SoundEngine.PlaySound(CarveSound, new Vector2(i * 16, j * 16));

                int left = i - Framing.GetTileSafely(i, j).TileFrameX / 18 % 3;
                int top = j - Framing.GetTileSafely(i, j).TileFrameY / 18 % 3;

                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 3; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        tile.TileType = (ushort)ModContent.TileType<GourdWhiteCarved>();
                    }
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, left, top, 12);
                }
            }

            return true;
        }
    }

    public class GourdWhiteCarved : GourdWhite
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(165, 173, 177));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
        }
        
        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

            bool HasCandle = ItemGlobal.ActiveItem(player).type == ModContent.ItemType<CandleItem>();

			player.cursorItemIconEnabled = HasCandle ? true : false;
			player.cursorItemIconID = HasCandle ? ModContent.ItemType<CandleItem>() : 0;
			player.cursorItemIconText = "";
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled = false;
			player.cursorItemIconID = 0;
		}

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (ItemGlobal.ActiveItem(player).type == ModContent.ItemType<CandleItem>() && player.ConsumeItem(ModContent.ItemType<CandleItem>()))
            {
                SoundEngine.PlaySound(SoundID.Dig, new Vector2(i * 16, j * 16));

                int left = i - Framing.GetTileSafely(i, j).TileFrameX / 18 % 3;
                int top = j - Framing.GetTileSafely(i, j).TileFrameY / 18 % 3;

                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 3; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        tile.TileType = (ushort)ModContent.TileType<GourdWhiteCarvedLit>();
                    }
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, left, top, 12);
                }
            }

            return true;
        }
    }

    public class GourdWhiteCarvedLit : GourdWhiteCarved
    {
        public override string Texture => "Spooky/Content/Tiles/SpookyBiome/Gourds/GourdWhiteCarved";

        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(165, 173, 177));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void MouseOver(int i, int j)
		{
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = false;
        }

        public override void MouseOverFar(int i, int j)
		{
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = false;
        }

        public override bool RightClick(int i, int j)
        {
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) 
		{
            Lighting.AddLight(new Vector2(i * 16, j * 16), Color.Coral.ToVector3());

            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Gourds/GourdWhiteCarvedGlow");

            Tile tile = Framing.GetTileSafely(i, j);
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