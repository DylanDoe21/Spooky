using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SpiritHorsemanBody : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiritHorsemanLegs>();

			int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

			ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }

		public override void SetDefaults() 
		{
			Item.defense = 7;
			Item.width = 34;
			Item.height = 22;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 3);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Generic) += 0.15f;
		}
	}
}