using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.SpookyBiome.Furniture;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class GourdGiant : ModTile
    {
        public static readonly SoundStyle CarveSound = new("Spooky/Content/Sounds/PumpkinCarve", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(147, 33, 27));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled = player.HasItem(ModContent.ItemType<PumpkinCarvingKit>()) ? true : false;
			player.cursorItemIconID = player.HasItem(ModContent.ItemType<PumpkinCarvingKit>()) ? ModContent.ItemType<PumpkinCarvingKit>() : 0;
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
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<PumpkinCarvingKit>()))
            {
                SoundEngine.PlaySound(CarveSound, new Vector2(i * 16, j * 16));

                int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
                int top = j - Main.tile[i, j].TileFrameY / 18 % 4;

                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 4; y++)
                    {
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<GourdGiantCarved>();
                    }
                }
            }

            return true;
        }
    }

    public class GourdGiantCarved : GourdGiant
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(147, 33, 27));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled = player.HasItem(ModContent.ItemType<CandleItem>()) ? true : false;
			player.cursorItemIconID = player.HasItem(ModContent.ItemType<CandleItem>()) ? ModContent.ItemType<CandleItem>() : 0;
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
            if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<CandleItem>()))
            {
                SoundEngine.PlaySound(SoundID.Dig, new Vector2(i * 16, j * 16));

                int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
                int top = j - Main.tile[i, j].TileFrameY / 18 % 4;

                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 4; y++)
                    {
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<GourdGiantCarvedLit>();
                    }
                }
            }

            return true;
        }
    }

    public class GourdGiantCarvedLit : GourdGiantCarved
    {
        public override string Texture => "Spooky/Content/Tiles/SpookyBiome/Ambient/GourdGiantCarved";

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

            Texture2D flameTexture = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Ambient/GourdGiantCarvedGlow").Value;

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
                Main.spriteBatch.Draw(flameTexture, new Vector2(drawPositionX + shakeX, drawPositionY + shakeY + yOffset) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, width, height), new Color(100, 100, 100, 0), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
		}
    }
}