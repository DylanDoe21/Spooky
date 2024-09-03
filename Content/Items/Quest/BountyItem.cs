using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
 
namespace Spooky.Content.Items.Quest
{
	public class BountyItem1 : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 34;
			Item.rare = ItemRarityID.Quest;
		}
	}

    public class BountyItem2 : ModItem
	{
        public override string Texture => "Spooky/Content/NPCs/Quest/BanditBook";

        public override void SetDefaults()
		{
			Item.width = 32;
            Item.height = 42;
			Item.rare = ItemRarityID.Quest;
		} 
    }

    public class BountyItem3 : ModItem
	{
        public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 24;
			Item.rare = ItemRarityID.Quest;
		}
    }

    public class BountyItem4 : ModItem
	{
        public override string Texture => "Spooky/Content/NPCs/Quest/BanditBook";

        public override void SetDefaults()
		{
			Item.width = 32;
            Item.height = 42;
			Item.rare = ItemRarityID.Quest;
		}
    }
}