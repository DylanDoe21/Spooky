using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    /*
    [LegacyName("SpookyWeedsTallGreen1")]
    [LegacyName("SpookyWeedsTallGreen2")]
    [LegacyName("SpookyWeedsTallGreen3")]
    */
    public class SpookyWeedsGreen : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(62, 95, 38));
            DustType = ModContent.DustType<SpookyGrassDustGreen>();
            HitSound = SoundID.Grass;
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

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);

			if (tileBelow.HasTile && tileBelow.TileType == ModContent.TileType<SpookyGrassGreen>()) 
            {
				return true;
			}
            else
            {
			    WorldGen.KillTile(i, j);
            }

			return true;
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
                    yield return new Item(ModContent.ItemType<SpookySeedsGreen>());
				}
            }
        }
    }
}