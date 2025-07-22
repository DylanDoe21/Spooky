using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.Minibiomes.Armor;

namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class TarSurpriseEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
			Item.height = 28;
            Item.useTime = 12;
			Item.useAnimation = 12;
            Item.consumable = true;
            Item.autoReuse = true;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(copper: 50);
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
		{
            int[] Items = { ModContent.ItemType<GoldrushPickaxe>(), ModContent.ItemType<TarBomb>(), 
            ModContent.ItemType<TarGun>(), ModContent.ItemType<TarFeatherTome>(),
            ModContent.ItemType<GoldrushHead>(), ModContent.ItemType<GoldrushBody>(), ModContent.ItemType<GoldrushLegs>() };

            resultType = Main.rand.Next(Items);
            resultStack = 1;
		}
    }
}