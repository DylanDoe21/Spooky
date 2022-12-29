using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class FleshStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retina Staff");
            Tooltip.SetDefault("Summon eyes that follow your cursor while holding down left click"
            + "\nRelease left click to fling the eyes, dealing double their original damage"
            + "\nOnly up to eight eyes can be active at once");
            Item.staff[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.channel = true;
			Item.width = 42;
            Item.height = 48;
			Item.useTime = 30;         
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item17;     
			Item.shoot = ModContent.ProjectileType<ControllableEye>();
			Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 8;
		}
    }
}