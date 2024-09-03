using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class NoseCultistHood : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 28;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}

		public override void EquipFrameEffects(Player player, EquipType type)
        {
			player.GetModPlayer<SpookyPlayer>().NoseCultistDisguise1 = true;
		}
	}
}