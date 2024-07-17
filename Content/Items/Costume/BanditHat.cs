using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class BanditHat : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
			ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = false;
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 18;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}