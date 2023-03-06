using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class SkullKey : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Key");
			Tooltip.SetDefault("Summons a pet skull to float above you");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 20;
			Item.height = 24;
			Item.shoot = ModContent.ProjectileType<SkullEmoji>();
			Item.buffType = ModContent.BuffType<SkullEmojiBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }

        public override bool CanUseItem(Player player)
		{
			return player.miscEquips[0].IsAir;
		}
	}
}