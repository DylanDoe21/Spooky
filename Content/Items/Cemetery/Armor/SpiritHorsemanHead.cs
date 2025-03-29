using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SpiritHorsemanHead : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiritHorsemanBody>();
        }

		public override void SetDefaults() 
		{
			Item.defense = 6;
			Item.width = 26;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 3);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SpiritHorsemanBody>() && legs.type == ModContent.ItemType<SpiritHorsemanLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SpiritHorsemanArmor");
			player.GetModPlayer<SpookyPlayer>().HorsemanSet = true;

			bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<PumpkinHead>()] <= 0;
			if (NotSpawned)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 
				ModContent.ProjectileType<PumpkinHead>(), 40, 0f, player.whoAmI);
			}
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Generic) += 12;
		}
	}
}