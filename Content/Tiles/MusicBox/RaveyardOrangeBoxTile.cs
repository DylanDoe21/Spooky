using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.MusicBox
{
    public class RaveyardOrangeBoxTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default(AnchorData);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = -2;
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
            player.cursorItemIconID = ModContent.ItemType<RaveyardOrangeBox>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
            if (Main.tile[i, j].TileFrameX >= 36)
            {
                r = Color.Orange.R / 450f;
                g = Color.Orange.G / 450f;
                b = Color.Orange.B / 450f;
            }
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
    }
}