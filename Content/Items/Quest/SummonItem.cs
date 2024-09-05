using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Quest
{
    public class SummonItem1 : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
			Item.height = 32;
            Item.rare = ItemRarityID.Quest;
        }
    }

    public class SummonItem2 : SummonItem1
    {
        public override void SetDefaults()
        {
            Item.width = 36;
			Item.height = 34;
            Item.rare = ItemRarityID.Quest;
        }
    }

    public class SummonItem3 : SummonItem1
    {
        public override void SetDefaults()
        {
            Item.width = 26;
			Item.height = 26;
            Item.rare = ItemRarityID.Quest;
        }
    }

    public class SummonItem4 : SummonItem1
    {
        public override void SetDefaults()
        {
            Item.width = 16;
			Item.height = 32;
            Item.rare = ItemRarityID.Quest;
        }
    }
}