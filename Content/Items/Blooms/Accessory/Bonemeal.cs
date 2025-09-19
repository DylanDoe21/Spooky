using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.Items.Blooms.Accessory
{
    public class Bonemeal : ModItem
    {
        public static List<ushort> Blooms = new()
		{
			(ushort)ModContent.TileType<CemeteryBloomPlant>(),
			(ushort)ModContent.TileType<DandelionBloomPlant>(),
			(ushort)ModContent.TileType<DragonfruitBloomPlant>(),
            (ushort)ModContent.TileType<FallBloomPlant>(),
            (ushort)ModContent.TileType<FossilBloomPlant>(),
            (ushort)ModContent.TileType<SeaBloomPlant>(),
            (ushort)ModContent.TileType<SpringBloomPlant>(),
            (ushort)ModContent.TileType<SummerBloomPlant>(),
            (ushort)ModContent.TileType<VegetableBloomPlant>(),
            (ushort)ModContent.TileType<WinterBloomPlant>()
		};

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            if (Main.rand.NextBool(350))
            {
                for (int i = (int)(player.Center.X / 16) - 20; i <= (int)(player.Center.X / 16) + 20; i++)
                {
                    for (int j = (int)(player.Center.Y / 16) - 20; j <= (int)(player.Center.Y / 16) + 20; j++)
                    {
                        Tile tile = Main.tile[i, j];
                        if (Main.rand.NextBool(30) && tile.TileFrameX < 216 && Blooms.Contains(tile.TileType))
                        {
                            int left = i - tile.TileFrameX / 18 % 3;
                            int top = j - tile.TileFrameY / 18 % 3;

                            for (int x = left; x < left + 3; x++)
                            {
                                for (int y = top; y < top + 3; y++)
                                {
                                    Tile CheckTile = Framing.GetTileSafely(x, y);
                                    CheckTile.TileFrameX += 54;
                                }
                            }

                            if (Main.netMode != NetmodeID.SinglePlayer)
                            {
                                NetMessage.SendTileSquare(-1, left, top, 6);
                            }
                        }
                    }
                }
            }
        }
    }
}