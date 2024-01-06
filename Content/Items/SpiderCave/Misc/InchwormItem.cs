using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.NPCs.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Misc
{
    public class InchwormGreenItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BlackDragonfly);
            Item.rare = ItemRarityID.Blue;
			Item.makeNPC = (short)ModContent.NPCType<Inchworm1>();
            Item.bait = 10;
        }
    }

    public class InchwormOrangeItem : InchwormGreenItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BlackDragonfly);
            Item.rare = ItemRarityID.Blue;
			Item.makeNPC = (short)ModContent.NPCType<Inchworm2>();
            Item.bait = 10;
        }
    }

    public class InchwormBlackItem : InchwormGreenItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BlackDragonfly);
            Item.rare = ItemRarityID.Blue;
			Item.makeNPC = (short)ModContent.NPCType<Inchworm3>();
            Item.bait = 10;
        }
    }
}