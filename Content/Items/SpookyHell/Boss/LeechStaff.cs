using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell.Boss
{
    public class LeechStaff : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Staff of the Swarm");
			Tooltip.SetDefault("Casts leeches that will swarm nearby enemies at the cost of 10 life"
			+ "\nLeeches cannot be summoned when below half health, and they do not take up minion slots"
			+ "\nIf you are below half health, leeches will sometimes heal you on enemy hits");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
        {
            Item.damage = 55; 
			Item.mana = 10;                        
            Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;  
			Item.autoReuse = true;                  
            Item.width = 58;
            Item.height = 48;
            Item.useTime = 25;
            Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2); 
            Item.UseSound = SoundID.DD2_BetsysWrathShot;
            Item.shoot = ModContent.ProjectileType<Leech>();  
            Item.shootSpeed = 8f;
        }

		public override bool CanUseItem(Player player)
		{
			return player.statLife >= (player.statLifeMax2 / 4);
		}
	}
}