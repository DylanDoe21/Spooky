using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class SuspiciousBrownie : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 20;
			Item.height = 20;
			Item.shoot = ModContent.ProjectileType<Columbo>();
			Item.buffType = ModContent.BuffType<ColumboBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}