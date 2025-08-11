using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.Debug
{
    public class CatacombWeedKiller : ModItem
    {
        public override string Texture => "Spooky/Content/Items/BossSummon/CowBell";

        public override void SetDefaults()
        {                
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool? UseItem(Player player)
        {
            for (int i = 20; i < Main.maxTilesX - 20; i++)
            {
                for (int j = 20; j < Main.maxTilesY - 20; j++)
                {
                    Tile tile = Main.tile[i, j];

                    if (tile.TileType == ModContent.TileType<CatacombVines>() || tile.TileType == ModContent.TileType<CatacombWeeds>() || 
                    tile.TileType == ModContent.TileType<SporeMushroom>() || tile.TileType == ModContent.TileType<BigFlower>())
                    {
                        WorldGen.KillTile(i, j);
                    }
                }
            }

            return true;
        }
    }
}