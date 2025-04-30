using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.Tiles.Minibiomes.Desert
{
    public class PlantFossilItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PlantFossil>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
		{
			if (Main.rand.NextBool(35))
			{
				resultType = ModContent.ItemType<FossilSeed>();
                resultStack = 1;
			}
		}
    }
}