using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class SirenHead : ModItem
	{
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            ArmorIDs.Head.Sets.DrawHead[equipSlot] = false;
        }
        public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 28;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}