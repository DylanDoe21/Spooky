using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class SentientFleshStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentient Retina Staff");
            Tooltip.SetDefault("Hold down left click to summon eyes that follow your cursor"
            + "\nReleasing left click will explode them, dealing massive damage and slowing enemies"
            + "\nOnly up to fifteen eyes can be active at once");
            Item.staff[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.channel = true;
			Item.width = 54;
            Item.height = 72;
			Item.useTime = 30;         
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 10);
			Item.UseSound = SoundID.Item17;     
			Item.shoot = ModContent.ProjectileType<ControllableEyeBig>();
			Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 15;
		}
    }
}