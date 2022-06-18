using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class FlyScroll : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Scroll of the Flies");
			Tooltip.SetDefault("Summons flies that linger above you or any nearby target"
			+ "\nAfter a few seconds, they will fling themselves towards your cursor"
			+ "\nOnly up to 10 flies can exist at once");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.damage = 18;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.mana = 12;
			Item.width = 36;
            Item.height = 36;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = 5;
            Item.knockBack = 2f;
            Item.rare = 1;
           	Item.value = Item.buyPrice(silver: 50);
            Item.UseSound = SoundID.Item77;
            Item.shoot = ModContent.ProjectileType<FlyMinion>();
            Item.shootSpeed = 10f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 10;
		}
    }
}