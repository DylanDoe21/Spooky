using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SporeShroomHead : ModItem, ISpecialArmorDraw
	{
		public string HeadTexture => "Spooky/Content/Items/SpiderCave/Armor/SporeShroomHeadHat";

		public Vector2 Offset => new Vector2(-3f, 20f);

		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SporeShroomBody>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 12;
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.LightRed;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SporeShroomBody>() && legs.type == ModContent.ItemType<SporeShroomLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SporeShroomArmor");
			player.GetModPlayer<SpookyPlayer>().SporeShroomSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Generic) += 10;
        }
	}
}