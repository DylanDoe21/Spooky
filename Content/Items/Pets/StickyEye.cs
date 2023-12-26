using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Buffs.Pets;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Items.Pets
{
	public class StickyEye : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 32;
			Item.height = 34;
			Item.shoot = ModContent.ProjectileType<StickyEyePet>();
			Item.buffType = ModContent.BuffType<StickyEyeBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}