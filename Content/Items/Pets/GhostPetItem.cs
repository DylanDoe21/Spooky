using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class GhostPetItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spooky Ghost");
			// Tooltip.SetDefault("Summons a little ghost to provide light");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 30;
			Item.height = 34;
			Item.shoot = ModContent.ProjectileType<GhostPet>();
			Item.buffType = ModContent.BuffType<GhostPetBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}