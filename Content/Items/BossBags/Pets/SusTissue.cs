using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.BossBags.Pets
{
	public class SusTissue : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Suspicious Looking Tissue");
			Tooltip.SetDefault("Summons a baby moco pet\n'The most unsanitary tissue known to mankind'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 30;
			Item.height = 34;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.shoot = ModContent.ProjectileType<MocoPet>();
			Item.buffType = ModContent.BuffType<MocoPetBuff>();
		}

		/*
		public override void UseStyle(Player player)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0) 
            {
				player.AddBuff(Item.buffType, 3600, true);
			}
		}
		*/

		public override bool CanUseItem(Player player)
		{
			return player.miscEquips[0].IsAir;
		}
	}
}