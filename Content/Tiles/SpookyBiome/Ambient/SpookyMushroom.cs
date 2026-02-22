using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class SpookyMushroom : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<SpookyStone>(), ModContent.TileType<MushroomMoss>() };
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(166, 158, 187));
            DustType = DustID.Slush;
            HitSound = SoundID.Grass;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float divide = 500f;

            r = 155f / divide;
            g = 83f / divide;
            b = 250f / divide;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -14;
            height = 32;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
            Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];
            if (nearestPlayer.active)
            {                
                if (nearestPlayer.HeldItem.type == ItemID.Sickle)
				{
                    yield return new Item(ItemID.Hay, Main.rand.Next(1, 2 + 1));
				}
                
                if (Main.rand.NextBool(20))
				{
                    yield return new Item(ModContent.ItemType<MushroomMossSeeds>());
				}
            }
        }
    }
}