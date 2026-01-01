using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class MortarLegs : ModItem
	{
		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MortarHead>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 18;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Yellow;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
			player.GetAttackSpeed(DamageClass.Ranged) += 0.07f;
			player.GetAttackSpeed(DamageClass.Magic) += 0.07f;
			player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.07f;
			player.maxMinions += 1;
			player.maxTurrets += 1;
			player.endurance += 0.03f;
		}
	}
}