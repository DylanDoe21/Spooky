using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Quest
{
    public class SummonItem1 : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
			Item.height = 32;
            Item.rare = ItemRarityID.Quest;
        }
    }

    public class SummonItem2 : SummonItem1
    {
    }

    public class SummonItem3 : SummonItem1
    {
    }

    public class SummonItem4 : SummonItem1
    {
    }
}